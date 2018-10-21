using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Strategies
            {
                /**
                 * A map strategy holder will reference its map and also will find its way to
                 *   initialize its strategy. When initializing the strategy, it should provide
                 *   itself to the strategy constructor (alongside any needed data).
                 */
                [RequireComponent(typeof(Map))]
                public class StrategyHolder : MonoBehaviour
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

                    public class InvalidStrategyComponentException : Types.Exception
                    {
                        public InvalidStrategyComponentException() { }
                        public InvalidStrategyComponentException(string message) : base(message) { }
                        public InvalidStrategyComponentException(string message, Exception inner) : base(message, inner) { }
                    }

                    public class ObjectLacksOfCompatibleStrategy : Types.Exception
                    {
                        public ObjectLacksOfCompatibleStrategy(string message) : base(message) { }
                        public ObjectLacksOfCompatibleStrategy(string message, System.Exception inner) : base(message, inner) { }
                    }

                    public class DuplicatedComponentException : Types.Exception
                    {
                        public DuplicatedComponentException(string message) : base(message) { }
                        public DuplicatedComponentException(string message, System.Exception inner) : base(message, inner) { }
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
                     * The root strategy that can be picked in the editor.
                     */
                    [SerializeField]
                    private Strategy strategy;

                    /**
                     * Each strategy holder tells its strategy.
                     */
                    public Strategy Strategy { get { return strategy; } }

                    /**
                     * And which tilemaps does it have.
                     */
                    public UnityEngine.Tilemaps.Tilemap[] tilemaps { get; private set; }

                    /**
                     * This is the list of sorted strategy componentes here.
                     */
                    private Strategy[] sortedStrategies;

                    /**
                     * On initialization, the strategy will fetch its map to, actually, know it.
                     * Also it will fetch the active tilemaps, and build its strategy.
                     */
                    private void Awake()
                    {
                        Map = GetComponent<Map>();
                        if (strategy == null || !(new HashSet<Strategy>(GetComponents<Strategy>()).Contains(strategy)))
                        {
                            Destroy(gameObject);
                            throw new InvalidStrategyComponentException("The selected strategy component must be non-null and present among the current map's components");
                        }
                        // We enumerate all the strategies attached. We will iterate their calls and cache their results, if any.
                        sortedStrategies = (from component in Support.Utils.Layout.SortByDependencies(GetComponents<Strategy>()) select (component as Strategy)).ToArray();

                        // We cannot allow a strategy type being added (depended) twice.
                        if (sortedStrategies.Length != new HashSet<Strategy>(sortedStrategies).Count)
                        {
                            Destroy(gameObject);
                            throw new DuplicatedComponentException("Cannot add the same strategy component more than one time per component type to a strategy");
                        }

                        // Initializing tilemaps.
                        PrepareTilemaps();
                    }

                    /**
                     * Iterates and collects the same boolean call to each strategy into a dictionary. Returns the
                     *   value according to the main strategy.
                     */
                    private bool Collect(Func<Dictionary<Type, bool>, Strategy, bool> predicate)
                    {
                        Dictionary<Type, bool> collected = new Dictionary<Type, bool>();
                        foreach (Strategy subStrategy in sortedStrategies)
                        {
                            collected[subStrategy.GetType()] = predicate(collected, subStrategy);
                        }
                        return collected[Strategy.GetType()];
                    }

                    /**
                     * Iterates on each strategy and calls a function.
                     */
                    private void Traverse(Action<Strategy> traverser)
                    {
                        foreach (Strategy subStrategy in sortedStrategies)
                        {
                            traverser(subStrategy);
                        }
                    }

                    /**
                     * Given a particular strategy component, obtain the appropriate objectStrategy component from a main object
                     *   strategy.
                     */
                    private Objects.Strategies.ObjectStrategy GetCompatible(Objects.Strategies.ObjectStrategy target, Strategy source)
                    {
                        return target.GetComponent(source.CounterpartType) as Objects.Strategies.ObjectStrategy;
                    }

                    /**
                     * Gets the main strategy of the target holder according to our main strategy.
                     */
                    private Objects.Strategies.ObjectStrategy GetMainCompatible(Objects.Strategies.ObjectStrategyHolder target)
                    {
                        Objects.Strategies.ObjectStrategy objectStrategy = target.GetComponent(strategy.CounterpartType) as Objects.Strategies.ObjectStrategy;
                        if (objectStrategy == null)
                        {
                            throw new ObjectLacksOfCompatibleStrategy("Related object strategy holder component lacks of compatible strategy component for the current map strategy");
                        }
                        return objectStrategy;
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
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.InitGlobalCellsData();
                            strategy.InitIndividualCellsData(delegate (Action<uint, uint> callback)
                            {
                                for (uint y = 0; y < Map.Height; y++)
                                {
                                    for (uint x = 0; x < Map.Width; x++)
                                    {
                                        callback(x, y);
                                    }
                                }
                            });
                        });
                    }

                    /**
                     * Method to initialize the tilemaps.
                     */
                    private void PrepareTilemaps()
                    {
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

                    /**
                     * Utility method to iterate over the map's tilemaps.
                     */
                    public bool ForEachTilemap(Predicate<UnityEngine.Tilemaps.Tilemap> callback)
                    {
                        foreach (UnityEngine.Tilemaps.Tilemap tilemap in tilemaps)
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
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);
                        if (attachedStrategies.ContainsKey(objectStrategy))
                        {
                            return attachedStrategies[objectStrategy];
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
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        // Require it not attached
                        RequireNotAttached(objectStrategy);

                        // Do we accept or reject the strategy being attached? (no per-strategy-component call is needed here)
                        if (objectStrategy.GetType().IsSubclassOf(Strategy.CounterpartType))
                        {
                            throw new StrategyNowAllowedException("This strategy is not allowed on this map because is not a valid counterpart of the current map strategy.");
                        }

                        // Does it fit regarding bounds?
                        if (x > Map.Width - objectStrategyHolder.Positionable.Width || y > Map.Height - objectStrategyHolder.Positionable.Height)
                        {
                            throw new InvalidPositionException("Object coordinates and dimensions are not valid inside intended map's dimensions", x, y);
                        }

                        // Store its position
                        Status status = new Status(x, y);
                        attachedStrategies[objectStrategy] = status;

                        // Notify the map strategy, so data may be updated
                        AttachedStrategy(objectStrategy, status);

                        // Finally, notify the client strategy.
                        objectStrategy.TriggerEvent("OnAttached", Map);
                    }

                    /**
                     * Iterates over each strategy and calls its AttachedStrategy appropriately
                     *   (from less to more dependent strategies).
                     */
                    private void AttachedStrategy(Objects.Strategies.ObjectStrategy objectStrategy, Status status)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.AttachedStrategy(GetCompatible(objectStrategy, strategy), status);
                        });
                    }

                    /**
                     * Detaches the object strategy from the current map strategy.
                     */
                    public void Detach(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder)
                    {
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        // Require it attached to the map
                        RequireAttached(objectStrategy);
                        Status status = attachedStrategies[objectStrategy];

                        // Cancels the movement, if any
                        ClearMovement(objectStrategy, status);

                        // Notify the map strategy, so data may be cleaned
                        DetachedStrategy(objectStrategy, status);

                        // Clear its position
                        attachedStrategies.Remove(objectStrategy);

                        // Finally, notify the client strategy.
                        objectStrategy.TriggerEvent("OnDetached", Map);
                    }

                    /**
                     * Iterates over each strategy and calls its DetachedStrategy appropriately
                     *   (from less to more dependent strategies).
                     */
                    private void DetachedStrategy(Objects.Strategies.ObjectStrategy objectStrategy, Status status)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.DetachedStrategy(GetCompatible(objectStrategy, strategy), status);
                        });
                    }

                    /*************************************************************************************************
                     * 
                     * Starting the movement of an object (strategy) in a direction and telling whether it is the
                     *   continuation of a former movement (this will happen in Movable component).
                     * 
                     *************************************************************************************************/

                    public bool MovementStart(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, Types.Direction direction, bool continuated = false)
                    {
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        // Require it attached to the map
                        RequireAttached(objectStrategy);

                        Status status = attachedStrategies[objectStrategy];

                        return AllocateMovement(objectStrategy, status, direction, continuated);
                    }

                    /**
                     * Executes the actual movement allocation.
                     */
                    private bool AllocateMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status, Types.Direction direction, bool continuated = false)
                    {
                        if (CanAllocateMovement(objectStrategy, status, direction, continuated))
                        {
                            DoAllocateMovement(objectStrategy, status, direction, continuated, "Before");
                            status.Movement = direction;
                            DoAllocateMovement(objectStrategy, status, direction, continuated, "AfterMovementAllocation");
                            objectStrategy.TriggerEvent("OnMovementStarted", direction);
                            DoAllocateMovement(objectStrategy, status, direction, continuated, "After");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    /**
                     * Iterates all the strategies to tell whether it can allocate the movement or not.
                     */
                    private bool CanAllocateMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status, Types.Direction direction, bool continuated = false)
                    {
                        return Collect(delegate (Dictionary<Type, bool> collected, Strategy strategy)
                        {
                            return strategy.CanAllocateMovement(collected, GetCompatible(objectStrategy, strategy), status, direction, continuated);
                        });
                    }

                    /**
                     * Iterates all the strategies for the different stages of movement allocation.
                     */
                    private void DoAllocateMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status, Types.Direction direction, bool continuated, string stage)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.DoAllocateMovement(GetCompatible(objectStrategy, strategy), status, direction, continuated, stage);
                        });
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
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        // Require it attached to the map
                        RequireAttached(objectStrategy);

                        return ClearMovement(objectStrategy, attachedStrategies[objectStrategy]);
                    }

                    /**
                     * Executes the actual movement clearing.
                     */
                    private bool ClearMovement(Objects.Strategies.ObjectStrategy strategy, Status status)
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
                     * Iterates all the strategies to tell whether it can clear the movement or not.
                     */
                    private bool CanClearMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status)
                    {
                        return Collect(delegate (Dictionary<Type, bool> collected, Strategy strategy)
                        {
                            return strategy.CanClearMovement(collected, GetCompatible(objectStrategy, strategy), status);
                        });
                    }

                    /**
                     * Iterates all the strategies for the different stages of movement clearing.
                     */
                    private void DoClearMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status, Types.Direction? formerMovement, string stage)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.DoClearMovement(GetCompatible(objectStrategy, strategy), status, formerMovement, stage);
                        });
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
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        // Require it attached to the map
                        RequireAttached(objectStrategy);

                        Status status = attachedStrategies[objectStrategy];

                        if (status.Movement != null)
                        {
                            Types.Direction formerMovement = status.Movement.Value;
                            Strategy.DoConfirmMovement(objectStrategy, status, formerMovement, "Before");
                            switch (formerMovement)
                            {
                                case Types.Direction.UP:
                                    status.Y++;
                                    break;
                                case Types.Direction.DOWN:
                                    status.Y--;
                                    break;
                                case Types.Direction.LEFT:
                                    status.X--;
                                    break;
                                case Types.Direction.RIGHT:
                                    status.X++;
                                    break;
                            }
                            DoConfirmMovement(objectStrategy, status, formerMovement, "AfterPositionChange");
                            status.Movement = null;
                            DoConfirmMovement(objectStrategy, status, formerMovement, "AfterMovementClear");
                            objectStrategy.TriggerEvent("OnMovementFinished", formerMovement);
                            DoConfirmMovement(objectStrategy, status, formerMovement, "After");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    /**
                     * Iterates all the strategies for the different stages of movement allocation.
                     */
                    private void DoConfirmMovement(Objects.Strategies.ObjectStrategy objectStrategy, Status status, Types.Direction? formerMovement, string stage)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.DoConfirmMovement(GetCompatible(objectStrategy, strategy), status, formerMovement, stage);
                        });
                    }

                    /*************************************************************************************************
                     * 
                     * Teleports the object strategy to another position in the map
                     * 
                     *************************************************************************************************/

                    public void Teleport(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, uint x, uint y)
                    {
                        Objects.Strategies.ObjectStrategy objectStrategy = GetMainCompatible(objectStrategyHolder);

                        RequireAttached(objectStrategy);

                        Status status = attachedStrategies[objectStrategy];

                        if (status.X > Map.Width - objectStrategyHolder.Positionable.Width || y > Map.Height - objectStrategyHolder.Positionable.Height)
                        {
                            throw new InvalidPositionException("New object coordinates and dimensions are not valid inside intended map's dimensions", status.X, status.Y);
                        }

                        ClearMovement(objectStrategy, status);
                        DoTeleport(objectStrategy, status, x, y, "Before");
                        status.X = x;
                        status.Y = y;
                        DoTeleport(objectStrategy, status, x, y, "AfterPositionChange");
                        objectStrategy.TriggerEvent("OnTeleported", x, y);
                        DoTeleport(objectStrategy, status, x, y, "After");
                    }

                    /**
                     * Iterates all the strategies for the different stages of teleportation.
                     */
                    private void DoTeleport(Objects.Strategies.ObjectStrategy objectStrategy, Status status, uint x, uint y, string stage)
                    {
                        Traverse(delegate (Strategy strategy)
                        {
                            strategy.DoTeleport(GetCompatible(objectStrategy, strategy), status, x, y, stage);
                        });
                    }

                    /*************************************************************************************************
                     * 
                     * Updates according to particular data change. These fields exist in the strategy. This method
                     *   will get the holder, the strategy being updated (which belongs to the holder), and the
                     *   property with the old/new values.
                     * 
                     * You will never call this method directly.
                     * 
                     * The strategy processing this data change will be picked according to the counterpart setting.
                     * It does not, and will not (most times in combined strategies) match the current map strategy
                     *   but instead map a strategy component in this same object.
                     * 
                     *************************************************************************************************/

                    public void PropertyWasUpdated(Objects.Strategies.ObjectStrategyHolder objectStrategyHolder, Objects.Strategies.ObjectStrategy objectStrategy, string property, object oldValue, object newValue)
                    {
                        Objects.Strategies.ObjectStrategy mainObjectStrategy = GetMainCompatible(objectStrategyHolder);

                        RequireAttached(mainObjectStrategy);

                        (GetComponent(objectStrategy.CounterpartType) as Strategy).DoProcessPropertyUpdate(mainObjectStrategy, attachedStrategies[mainObjectStrategy], property, oldValue, newValue);

                        mainObjectStrategy.TriggerEvent("OnPropertyUpdated", property, oldValue, newValue);
                    }
                }
            }
        }
    }
}
