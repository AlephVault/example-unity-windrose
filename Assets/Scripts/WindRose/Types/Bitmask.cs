using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WindRose.Types
{
    public class Bitmask
    {
        public enum CheckType { ANY_BLOCKED, ANY_FREE, ALL_BLOCKED, ALL_FREE }
        private uint[] bits;
        public readonly uint Width;
        public readonly uint Height;

        /**
         * Creates a bitmask by cloning an existing one.
         */
        public Bitmask(Bitmask source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Width = source.Width;
            Height = source.Height;
            bits = (uint[]) source.bits.Clone();
        }

        /**
         * Creates a bitmask by filling its space given by width and height, and a value to fill.
         */
        public Bitmask(uint width, uint height, bool initial = false)
        {
            uint size = width * height;
            if (size == 0) throw new ArgumentException("Both width and height must be > 0");
            bits = new uint[(size + 31) / 32];
            Width = width;
            Height = height;
            Fill(initial);
        }

        /**
         * Creates a bitmask by passing a texture and considering the black pixels as 0, and others as 1.
         */
        public Bitmask(Texture2D source)
        {
            if (source == null) throw new ArgumentNullException("source");
            uint width = (uint) source.width;
            uint height = (uint) source.height;
            uint size = width * height;
            if (size == 0) throw new ArgumentException("Both width and height in the texture must be > 0");
            bits = new uint[(size + 31) / 32];
            Width = width;
            Height = height;
            Color32[] pixels = source.GetPixels32();
            Flip(pixels, source.width, source.height);
            IEnumerable<bool> pixelToBit = pixels.Select((Color32 color) => color != Color.black);
            uint rowIdx = 0, colIdx = 0;
            foreach (bool value in pixelToBit)
            {
                if (colIdx == 32)
                {
                    rowIdx++;
                    colIdx = 0;
                }

                if (value)
                {
                    bits[rowIdx] |= (uint)(1 << (int)(colIdx));
                }
                else
                {
                    bits[rowIdx] &= ~(uint)(1 << (int)(colIdx));
                }

                colIdx++;
            }
        }

        private static void Flip(Color32[] pixels, int width, int height)
        {
            int iterations = height / 2;
            Color32[] buffer = new Color32[width];
            for(int row = 0; row < iterations; row++)
            {
                int index = row * width;
                int reflectedIndex = (height - row - 1) * width;
                Array.Copy(pixels, index, buffer, 0, width);
                Array.Copy(pixels, reflectedIndex, pixels, index, width);
                Array.Copy(buffer, 0, pixels, reflectedIndex, width);
            }
        }

        /**
         * Creates a texture from the current bitmask. The texture will use 16bits per pixel, which is a crap but
         *   we cannot do anything.
         */
        public Texture2D Export()
        {
            Texture2D texture = new Texture2D((int) Width, (int) Height, TextureFormat.ARGB32, false);
            Color32[] pixels = Enumerable.Range(0, (int)(Width * Height)).Select<int, Color32>(
                (int idx) => ((bits[idx / 32] & (1 << (int)(idx % 32))) != 0) ? Color.white : Color.black
            ).ToArray();
            Flip(pixels, texture.width, texture.height);
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }

        /**
         * Sets or gets a bit in the mask.
         */
        public bool this[uint x, uint y]
        {
            get
            {
                uint flat_index = y * Width + x;
                return (this.bits[flat_index / 32] & (uint)(1 << (int)(flat_index % 32))) != 0;
            }
            set
            {
                uint flat_index = y * Width + x;
                if (value)
                {
                    this.bits[flat_index / 32] |= (uint)(1 << (int)(flat_index % 32));
                }
                else
                {
                    this.bits[flat_index / 32] &= ~(uint)(1 << (int)(flat_index % 32));
                }
            }
        }

        /**
         * Fills the array with 1s or 0s, depending on the passed values (true=1, false=0).
         */
        public void Fill(bool value)
        {
            uint filler = value ? 0xffffffff : 0;
            for (int x = 0; x < bits.Length; x++) bits[x] = filler;
        }

        /**
         * Translates the array to a new width and height. It can even apply an offset to the
         *   array being translated. E.g. if the array has (4, 4) filled with 1s and I translate
         *   to (6, 6) with offset (1, 1) the final array would be 000000 011110 011110 011110
         *   011110 000000.
         *   
         * If offset has any component lt 0, some information will be lost. If offset has any
         *   component > (newDimension - olddimension) in the respective axis, some information
         *   will be lost.
         * 
         * Additionally, a filling will be specified for the new data if the space is bigger.
         */
        public Bitmask Translated(uint newWidth, uint newHeight, int offsetX, int offsetY, bool newFillingValue = false)
        {
            Bitmask result = new Bitmask(newWidth, newHeight, newFillingValue);
            if (offsetX < newWidth && offsetY < newHeight && offsetX + newWidth > 0 && offsetY + newHeight > 0)
            {
                // startx and endx, like their y-siblings, belong to the translated array.
                int startX = Utils.Values.Max<int>(offsetX, 0);
                int endX = Utils.Values.Min<int>(offsetX + (int)Width, (int)newWidth);
                int startY = Utils.Values.Max<int>(offsetY, 0);
                int endY = Utils.Values.Min<int>(offsetY + (int)Height, (int)newHeight);

                // origin array use coordinates subtracting offsetX and offsetY.
                for(int x = startX; x < endX; x++)
                {
                    for(int y = startY; y < endY; y++)
                    {
                        result[(uint)x, (uint)y] = this[(uint)(x - offsetX), (uint)(y - offsetY)];
                    }
                }
            }
            return result;
        }

        /**
         * Returns a clone of the current bitmask.
         */
        public Bitmask Clone()
        {
            return new Bitmask(this);
        }

        /**
         * Applies bitwise-OR, in-place, to the whole mask.
         */
        public void Unite(Bitmask other)
        {
            CheckSameDimensions(other);
            int l = bits.Length;
            for(int i = 0; i < l; i++)
            {
                bits[i] |= other.bits[i];
            }
        }

        /**
         * Applies bitwise-AND, in-place, to the whole mask.
         */
        public void Intersect(Bitmask other)
        {
            CheckSameDimensions(other);
            int l = bits.Length;
            for (int i = 0; i < l; i++)
            {
                bits[i] &= other.bits[i];
            }
        }

        /**
         * Applies bitwise difference (s & ~o), in-place, to the whole mask.
         */
        public void Subtract(Bitmask other)
        {
            CheckSameDimensions(other);
            int l = bits.Length;
            for (int i = 0; i < l; i++)
            {
                bits[i] &= ~other.bits[i];
            }
        }

        /**
         * Applies bitwise xor, in-place, to the whole mask.
         */
        public void SymmetricSubtract(Bitmask other)
        {
            CheckSameDimensions(other);
            int l = bits.Length;
            for (int i = 0; i < l; i++)
            {
                bits[i] ^= other.bits[i];
            }
        }

        /**
         * Applies bitwise-complement, in place, to the whole mask.
         */
        public void Invert()
        {
            int l = bits.Length;
            for (int i = 0; i < l; i++)
            {
                bits[i] = ~bits[i];
            }
        }

        public static Bitmask operator|(Bitmask self, Bitmask other)
        {
            Bitmask result = self.Clone();
            result.Unite(other);
            return result;
        }

        public static Bitmask operator&(Bitmask self, Bitmask other)
        {
            Bitmask result = self.Clone();
            result.Intersect(other);
            return result;
        }

        public static Bitmask operator~(Bitmask self)
        {
            Bitmask result = self.Clone();
            result.Invert();
            return result;
        }

        public static Bitmask operator-(Bitmask self, Bitmask other)
        {
            Bitmask result = self.Clone();
            result.Subtract(other);
            return result;
        }

        public static Bitmask operator^(Bitmask self, Bitmask other)
        {
            Bitmask result = self.Clone();
            result.SymmetricSubtract(other);
            return result;
        }

        /**
         * Mass-updating/fetching values in the array.
         */
        
        /**
         * Square-setting a value in the array.
         */
        public void SetSquare(uint xi, uint yi, uint xf, uint yf, bool blocked)
        {
            xi = Utils.Values.Clamp<uint>(0, xi, Width - 1);
            yi = Utils.Values.Clamp<uint>(0, yi, Height - 1);
            xf = Utils.Values.Clamp<uint>(0, xf, Width - 1);
            yf = Utils.Values.Clamp<uint>(0, yf, Height - 1);

            uint xi_ = Utils.Values.Min<uint>(xi, xf);
            uint xf_ = Utils.Values.Max<uint>(xi, xf);
            uint yi_ = Utils.Values.Min<uint>(yi, yf);
            uint yf_ = Utils.Values.Max<uint>(yi, yf);

            for (uint x = xi_; x <= xf_; x++)
            {
                for (uint y = yi_; y <= yf_; y++)
                {
                    this[x, y] = blocked;
                }
            }
        }

        /**
         * Square-getting a value in the array.
         */
        public bool GetSquare(uint xi, uint yi, uint xf, uint yf, CheckType checkType)
        {
            xi = Utils.Values.Clamp<uint>(0, xi, Width - 1);
            yi = Utils.Values.Clamp<uint>(0, yi, Height - 1);
            xf = Utils.Values.Clamp<uint>(0, xf, Width - 1);
            yf = Utils.Values.Clamp<uint>(0, yf, Height - 1);

            uint xi_ = Utils.Values.Min<uint>(xi, xf);
            uint xf_ = Utils.Values.Max<uint>(xi, xf);
            uint yi_ = Utils.Values.Min<uint>(yi, yf);
            uint yf_ = Utils.Values.Max<uint>(yi, yf);

            for (uint x = xi_; x <= xf_; x++)
            {
                for (uint y = yi_; y <= yf_; y++)
                {
                    switch (checkType)
                    {
                        case CheckType.ANY_BLOCKED:
                            if (this[x, y]) { return true; }
                            break;
                        case CheckType.ANY_FREE:
                            if (!this[x, y]) { return true; }
                            break;
                        case CheckType.ALL_BLOCKED:
                            if (!this[x, y]) { return false; }
                            break;
                        case CheckType.ALL_FREE:
                            if (this[x, y]) { return false; }
                            break;
                        default:
                            return false;
                    }
                }
            }
            switch (checkType)
            {
                case CheckType.ALL_BLOCKED:
                case CheckType.ALL_FREE:
                    return true;
                default:
                    return false;
            }
        }

        public void SetRow(uint xi, uint xf, uint y, bool blocked)
        {
            SetSquare(xi, y, xf, y, blocked);
        }

        public bool GetRow(uint xi, uint xf, uint y, CheckType checkType)
        {
            return GetSquare(xi, y, xf, y, checkType);
        }

        public void SetColumn(uint x, uint yi, uint yf, bool blocked)
        {
            SetSquare(x, yi, x, yf, blocked);
        }

        public bool GetColumn(uint x, uint yi, uint yf, CheckType checkType)
        {
            return GetSquare(x, yi, x, yf, checkType);
        }

        public void SetCell(uint x, uint y, bool blocked)
        {
            this[Utils.Values.Clamp<uint>(0, x, Width - 1), Utils.Values.Clamp<uint>(0, y, Height - 1)] = blocked;
        }

        public bool GetCell(uint x, uint y)
        {
            return this[Utils.Values.Clamp<uint>(0, x, Width - 1), Utils.Values.Clamp<uint>(0, y, Height - 1)];
        }

        private void CheckSameDimensions(Bitmask other)
        {
            if (Width != other.Width || Height != other.Height) throw new ArgumentException("Dimensions must match between bitmasks");
        }
    }
}
