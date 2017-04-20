using System;

namespace RoleWorldArchitect
{
    namespace Types
    {
        namespace ObjectLayerHelpers
        {
            public class SolidMask
            {
                public class InvalidSpatialSpecException : Exception
                {
                    public InvalidSpatialSpecException() { }
                    public InvalidSpatialSpecException(string message) : base(message) { }
                    public InvalidSpatialSpecException(string message, System.Exception inner) : base(message, inner) { }
                }

                public class CannotDecrementException : Exception
                {
                    public CannotDecrementException() { }
                    public CannotDecrementException(string message) : base(message) { }
                    public CannotDecrementException(string message, System.Exception inner) : base(message, inner) { }
                }

                public class CannotIncrementException : Exception
                {
                    public CannotIncrementException() { }
                    public CannotIncrementException(string message) : base(message) { }
                    public CannotIncrementException(string message, System.Exception inner) : base(message, inner) { }
                }

                public readonly uint width, height;
                private ushort[] positions;

                public SolidMask(uint width, uint height)
                {
                    this.width = Utils.Values.Clamp<uint>(1, width, 100);
                    this.height = Utils.Values.Clamp<uint>(1, height, 100);
                    this.positions = new ushort[this.width * this.height];
                    Array.Clear(this.positions, 0, (int)(this.width * this.height));
                }

                /**
                 * 
                 * With this class we ensure we can update counters on each position in the mask
                 *   so we can ensure whether the position is "busy" or not. A busy position is
                 *   only considered to be like that when occupied by "solid" objects. This mask
                 *   only accounts for the counters, and not for additional functionalities.
                 * 
                 */

                public void CheckDimensions(uint x, uint y, uint width, uint height)
                {
                    if (x + width > this.width || x + height > this.height)
                    {
                        throw new InvalidSpatialSpecException("Dimensions " + width + "x" + height + " starting at (" + x + ", " + y + ") cannot be contained on a map of " + this.width + "x" + this.height);
                    }
                }

                public void IncSquare(uint x, uint y, uint width, uint height)
                {
                    CheckDimensions(x, y, width, height);
                    uint yEnd = y + height;
                    for (uint j = y; j < yEnd; j++)
                    {
                        uint offset = j * this.width + x;
                        for (uint i = 0; i < width; i++)
                        {
                            if (this.positions[offset] < ushort.MaxValue)
                            {
                                this.positions[offset++]++;
                            }
                            else
                            {
                                throw new CannotIncrementException("Cannot increment position (" + x + ", " + y + ") beyond its maximum");
                            }
                        }
                    }
                }

                public void IncRow(uint x, uint y, uint width)
                {
                    IncSquare(x, y, width, 1);
                }

                public void IncColumn(uint x, uint y, uint height)
                {
                    IncSquare(x, y, 1, height);
                }

                public void DecSquare(uint x, uint y, uint width, uint height)
                {
                    CheckDimensions(x, y, width, height);
                    uint yEnd = y + height;
                    for (uint j = y; j < yEnd; j++)
                    {
                        uint offset = j * this.width + x;
                        for (uint i = 0; i < width; i++)
                        {
                            if (this.positions[offset] > ushort.MinValue)
                            {
                                this.positions[offset++]--;
                            }
                            else
                            {
                                throw new CannotIncrementException("Cannot decrement position (" + x + ", " + y + ") beyond its maximum");
                            }
                        }
                    }
                }

                public void DecRow(uint x, uint y, uint width)
                {
                    DecSquare(x, y, width, 1);
                }

                public void DecColumn(uint x, uint y, uint height)
                {
                    DecSquare(x, y, 1, height);
                }

                public bool EmptySquare(uint x, uint y, uint width, uint height)
                {
                    CheckDimensions(x, y, width, height);
                    uint yEnd = y + height;
                    for (uint j = y; j < yEnd; j++)
                    {
                        uint offset = j * this.width + x;
                        for (uint i = 0; i < width; i++)
                        {
                            if (this.positions[offset++] > 0)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }

                public bool EmptyRow(uint x, uint y, uint width)
                {
                    return EmptySquare(x, y, width, 1);
                }

                public bool EmptyColumn(uint x, uint y, uint height)
                {
                    return EmptySquare(x, y, 1, height);
                }

                public bool this[uint x, uint y]
                {
                    get
                    {
                        return this.positions[y * this.width + x] == 0;
                    }
                }
            }
        }
    }
}