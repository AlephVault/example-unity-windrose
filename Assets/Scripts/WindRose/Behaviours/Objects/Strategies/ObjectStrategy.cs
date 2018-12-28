using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Strategies
            {
                /**
                 * This is an object strategy. It will know:
                 * - The strategy-holder it is tied to (directly or indirectly).
                 * And it will have as behaviour:
                 * - A way of notifying the strategy-holder's positionable's map's
                 *   strategy about changes. Actually, each subclass of strategy
                 *   will decide when to notify the changes appropriately.
                 * This behaviour will be a Unity Behaviour, so it will be attached
                 *   to the same object of the ObjectStrategyHolder.
                 * It is related to a counterpart type which is a subtype of map
                 *   strategy.
                 */
                [RequireComponent(typeof(Positionable))]
                public abstract class ObjectStrategy : MonoBehaviour
                {
                    private static Type baseCounterpartStrategyType = typeof(World.ObjectsManagementStrategies.ObjectsManagementStrategy);

                    public class UnsupportedTypeException : Types.Exception
                    {
                        public UnsupportedTypeException() { }
                        public UnsupportedTypeException(string message) : base(message) { }
                        public UnsupportedTypeException(string message, Exception inner) : base(message, inner) { }
                    }

                    /**
                     * Each strategy knows its holder and its counterpart type.
                     */
                    public ObjectStrategyHolder StrategyHolder { get; private set; }
                    public Type CounterpartType { get; private set; }
                    public Positionable Positionable { get; private set; }

                    public virtual void Awake()
                    {
                        StrategyHolder = GetComponent<ObjectStrategyHolder>();
                        CounterpartType = GetCounterpartType();
                        if (CounterpartType == null || !Support.Utils.Classes.IsSameOrSubclassOf(CounterpartType, baseCounterpartStrategyType))
                        {
                            Destroy(gameObject);
                            throw new UnsupportedTypeException(string.Format("The type returned by CounterpartType must be a subclass of {0}", baseCounterpartStrategyType.FullName));
                        }
                        Positionable = GetComponent<Positionable>();
                    }

                    /**
                     * Gets the counterpart type to operate against (in the map).
                     */
                    protected abstract Type GetCounterpartType();

                    /**
                     * This method initializes the current strategy.
                     */
                    public virtual void Initialize()
                    {
                    }

                    /**
                     * Sends the property update to the map strategy.
                     */
                    protected void PropertyWasUpdated(string property, object oldValue, object newValue)
                    {
                        World.ObjectsManagementStrategyHolder strategyHolder = null;
                        try
                        {
                            strategyHolder = StrategyHolder.Positionable.ParentMap.StrategyHolder;
                        }
                        catch(NullReferenceException)
                        {
                            strategyHolder = null;
                        }
                        if (strategyHolder != null) { strategyHolder.PropertyWasUpdated(StrategyHolder, this, property, oldValue, newValue); }
                    }
                }
            }
        }
    }
}
