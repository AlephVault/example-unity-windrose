using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Support.Utils;

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
                int startX = Values.Max<int>(offsetX, 0);
                int endX = Values.Min<int>(offsetX + (int)Width, (int)newWidth);
                int startY = Values.Max<int>(offsetY, 0);
                int endY = Values.Min<int>(offsetY + (int)Height, (int)newHeight);

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
            xi = Values.Clamp<uint>(0, xi, Width - 1);
            yi = Values.Clamp<uint>(0, yi, Height - 1);
            xf = Values.Clamp<uint>(0, xf, Width - 1);
            yf = Values.Clamp<uint>(0, yf, Height - 1);

            uint xi_ = Values.Min<uint>(xi, xf);
            uint xf_ = Values.Max<uint>(xi, xf);
            uint yi_ = Values.Min<uint>(yi, yf);
            uint yf_ = Values.Max<uint>(yi, yf);

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
            xi = Values.Clamp<uint>(0, xi, Width - 1);
            yi = Values.Clamp<uint>(0, yi, Height - 1);
            xf = Values.Clamp<uint>(0, xf, Width - 1);
            yf = Values.Clamp<uint>(0, yf, Height - 1);

            uint xi_ = Values.Min<uint>(xi, xf);
            uint xf_ = Values.Max<uint>(xi, xf);
            uint yi_ = Values.Min<uint>(yi, yf);
            uint yf_ = Values.Max<uint>(yi, yf);

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
            this[Values.Clamp<uint>(0, x, Width - 1), Values.Clamp<uint>(0, y, Height - 1)] = blocked;
        }

        public bool GetCell(uint x, uint y)
        {
            return this[Values.Clamp<uint>(0, x, Width - 1), Values.Clamp<uint>(0, y, Height - 1)];
        }

        private void CheckSameDimensions(Bitmask other)
        {
            if (Width != other.Width || Height != other.Height) throw new ArgumentException("Dimensions must match between bitmasks");
        }
    }
}
