using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Types
    {
        namespace Tilemaps
        {
            public class Tilemap
            {
                public const uint MAX_WIDTH = 100;
                public const uint MAX_HEIGHT = 100;

                public class InvalidDimensionsException : Types.Exception
                {
                    public readonly uint Width;
                    public readonly uint Height;
                    public InvalidDimensionsException(uint width, uint height) { Width = width; Height = height; }
                    public InvalidDimensionsException(string message, uint width, uint height) : base(message) { Width = width; Height = height; }
                    public InvalidDimensionsException(string message, uint width, uint height, System.Exception inner) : base(message, inner) { Width = width; Height = height; }
                }

                public class InvalidPositionException : Types.Exception
                {
                    public readonly uint X;
                    public readonly uint Y;
                    public InvalidPositionException(uint x, uint y) { X = x; Y = y; }
                    public InvalidPositionException(string message, uint x, uint y) : base(message) { X = x; Y = y; }
                    public InvalidPositionException(string message, uint x, uint y, System.Exception inner) : base(message, inner) { X = x; Y = y; }
                }

                public class AlreadyAttachedException : Types.Exception
                {
                    public AlreadyAttachedException(string message) : base(message) {}
                    public AlreadyAttachedException(string message, System.Exception inner) : base(message, inner) {}
                }

                public class NotAttachedException : Types.Exception
                {
                    public NotAttachedException(string message) : base(message) { }
                    public NotAttachedException(string message, System.Exception inner) : base(message, inner) { }
                }

                private BlockMask blockMask;
                private SolidMask solidMask;
                public readonly uint Width;
                public readonly uint Height;

                public class TilemapObject
                {
                    public Tilemap Map { get; private set; }
                    public uint X { get; private set; }
                    public uint Y { get; private set; }
                    public uint Xf { get { return X + Width - 1; } }
                    public uint Yf { get { return Y + Height - 1; } }
                    public readonly Behaviors.Positionable RelatedComponent;
                    public readonly uint Width;
                    public readonly uint Height;
                    public Direction? Movement { get; private set; }
                    public SolidnessStatus Solidness { get; private set; }

                    private static bool Occupies(SolidnessStatus solidness)
                    {
                        // Ghost objects will not occupy space
                        return solidness != SolidnessStatus.Ghost;
                    }

                    private static bool Traverses(SolidnessStatus solidness)
                    {
                        // Solid objects cannot traverse/overlap others.
                        return solidness != SolidnessStatus.Solid;
                    }

                    private static Direction? Opposite(Direction? direction)
                    {
                        switch (direction)
                        {
                            case Direction.LEFT:
                                return Direction.RIGHT;
                            case Direction.UP:
                                return Direction.DOWN;
                            case Direction.RIGHT:
                                return Direction.LEFT;
                            case Direction.DOWN:
                                return Direction.UP;
                            default:
                                return null;
                        }
                    }

                    private static bool OccupianceChanges(SolidnessStatus oldStatus, SolidnessStatus newStatus)
                    {
                        return Occupies(oldStatus) ^ Occupies(newStatus);
                    }

                    private void IncrementBody()
                    {
                        if (Occupies(Solidness)) Map.IncrementBody(X, Y, Width, Height);
                    }

                    private void DecrementBody()
                    {
                        if (Occupies(Solidness)) Map.DecrementBody(X, Y, Width, Height);
                    }

                    private void IncrementAdjacent()
                    {
                        if (Occupies(Solidness)) Map.IncrementAdjacent(X, Y, Width, Height, Movement);
                    }

                    private void DecrementAdjacent()
                    {
                        if (Occupies(Solidness)) Map.DecrementAdjacent(X, Y, Width, Height, Movement);
                    }

                    private void DecrementOppositeAdjacent()
                    {
                        if (Occupies(Solidness)) Map.DecrementAdjacent(X, Y, Width, Height, Opposite(Movement));
                    }

                    private void TriggerEvent(string targetEvent, params System.Object[] args)
                    {
                        RelatedComponent.SendMessage(targetEvent, args, SendMessageOptions.DontRequireReceiver);
                    }

                    private bool CanMoveTo(Direction direction)
                    {
                        if (Map.IsHittingEdge(X, Y, Width, Height, direction)) return false;
                        if (Map.IsAdjacencyBlocked(X, Y, Width, Height, direction)) return false;
                        return Traverses(Solidness) || Map.IsAdjacencyFree(X, Y, Width, Height, direction);
                    }

                    public TilemapObject(Behaviors.Positionable relatedComponent, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
                    {
                        if (relatedComponent == null)
                        {
                            throw new NullReferenceException("Related component for tile object must not be null");
                        }

                        if (width < 1 || width > MAX_WIDTH || height < 1 || height > MAX_HEIGHT)
                        {
                            throw new InvalidDimensionsException(width, height);
                        }

                        if (x + width >= MAX_WIDTH || y + height >= MAX_HEIGHT)
                        {
                            throw new InvalidPositionException(x, y);
                        }

                        X = x;
                        Y = y;
                        RelatedComponent = relatedComponent;
                        Width = width;
                        Height = height;
                        Solidness = solidness;
                        Movement = null;
                        Map = null;
                    }

                    public void Attach(Tilemap map, uint? x = null, uint? y = null)
                    {
                        if (Map != null) { throw new AlreadyAttachedException("This TilemapObject is already attached to a map"); }
                        if (map == null) { throw new ArgumentNullException("The specified map to attach to cannot be null"); }
                        if (x != null) { X = x.Value; }
                        if (y != null) { Y = y.Value; }
                        if (X > map.Width - Width || Y > map.Height - Height)
                        {
                            throw new InvalidPositionException("Object coordinates and dimensions are not valid inside intended map's dimensions", X, Y);
                        }
                        Map = map;
                        IncrementBody();
                        TriggerEvent("OnAttached", Map);
                    }

                    public void Detach()
                    {
                        if (Map != null)
                        {
                            CancelMovement();
                            DecrementBody();
                            Map = null;
                            TriggerEvent("OnDetached");
                        }
                    }

                    public bool IsAttached()
                    {
                        return Map != null;
                    }

                    public bool StartMovement(Direction direction)
                    {
                        if (Map == null) return false;
                        if (Movement == null && CanMoveTo(direction))
                        {
                            Movement = direction;
                            IncrementAdjacent();
                            TriggerEvent("OnMovementStarted", Movement);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    public bool CancelMovement()
                    {
                        if (Map == null) return false;
                        if (Movement != null)
                        {
                            DecrementAdjacent();
                            Direction formerMovement = Movement.Value;
                            Movement = null;
                            TriggerEvent("OnMovementCancelled", formerMovement);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    public bool FinishMovement()
                    {
                        if (Map == null) return false;
                        if (Movement != null)
                        {
                            switch (Movement)
                            {
                                case Direction.UP:
                                    Y--;
                                    break;
                                case Direction.DOWN:
                                    Y++;
                                    break;
                                case Direction.LEFT:
                                    X--;
                                    break;
                                case Direction.RIGHT:
                                    X++;
                                    break;
                            }
                            DecrementOppositeAdjacent();
                            Direction formerMovement = Movement.Value;
                            Movement = null;
                            TriggerEvent("OnMovementFinished", formerMovement);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    public void SetSolidness(SolidnessStatus newSolidness)
                    {
                        if (Map == null) return;
                        if (OccupianceChanges(Solidness, newSolidness))
                        {
                            CancelMovement();
                            DecrementBody();
                            Solidness = newSolidness;
                            IncrementBody();
                        }
                        else
                        {
                            Solidness = newSolidness;
                        }
                        TriggerEvent("OnSolidnessChanged", Solidness);
                    }

                    public void Teleport(uint? x, uint? y)
                    {
                        if (Map == null) return;
                        if (x == null) { x = X; }
                        if (y == null) { y = Y; }
                        if (x == X && y == Y) return;
                        if (x > Map.Width - Width || y > Map.Height - Height)
                        {
                            throw new InvalidPositionException("New object coordinates and dimensions are not valid inside intended map's dimensions", X, Y);
                        }
                        CancelMovement();
                        DecrementBody();
                        X = x.Value;
                        Y = y.Value;
                        IncrementBody();
                        TriggerEvent("OnTeleported", X, Y);
                    }
                }

                // Constructor

                /**
                 * Constructing a map involves constructing the internal objects layer and the internal block layer.
                 * The block mask will be immutable, since it will never be modified. OTOH the solid mask (for the objects layer)
                 *   will be mutable. But those layers are initialized inside the tilemap.
                 */
                public Tilemap(uint width, uint height, string blockMaskPattern, char freeMarkingChar = '0', char blockMarkingChar = '1',
                               uint maskApplicationOffsetX = 0, uint maskApplicationOffsetY = 0)
                {
                    if (width < 1 || width > MAX_WIDTH || height < 1 || height > MAX_HEIGHT)
                    {
                        throw new InvalidDimensionsException(width, height);
                    }

                    this.Width = width;
                    this.Height = height;
                    this.solidMask = new SolidMask(width, height);
                    string[] paddedBlockMaskPattern = BlockMask.PadMask(
                        blockMaskPattern, width, height, freeMarkingChar, blockMarkingChar, maskApplicationOffsetX, maskApplicationOffsetY
                    );
                    this.blockMask = new BlockMask(width, height, paddedBlockMaskPattern, freeMarkingChar, blockMarkingChar);
                }

                // Metodos privados para incrementar contadores de solidez

                private void IncrementBody(uint x, uint y, uint width, uint height)
                {
                    solidMask.IncSquare(x, y, width, height);
                }

                private void DecrementBody(uint x, uint y, uint width, uint height)
                {
                    solidMask.DecSquare(x, y, width, height);
                }

                private bool IsHittingEdge(uint x, uint y, uint width, uint height, Direction? direction)
                {
                    switch (direction)
                    {
                        case Direction.LEFT:
                            return x == 0;
                        case Direction.UP:
                            return y == 0;
                        case Direction.RIGHT:
                            return x + width == solidMask.width;
                        case Direction.DOWN:
                            return y + height == solidMask.height;
                    }
                    return false;
                }

                private bool IsAdjacencyBlocked(uint x, uint y, uint width, uint height, Direction? direction)
                {
                    /** Precondition: IsHittingEdge was already called to this point */
                    switch (direction)
                    {
                        case Direction.LEFT:
                            return blockMask.GetColumn(x - 1, y, y + height - 1, BlockMask.CheckType.ANY_BLOCKED);
                        case Direction.UP:
                            return blockMask.GetRow(x, x + width - 1, y - 1, BlockMask.CheckType.ANY_BLOCKED);
                        case Direction.RIGHT:
                            return blockMask.GetColumn(x + width, y, y + height - 1, BlockMask.CheckType.ANY_BLOCKED);
                        case Direction.DOWN:
                            return blockMask.GetRow(x, x + width - 1, y + height, BlockMask.CheckType.ANY_BLOCKED);
                        default:
                            return true;
                    }
                }

                private bool IsAdjacencyFree(uint x, uint y, uint width, uint height, Direction? direction)
                {
                    /** Precondition: IsHittingEdge was already called to this point */
                    switch (direction)
                    {
                        case Direction.LEFT:
                            return solidMask.EmptyColumn(x - 1, y, height);
                        case Direction.UP:
                            return solidMask.EmptyRow(x, y - 1, width);
                        case Direction.RIGHT:
                            return solidMask.EmptyColumn(x + width, y, height);
                        case Direction.DOWN:
                            return solidMask.EmptyRow(x, y + height, width);
                        default:
                            return true;
                    }
                }

                private void IncrementAdjacent(uint x, uint y, uint width, uint height, Direction? direction)
                {
                    if (!IsHittingEdge(x, y, width, height, direction))
                    {
                        switch (direction)
                        {
                            case Direction.LEFT:
                                solidMask.IncColumn(x - 1, y, height);
                                break;
                            case Direction.UP:
                                solidMask.IncRow(x, y - 1, width);
                                break;
                            case Direction.RIGHT:
                                solidMask.IncColumn(x + width, y, height);
                                break;
                            case Direction.DOWN:
                                solidMask.IncRow(x, y + height, width);
                                break;
                        }
                    }
                }

                private void DecrementAdjacent(uint x, uint y, uint width, uint height, Direction? direction)
                {
                    if (!IsHittingEdge(x, y, width, height, direction))
                    {
                        switch (direction)
                        {
                            case Direction.LEFT:
                                solidMask.DecColumn(x - 1, y, height);
                                break;
                            case Direction.UP:
                                solidMask.DecRow(x, y - 1, width);
                                break;
                            case Direction.RIGHT:
                                solidMask.DecColumn(x + width, y, height);
                                break;
                            case Direction.DOWN:
                                solidMask.DecRow(x, y + height, width);
                                break;
                        }
                    }
                }
            }
        }
    }
}
