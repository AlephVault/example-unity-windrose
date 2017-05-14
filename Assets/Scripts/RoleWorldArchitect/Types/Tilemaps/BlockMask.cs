using System;
using System.Text;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Types
    {
        namespace Tilemaps
        {
            public class BlockMask
            {
                private static string Repeat(int width, char target)
                {
                    return "".PadRight(width, target);
                }

                private static string EnforceMask(string content, char free, char blocked)
                {
                    StringBuilder result = new StringBuilder();
                    for (int index = 0; index < content.Length; index++)
                    {
                        char currentChar = content[index];
                        result.Append(currentChar != blocked ? free : blocked);
                    }
                    return result.ToString();
                }

                public static string[] PadMask(string content, uint width, uint height, char free = '0', char blocked = '1', uint offsetX = 0, uint offsetY = 0)
                {
                    return PadMask(content == null ? null : content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None), width, height, free, blocked, offsetX, offsetY);
                }

                public static string[] PadMask(string[] content, uint width, uint height, char free = '0', char blocked = '1', uint offsetX = 0, uint offsetY = 0)
                {
                    width = Utils.Values.Max<uint>(width, 1);
                    height = Utils.Values.Max<uint>(height, 1);
                    offsetX = Utils.Values.Clamp<uint>(0, offsetX, width - 1);
                    offsetY = Utils.Values.Clamp<uint>(0, offsetY, height - 1);

                    string[] result = new string[height];
                    uint endingRow = offsetY + (uint) content.Length;
                    uint rowIdx = 0;
                    uint contentIdx = 0;
                    for (; rowIdx < offsetY; rowIdx++)
                    {
                        result[rowIdx] = Repeat((int)width, free);
                    }
                    for (; rowIdx < endingRow; rowIdx++)
                    {
                        result[rowIdx] = (Repeat((int)offsetX, free) + EnforceMask(content[contentIdx++], free, blocked)).PadRight((int)width, free).Substring(0, (int)width);
                    }
                    for (; rowIdx < height; rowIdx++)
                    {
                        result[rowIdx] = Repeat((int)width, free);
                    }

                    return result;
                }

                public enum CheckType { ANY_BLOCKED, ANY_FREE, ALL_BLOCKED, ALL_FREE }

                public readonly uint width, height;
                private uint[] bits;

                public BlockMask(uint width, uint height)
                {
                    this.width = Utils.Values.Clamp<uint>(1, width, 100);
                    this.height = Utils.Values.Clamp<uint>(1, height, 100);
                    this.bits = new uint[(this.width * this.height + 31) / 32];
                }

                public BlockMask(uint width, uint height, string content, char freeChar = '0', char blockedChar = '1') : this(width, height)
                {
                    SetWhole(content, freeChar, blockedChar);
                }

                public BlockMask(uint width, uint height, string[] content, char freeChar = '0', char blockedChar = '1') : this(width, height)
                {
                    SetWhole(content, freeChar, blockedChar);
                }

                public void SetBit(uint x, uint y, bool bit)
                {
                    uint flat_index = y * width + x;
                    if (bit)
                    {
                        this.bits[flat_index / 32] |= (uint)(1 << (int)(flat_index % 32));
                    }
                    else
                    {
                        this.bits[flat_index / 32] &= ~(uint)(1 << (int)(flat_index % 32));
                    }
                }

                public bool GetBit(uint x, uint y)
                {
                    uint flat_index = y * width + x;
                    return (this.bits[flat_index / 32] & (uint)(1 << (int)(flat_index % 32))) != 0;
                }

                public bool this[uint x, uint y]
                {
                    get
                    {
                        return GetBit(x, y);
                    }
                    set
                    {
                        SetBit(x, y, value);
                    }
                }

                public void SetSquare(uint xi, uint yi, uint xf, uint yf, bool blocked)
                {
                    xi = Utils.Values.Clamp<uint>(0, xi, width - 1);
                    yi = Utils.Values.Clamp<uint>(0, yi, height - 1);
                    xf = Utils.Values.Clamp<uint>(0, xf, width - 1);
                    yf = Utils.Values.Clamp<uint>(0, yf, height - 1);

                    uint xi_ = Utils.Values.Min<uint>(xi, xf);
                    uint xf_ = Utils.Values.Max<uint>(xi, xf);
                    uint yi_ = Utils.Values.Min<uint>(yi, yf);
                    uint yf_ = Utils.Values.Max<uint>(yi, yf);

                    for (uint x = xi; x <= xf; x++)
                    {
                        for (uint y = yi; y <= yf; y++)
                        {
                            SetBit(x, y, blocked);
                        }
                    }
                }

                public bool GetSquare(uint xi, uint yi, uint xf, uint yf, CheckType checkType)
                {
                    xi = Utils.Values.Clamp<uint>(0, xi, width - 1);
                    yi = Utils.Values.Clamp<uint>(0, yi, height - 1);
                    xf = Utils.Values.Clamp<uint>(0, xf, width - 1);
                    yf = Utils.Values.Clamp<uint>(0, yf, height - 1);

                    uint xi_ = Utils.Values.Min<uint>(xi, xf);
                    uint xf_ = Utils.Values.Max<uint>(xi, xf);
                    uint yi_ = Utils.Values.Min<uint>(yi, yf);
                    uint yf_ = Utils.Values.Max<uint>(yi, yf);

                    for (uint x = xi; x <= xf; x++)
                    {
                        for (uint y = yi; y <= yf; y++)
                        {
                            switch (checkType)
                            {
                                case CheckType.ANY_BLOCKED:
                                    if (!GetBit(x, y)) { return true; }
                                    break;
                                case CheckType.ANY_FREE:
                                    if (GetBit(x, y)) { return true; }
                                    break;
                                case CheckType.ALL_BLOCKED:
                                    if (!GetBit(x, y)) { return false; }
                                    break;
                                case CheckType.ALL_FREE:
                                    if (GetBit(x, y)) { return false; }
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
                    SetBit(Utils.Values.Clamp<uint>(0, x, width - 1), Utils.Values.Clamp<uint>(0, y, height - 1), blocked);
                }

                public bool GetCell(uint x, uint y)
                {
                    return GetBit(Utils.Values.Clamp<uint>(0, x, width - 1), Utils.Values.Clamp<uint>(0, y, height - 1));
                }

                public void SetWhole(string content, char freeChar = '0', char blockedChar = '1')
                {
                    SetWhole(content == null ? null : content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None), freeChar, blockedChar);
                }

                public void SetWhole(string[] content, char freeChar = '0', char blockedChar = '1')
                {
                    if (content == null)
                    {
                        throw new System.ArgumentNullException("Content must not be null");
                    }

                    if (content.Length != height)
                    {
                        throw new System.ArgumentException("Content must be one line per row");
                    }

                    foreach (string row in content)
                    {
                        if (row == null || row.Length != width)
                        {
                            throw new System.ArgumentException("Each row must have one character per cell");
                        }
                    }

                    uint rowIdx = 0;
                    uint colIdx = 0;

                    for (uint x = 0; x < width; x++)
                    {
                        for (uint y = 0; y < height; y++)
                        {
                            if (colIdx == 32)
                            {
                                colIdx = 0;
                                rowIdx++;
                            }

                            char posChar = content[y][(int)x];
                            if (posChar == freeChar)
                            {
                                this.bits[rowIdx] &= ~(uint)(1 << (int)(colIdx));
                            }
                            else if (posChar == blockedChar)
                            {
                                this.bits[rowIdx] |= (uint)(1 << (int)(colIdx));
                            }
                            else
                            {
                                throw new System.ArgumentException("Each position in the content must hold either character " + freeChar + " or character " + blockedChar);
                            }
                        }
                    }
                }
            }
        }
    }
}