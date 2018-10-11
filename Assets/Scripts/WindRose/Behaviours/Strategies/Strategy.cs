using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            /**
             * A map strategy requires a map, but will not perform any particular event but, instead, provide
             *   functionality.
             * 
             * The Map behaviour will try to find a strategy (a subclass of this class) or die trying. When
             *   needed, the map will invoke stuff on this class.
             */
            [RequireComponent(typeof(Map))]
            public abstract class Strategy : MonoBehaviour
            {
                /**
                 * All the needed exceptions go here.
                 */
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
                    public AlreadyAttachedException(string message) : base(message) { }
                    public AlreadyAttachedException(string message, System.Exception inner) : base(message, inner) { }
                }

                public class NotAttachedException : Types.Exception
                {
                    public NotAttachedException(string message) : base(message) { }
                    public NotAttachedException(string message, System.Exception inner) : base(message, inner) { }
                }

                public class StrategyNowAllowedException : Types.Exception
                {
                    public StrategyNowAllowedException(string message) : base(message) { }
                    public StrategyNowAllowedException(string message, System.Exception inner) : base(message, inner) { }
                }

                /**
                 * Each strategy knows its map.
                 */
                public Map Map { get; private set; }

                /**
                 * AND each strategy will know on initialization which tilemaps will it account for.
                 */
                private UnityEngine.Tilemaps.Tilemap []tilemaps;

                /**
                 * On initialization, the strategy will fetch its map to, actually, know it.
                 * Also it will fetch the active tilemaps;
                 */
                private void Awake()
                {
                    Map = GetComponent<Map>();
                    List<UnityEngine.Tilemaps.Tilemap> tilemaps = new List<UnityEngine.Tilemaps.Tilemap>();
                    int childCount = transform.childCount;
                    for (int index = 0; index < childCount; index++)
                    {
                        GameObject go = transform.GetChild(index).gameObject;
                        UnityEngine.Tilemaps.Tilemap tilemap = go.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                        if (tilemap != null)
                        {
                            tilemaps.Add(tilemap);
                        }
                    }
                    this.tilemaps = tilemaps.ToArray();
                }

                /*************************************************************************************************
                 * 
                 * Initializing the strategy.
                 * 
                 *************************************************************************************************/

                /**
                 * Initializing a strategy will be done on map initialization. This means: the map calls this
                 *   method from its own behaviour. Each behaviour will be differently initialized, if it is
                 *   at all. Initializing may involve computing custom data for each cell.
                 */
                public void Initialize()
                {
                    InitGlobalCellData();
                    if (ComputesCellsData())
                    {
                        ComputeCellsData();
                    }
                }

                /**
                 * Initializing the cell data will require initializing masks and other arrays, in a per-strategy
                 *   basis.
                 */
                protected abstract void InitGlobalCellData();

                /**
                 * This method tells whether the strategy has cells to compute, or not.
                 */
                protected bool ComputesCellsData()
                {
                    return true;
                }

                /**
                 * This method updates all the cells in the strategy. It depends on the method to update a
                 *   single cell. This is an utility method that will frequently be used when
                 */
                private void ComputeCellsData()
                {
                    for (uint y = 0; y < Map.Height; y++)
                    {
                        for (uint x = 0; x < Map.Width; x++)
                        {
                            ComputeCellData(x, y);
                        }
                    }
                }

                /**
                 * Utility method to iterate over the map's tilemaps.
                 */
                protected bool ForEachTilemap(Predicate<UnityEngine.Tilemaps.Tilemap> callback)
                {
                    for(int i = 0; i < tilemaps.Length; i++)
                    {
                        if (callback(tilemaps[i])) return true;
                    }
                    return false;
                }

                /**
                 * Updating a strategy actually computes a single cell. It is intended to be run after a tilemap
                 *   was changed a single tile, or in per-cell basis on initialization.
                 */
                protected abstract void ComputeCellData(uint x, uint y);

                /*************************************************************************************************
                 * 
                 * Attaching an object (strategy).
                 * 
                 *************************************************************************************************/

                public class Status
                {
                    public Types.Direction? Movement;
                    public uint X;
                    public uint Y;

                    public Status(uint x, uint y, Types.Direction? movement = null)
                    {
                        X = x;
                        Y = y;
                        Movement = movement;
                    }

                    public Status Copy()
                    {
                        return new Status(X, Y, Movement);
                    }
                }

                private Dictionary<Objects.Strategies.ObjectStrategy, Status> attachedStrategies = new Dictionary<Objects.Strategies.ObjectStrategy, Status>();

                private void RequireAttached(Objects.Strategies.ObjectStrategy strategy)
                {
                    if (strategy == null)
                    {
                        throw new ArgumentNullException("Cannot attach a null object strategy to a map");
                    }

                    if (!attachedStrategies.ContainsKey(strategy))
                    {
                        throw new NotAttachedException("This strategy is not attached to the map");
                    }
                }

                private void RequireNotAttached(Objects.Strategies.ObjectStrategy strategy)
                {
                    if (attachedStrategies.ContainsKey(strategy))
                    {
                        throw new AlreadyAttachedException("This strategy is already attached to the map");
                    }
                }

                /**
                 * Gets the status of the object strategy as attached to the map.
                 */
                public Status StatusFor(Objects.Strategies.ObjectStrategy strategy)
                {
                    if (attachedStrategies.ContainsKey(strategy))
                    {
                        return attachedStrategies[strategy];
                    }
                    else
                    {
                        throw new NotAttachedException("The object is not attached to this map");
                    }
                }

                /**
                 * Attaches the object strategy to the current map strategy.
                 */
                public void Attach(Objects.Strategies.ObjectStrategy strategy, uint x, uint y)
                {
                    // Require it not attached
                    RequireNotAttached(strategy);

                    // Do we accept or reject the strategy being attached?
                    if (!AcceptsObjectStrategy(strategy))
                    {
                        throw new StrategyNowAllowedException("This strategy is not allowed on this map");
                    }

                    // Does it fit regarding bounds?
                    if (x > Map.Width - strategy.Positionable.Width || y > Map.Height - strategy.Positionable.Height)
                    {
                        throw new InvalidPositionException("Object coordinates and dimensions are not valid inside intended map's dimensions", x, y);
                    }

                    // Store its position
                    Status status = new Status(x, y);
                    attachedStrategies[strategy] = status;

                    // Notify the map strategy, so data may be updated
                    AttachedStratergy(strategy, status);

                    // Finally, notify the client strategy.
                    strategy.TriggerEvent("OnAttached", Map);
                }

                /**
                 * Object strategy allowance. It will tell whether it accepts, or not, a certain strategy to be
                 */
                protected abstract bool AcceptsObjectStrategy(Objects.Strategies.ObjectStrategy strategy);

                /**
                 * Object strategy was added. Compute relevant data here.
                 */
                protected abstract void AttachedStratergy(Objects.Strategies.ObjectStrategy strategy, Status status);

                /**
                 * Detaches the object strategy from the current map strategy.
                 */
                public void Detach(Objects.Strategies.ObjectStrategy strategy)
                {
                    // Require it attached to the map
                    RequireAttached(strategy);
                    Status status = attachedStrategies[strategy];

                    // Cancels the movement, if any
                    ClearMovement(strategy, status);

                    // Notify the map strategy, so data may be cleaned
                    DetachedStratergy(strategy, status);

                    // Clear its position
                    attachedStrategies.Remove(strategy);

                    // Finally, notify the client strategy.
                    strategy.TriggerEvent("OnDetached", Map);
                }

                /**
                 * Object strategy is to be removed. Compute relevant data here.
                 */
                protected abstract void DetachedStratergy(Objects.Strategies.ObjectStrategy strategy, Status status);

                /*************************************************************************************************
                 * 
                 * Starting the movement of an object (strategy) in a direction and telling whether it is the
                 *   continuation of a former movement (this will happen in Movable component).
                 * 
                 *************************************************************************************************/

                public bool MovementStart(Objects.Strategies.ObjectStrategy strategy, Types.Direction direction, bool continuated = false)
                {
                    // Require it attached to the map
                    RequireAttached(strategy);

                    Status status = attachedStrategies[strategy];

                    return AllocateMovement(strategy, status, direction, continuated);
                }

                /**
                 * Executes the actual movement allocation.
                 */
                protected bool AllocateMovement(Objects.Strategies.ObjectStrategy strategy, Status status, Types.Direction direction, bool continuated = false)
                {
                    if (CanAllocateMovement(strategy, status, direction, continuated))
                    {
                        DoAllocateMovement(strategy, status, direction, continuated, "Before");
                        status.Movement = direction;
                        DoAllocateMovement(strategy, status, direction, continuated, "AfterMovementAllocation");
                        strategy.TriggerEvent("OnMovementStarted", direction);
                        DoAllocateMovement(strategy, status, direction, continuated, "After");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /**
                 * Tells whether movement can be allocated.
                 */
                protected abstract bool CanAllocateMovement(Objects.Strategies.ObjectStrategy strategy, Status status, Types.Direction direction, bool continuated);

                /**
                 * Performs a dumb bounds-check on the object's position, size, and current map.
                 */
                protected bool IsHittingEdge(Objects.Positionable positionable, Status status, Types.Direction direction)
                {
                    switch (direction)
                    {
                        case Types.Direction.LEFT:
                            return status.X == 0;
                        case Types.Direction.UP:
                            return status.Y + positionable.Height == Map.Height;
                        case Types.Direction.RIGHT:
                            return status.X + positionable.Width == Map.Width;
                        case Types.Direction.DOWN:
                            return status.Y == 0;
                    }
                    return false;
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementAllocation", "After".
                 */
                protected abstract void DoAllocateMovement(Objects.Strategies.ObjectStrategy strategy, Status status, Types.Direction direction, bool continuated, string stage);

                /*************************************************************************************************
                 * 
                 * Cancelling the movement of an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Cancels the movement in the current object.
                 */
                public bool MovementCancel(Objects.Strategies.ObjectStrategy strategy)
                {
                    // Require it attached to the map
                    RequireAttached(strategy);

                    return ClearMovement(strategy, attachedStrategies[strategy]);
                }

                /**
                 * Override it to use a different criteria for allowing movement cancel.
                 */
                protected virtual bool CanClearMovement(Objects.Strategies.ObjectStrategy strategy, Status status)
                {
                    return status.Movement != null;
                }

                /**
                 * Executes the actual movement clearing.
                 */
                protected bool ClearMovement(Objects.Strategies.ObjectStrategy strategy, Status status)
                {
                    if (CanClearMovement(strategy, status))
                    {
                        Types.Direction? formerMovement = status.Movement;
                        DoClearMovement(strategy, status, formerMovement, "Before");
                        status.Movement = null;
                        DoClearMovement(strategy, status, formerMovement, "AfterMovementClear");
                        strategy.TriggerEvent("OnMovementCancelled", formerMovement);
                        DoClearMovement(strategy, status, formerMovement, "Before");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterMovementClear", "After".
                 */
                protected abstract void DoClearMovement(Objects.Strategies.ObjectStrategy strategy, Status status, Types.Direction? formerMovement, string stage);

                /*************************************************************************************************
                 * 
                 * Finishing the movement of an object (strategy), if any.
                 * 
                 *************************************************************************************************/

                /**
                 * Cancels the movement in the current object.
                 */
                public bool MovementFinish(Objects.Strategies.ObjectStrategy strategy)
                {
                    // Require it attached to the map
                    RequireAttached(strategy);

                    Status status = attachedStrategies[strategy];

                    if (status.Movement != null)
                    {
                        Types.Direction formerMovement = status.Movement.Value;
                        DoConfirmMovement(strategy, status, formerMovement, "Before");
                        switch (formerMovement)
                        {
                            case Types.Direction.UP:
                                Debug.Log("Y++ due to up");
                                status.Y++;
                                break;
                            case Types.Direction.DOWN:
                                Debug.Log("X-- due to down");
                                status.Y--;
                                break;
                            case Types.Direction.LEFT:
                                Debug.Log("X-- due to left");
                                status.X--;
                                break;
                            case Types.Direction.RIGHT:
                                Debug.Log("X++ due to right");
                                status.X++;
                                break;
                        }
                        DoConfirmMovement(strategy, status, formerMovement, "AfterPositionChange");
                        status.Movement = null;
                        DoConfirmMovement(strategy, status, formerMovement, "AfterMovementClear");
                        strategy.TriggerEvent("OnMovementFinished", formerMovement);
                        DoConfirmMovement(strategy, status, formerMovement, "After");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /**
                 * You have to define this method. Ask conditionally for "Before", "AfterPositionChange", "AfterMovementClear", "After".
                 */
                protected abstract void DoConfirmMovement(Objects.Strategies.ObjectStrategy strategy, Status status, Types.Direction? formerMovement, string stage);

                /*************************************************************************************************
                 * 
                 * Teleports the object strategy to another position in the map
                 * 
                 *************************************************************************************************/

                public void Teleport(Objects.Strategies.ObjectStrategy strategy, uint x, uint y)
                {
                    RequireAttached(strategy);

                    Status status = attachedStrategies[strategy];

                    if (status.X > Map.Width - strategy.Positionable.Width || y > Map.Height - strategy.Positionable.Height)
                    {
                        throw new InvalidPositionException("New object coordinates and dimensions are not valid inside intended map's dimensions", status.X, status.Y);
                    }

                    ClearMovement(strategy, status);

                    DoAroundTeleport(strategy, status, x, y, "Before");
                    status.X = x;
                    status.Y = y;
                    DoAroundTeleport(strategy, status, x, y, "AfterPositionChange");
                    strategy.TriggerEvent("OnTeleported", x, y);
                    DoAroundTeleport(strategy, status, x, y, "After");
                }

                /**
                 * You have to define this method. Ask conditionally for stages "Before", "AfterPositionChange", "After".
                 */
                protected abstract void DoAroundTeleport(Objects.Strategies.ObjectStrategy strategy, Status status, uint x, uint y, string stage);

                /*************************************************************************************************
                 * 
                 * Updates according to particular data change. These fields exist in the strategy.
                 * 
                 *************************************************************************************************/

                public void PropertyWasUpdated(Objects.Strategies.ObjectStrategy strategy, string property, object oldValue, object newValue)
                {
                    RequireAttached(strategy);

                    DoProcessPropertyUpdate(strategy, attachedStrategies[strategy], property, oldValue, newValue);

                    strategy.TriggerEvent("OnPropertyUpdated", property, oldValue, newValue);
                }

                protected abstract void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy strategy, Status status, string property, object oldValue, object newValue);
            }
        }
    }
}
