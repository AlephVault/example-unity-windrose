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
             * A map strategy holder will reference its map and also will find its way to
             *   initialize its strategy. When initializing the strategy, it should provide
             *   itself to the strategy constructor (alongside any needed data).
             */
            [RequireComponent(typeof(Map))]
            public abstract class StrategyHolder : MonoBehaviour
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
                 * Each strategy holder knows its map.
                 */
                public Map Map { get; private set; }

                /**
                 * Each strategy holder knows its strategy.
                 */
                public Strategy Strategy { get; private set; }

                /**
                 * And also each strategy holder knows how to instantiate its strategy.
                 */
                protected abstract Strategy BuildStrategy();

                /**
                 * And which tilemaps does it have.
                 */
                public UnityEngine.Tilemaps.Tilemap[] tilemaps { get; private set; }

                /**
                 * On initialization, the strategy will fetch its map to, actually, know it.
                 * Also it will fetch the active tilemaps, and build its strategy.
                 */
                private void Awake()
                {
                    Map = GetComponent<Map>();
                    Strategy = BuildStrategy();
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
                 *   method from its own behaviour.
                 */
                public void Initialize()
                {
                    Strategy.Initialize();
                }

                /**
                 * Utility method to iterate over the map's tilemaps.
                 */
                public bool ForEachTilemap(Predicate<UnityEngine.Tilemaps.Tilemap> callback)
                {
                    foreach(UnityEngine.Tilemaps.Tilemap tilemap in tilemaps)
                    {
                        if (callback(tilemap)) return true;
                    }
                    return false;
                }

                /**
                 * Get a tile in one of the tilemaps.
                 */
                public UnityEngine.Tilemaps.TileBase GetTile(int tilemap, int x, int y)
                {
                    return tilemaps[tilemap].GetTile(new Vector3Int(x, y, 0));
                }

                /**
                 * Set a tile in one of the tilemaps. It will force a strategy recomputation.
                 */
                public void SetTile(int tilemap, uint x, uint y, UnityEngine.Tilemaps.TileBase tile)
                {
                    tilemaps[tilemap].SetTile(new Vector3Int((int)x, (int)y, 0), tile);
                    Strategy.ComputeCellData(x, y);
                }

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
                public Status StatusFor(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder)
                {
                    if (attachedStrategies.ContainsKey(objectStrategyHolder.ObjectStrategy))
                    {
                        return attachedStrategies[objectStrategyHolder.ObjectStrategy];
                    }
                    else
                    {
                        throw new NotAttachedException("The object is not attached to this map");
                    }
                }

                /**
                 * Attaches the object strategy to the current map strategy.
                 */
                public void Attach(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, uint x, uint y)
                {
                    // Require it not attached
                    RequireNotAttached(objectStrategyHolder.ObjectStrategy);

                    // Do we accept or reject the strategy being attached?
                    if (!Strategy.AcceptsObjectStrategy(objectStrategyHolder.ObjectStrategy))
                    {
                        throw new StrategyNowAllowedException("This strategy is not allowed on this map");
                    }

                    // Does it fit regarding bounds?
                    if (x > Map.Width - objectStrategyHolder.Positionable.Width || y > Map.Height - objectStrategyHolder.Positionable.Height)
                    {
                        throw new InvalidPositionException("Object coordinates and dimensions are not valid inside intended map's dimensions", x, y);
                    }

                    // Store its position
                    Status status = new Status(x, y);
                    attachedStrategies[objectStrategyHolder.ObjectStrategy] = status;

                    // Notify the map strategy, so data may be updated
                    Strategy.AttachedStratergy(objectStrategyHolder.ObjectStrategy, status);

                    // Finally, notify the client strategy.
                    objectStrategyHolder.ObjectStrategy.TriggerEvent("OnAttached", Map);
                }

                /**
                 * Detaches the object strategy from the current map strategy.
                 */
                public void Detach(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder)
                {
                    // Require it attached to the map
                    RequireAttached(objectStrategyHolder.ObjectStrategy);
                    Status status = attachedStrategies[objectStrategyHolder.ObjectStrategy];

                    // Cancels the movement, if any
                    Strategy.ClearMovement(objectStrategyHolder.ObjectStrategy, status);

                    // Notify the map strategy, so data may be cleaned
                    Strategy.DetachedStratergy(objectStrategyHolder.ObjectStrategy, status);

                    // Clear its position
                    attachedStrategies.Remove(objectStrategyHolder.ObjectStrategy);

                    // Finally, notify the client strategy.
                    objectStrategyHolder.ObjectStrategy.TriggerEvent("OnDetached", Map);
                }

                /*************************************************************************************************
                 * 
                 * Starting the movement of an object (strategy) in a direction and telling whether it is the
                 *   continuation of a former movement (this will happen in Movable component).
                 * 
                 *************************************************************************************************/

                public bool MovementStart(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, Types.Direction direction, bool continuated = false)
                {
                    // Require it attached to the map
                    RequireAttached(objectStrategyHolder.ObjectStrategy);

                    Status status = attachedStrategies[objectStrategyHolder.ObjectStrategy];

                    return Strategy.AllocateMovement(objectStrategyHolder.ObjectStrategy, status, direction, continuated);
                }

                /*************************************************************************************************
                 * 
                 * Cancelling the movement of an object (strategy).
                 * 
                 *************************************************************************************************/

                /**
                 * Cancels the movement in the current object.
                 */
                public bool MovementCancel(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder)
                {
                    // Require it attached to the map
                    RequireAttached(objectStrategyHolder.ObjectStrategy);

                    return Strategy.ClearMovement(objectStrategyHolder.ObjectStrategy, attachedStrategies[objectStrategyHolder.ObjectStrategy]);
                }

                /*************************************************************************************************
                 * 
                 * Finishing the movement of an object (strategy), if any.
                 * 
                 *************************************************************************************************/

                /**
                 * Cancels the movement in the current object.
                 */
                public bool MovementFinish(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder)
                {
                    // Require it attached to the map
                    RequireAttached(objectStrategyHolder.ObjectStrategy);

                    Status status = attachedStrategies[objectStrategyHolder.ObjectStrategy];

                    if (status.Movement != null)
                    {
                        Types.Direction formerMovement = status.Movement.Value;
                        Strategy.DoConfirmMovement(objectStrategyHolder.ObjectStrategy, status, formerMovement, "Before");
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
                        Strategy.DoConfirmMovement(objectStrategyHolder.ObjectStrategy, status, formerMovement, "AfterPositionChange");
                        status.Movement = null;
                        Strategy.DoConfirmMovement(objectStrategyHolder.ObjectStrategy, status, formerMovement, "AfterMovementClear");
                        objectStrategyHolder.ObjectStrategy.TriggerEvent("OnMovementFinished", formerMovement);
                        Strategy.DoConfirmMovement(objectStrategyHolder.ObjectStrategy, status, formerMovement, "After");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /*************************************************************************************************
                 * 
                 * Teleports the object strategy to another position in the map
                 * 
                 *************************************************************************************************/

                public void Teleport(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, uint x, uint y)
                {
                    RequireAttached(objectStrategyHolder.ObjectStrategy);

                    Status status = attachedStrategies[objectStrategyHolder.ObjectStrategy];

                    if (status.X > Map.Width - objectStrategyHolder.Positionable.Width || y > Map.Height - objectStrategyHolder.Positionable.Height)
                    {
                        throw new InvalidPositionException("New object coordinates and dimensions are not valid inside intended map's dimensions", status.X, status.Y);
                    }

                    Strategy.ClearMovement(objectStrategyHolder.ObjectStrategy, status);
                    Strategy.DoTeleport(objectStrategyHolder.ObjectStrategy, status, x, y, "Before");
                    status.X = x;
                    status.Y = y;
                    Strategy.DoTeleport(objectStrategyHolder.ObjectStrategy, status, x, y, "AfterPositionChange");
                    objectStrategyHolder.ObjectStrategy.TriggerEvent("OnTeleported", x, y);
                    Strategy.DoTeleport(objectStrategyHolder.ObjectStrategy, status, x, y, "After");
                }

                /*************************************************************************************************
                 * 
                 * Updates according to particular data change. These fields exist in the strategy.
                 * 
                 *************************************************************************************************/

                public void PropertyWasUpdated(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, string property, object oldValue, object newValue)
                {
                    RequireAttached(objectStrategyHolder.ObjectStrategy);

                    Strategy.DoProcessPropertyUpdate(objectStrategyHolder.ObjectStrategy, attachedStrategies[objectStrategyHolder.ObjectStrategy], property, oldValue, newValue);

                    objectStrategyHolder.ObjectStrategy.TriggerEvent("OnPropertyUpdated", property, oldValue, newValue);
                }
            }
        }
    }
}
