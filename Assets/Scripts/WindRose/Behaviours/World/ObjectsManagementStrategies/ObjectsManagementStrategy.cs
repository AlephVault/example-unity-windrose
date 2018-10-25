using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                /**
                 * This is an objects management strategy. It will know:
                 * - The strategy-holder it is tied to (directly or indirectly).
                 * And it will have as behaviour:
                 * - List of all methods invoked by the strategy holder that are
                 *   related to the current logic.
                 * This behaviour will be a Unity Behaviour, so it will be attached
                 *   to the same object of the StrategyHolder.
                 * It is related to a counterpart type which is a subtype of object
                 *   strategy.
                 */
                public abstract class ObjectsManagementStrategy : MonoBehaviour
                {
                    private static Type baseCounterpartStrategyType = typeof(Objects.Strategies.ObjectStrategy);

                    public class UnsupportedTypeException : Types.Exception
                    {
                        public UnsupportedTypeException() { }
                        public UnsupportedTypeException(string message) : base(message) { }
                        public UnsupportedTypeException(string message, Exception inner) : base(message, inner) { }
                    }

                    /**
                     * Each strategy knows its holder and its counterpart type.
                     */
                    public ObjectsManagementStrategyHolder StrategyHolder { get; private set; }
                    public Type CounterpartType { get; private set; }

                    public void Awake()
                    {
                        StrategyHolder = GetComponent<ObjectsManagementStrategyHolder>();
                        CounterpartType = GetCounterpartType();
                        if (CounterpartType == null || !CounterpartType.IsSubclassOf(baseCounterpartStrategyType))
                        {
                            Destroy(gameObject);
                            throw new UnsupportedTypeException(string.Format("The type returned by CounterpartType must be a subclass of {0}", baseCounterpartStrategyType.FullName));
                        }
                    }

                    /**
                     * Gets the counterpart type to operate against (in the map).
                     */
                    protected abstract Type GetCounterpartType();

                    /**
                     * Initializing the global data will require initializing masks and other arrays, in a
                     *   per-strategy basis.
                     */
                    public virtual void InitGlobalCellsData()
                    {
                    }

                    /**
                     * Initializing the cells data may involve an individual iterator, or not.
                     */
                    public virtual void InitIndividualCellsData(Action<Action<uint, uint>> allCellsIterator)
                    {
                        allCellsIterator(ComputeCellData);
                    }

                    /**
                     * Updating a strategy actually computes a single cell. It is intended to be run after a tilemap
                     *   was changed a single tile, or in per-cell basis on initialization.
                     */
                    public virtual void ComputeCellData(uint x, uint y)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Attaching an object (strategy).
                     * 
                     *************************************************************************************************/

                    /**
                     * Object strategy was added. Compute relevant data here.
                     */
                    public virtual void AttachedStrategy(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                    {
                    }

                    /**
                     * Object strategy is to be removed. Compute relevant data here.
                     */
                    public virtual void DetachedStrategy(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Starting the movement of an object (strategy) in a direction and telling whether it is the
                     *   continuation of a former movement (this will happen in Movable component).
                     * 
                     *************************************************************************************************/

                    /**
                     * Tells whether movement can be allocated.
                     */
                    public abstract bool CanAllocateMovement(Dictionary<Type, bool> otherComponentsResults, Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Types.Direction direction, bool continuated);

                    /**
                     * You may define this method. Ask conditionally for "Before", "AfterMovementAllocation", "After".
                     */
                    public virtual void DoAllocateMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Types.Direction direction, bool continuated, string stage)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Cancelling the movement of an object (strategy).
                     * 
                     *************************************************************************************************/

                    /**
                     * Override it to use a different criteria for allowing movement cancel.
                     */
                    // public virtual bool CanClearMovement(Objects.Strategies.ObjectStrategy strategy, StrategyHolder.Status status)
                    // {
                    //     return status.Movement != null;
                    // }
                    public abstract bool CanClearMovement(Dictionary<Type, bool> otherComponentsResults, Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status);

                    /**
                     * You may define this method. Ask conditionally for "Before", "AfterMovementClear", "After".
                     */
                    public virtual void DoClearMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Types.Direction? formerMovement, string stage)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Finishing the movement of an object (strategy), if any.
                     * 
                     *************************************************************************************************/

                    /**
                     * You may define this method. Ask conditionally for "Before", "AfterPositionChange", "AfterMovementClear", "After".
                     */
                    public virtual void DoConfirmMovement(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, Types.Direction? formerMovement, string stage)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Teleports the object strategy to another position in the map.
                     * 
                     *************************************************************************************************/

                    /**
                     * You may define this method. Ask conditionally for stages "Before", "AfterPositionChange", "After".
                     */
                    public virtual void DoTeleport(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, uint x, uint y, string stage)
                    {
                    }

                    /*************************************************************************************************
                     * 
                     * Updates according to particular data change. These fields exist in the strategy.
                     * 
                     *************************************************************************************************/

                    /**
                     * You may define this method.
                     */
                    public virtual void DoProcessPropertyUpdate(Objects.Strategies.ObjectStrategy strategy, ObjectsManagementStrategyHolder.Status status, string property, object oldValue, object newValue)
                    {
                    }
                }
            }
        }
    }
}
