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
                /// <summary>
                ///   <para>
                ///     Object strategies are the counterpart of <see cref="World.ObjectsManagementStrategies.ObjectsManagementStrategy"/>,
                ///       and will reside in the same object holding an <see cref="ObjectStrategyHolder"/>.
                ///   </para>
                ///   <para>
                ///     Quite often they hold state instead of logic, and notify the counterpart management strategy in the map by invoking
                ///       <see cref="PropertyWasUpdated(string, object, object)"/>.
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(Positionable))]
                public abstract class ObjectStrategy : MonoBehaviour
                {
                    private static Type baseCounterpartStrategyType = typeof(World.ObjectsManagementStrategies.ObjectsManagementStrategy);

                    /// <summary>
                    ///   Tells when the given counterpart type is not valid (i.e. subclass of
                    ///     <see cref="World.ObjectsManagementStrategies.ObjectsManagementStrategy"/>).
                    /// </summary>
                    public class UnsupportedTypeException : Types.Exception
                    {
                        public UnsupportedTypeException() { }
                        public UnsupportedTypeException(string message) : base(message) { }
                        public UnsupportedTypeException(string message, Exception inner) : base(message, inner) { }
                    }

                    /// <summary>
                    ///   The related strategy holder, which will be in the same object.
                    /// </summary>
                    public ObjectStrategyHolder StrategyHolder { get; private set; }

                    /// <summary>
                    ///   <para>
                    ///     The counterpart type, which is particular to each subtype of object
                    ///       strategy.
                    ///   </para>
                    ///   <para>
                    ///     It will be a subclass of <see cref="World.ObjectsManagementStrategies.ObjectsManagementStrategy"/>.
                    ///   </para>
                    /// </summary>
                    public Type CounterpartType { get; private set; }

                    /// <summary>
                    ///   The related object.
                    /// </summary>
                    public Positionable Positionable { get; private set; }

                    /**
                     * Initializes the related data and the counterpart type.
                     */
                    protected virtual void Awake()
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

                    /// <summary>
                    ///   Returns the counterpart type, which is per-strategy defined and
                    ///     is subclass of <see cref="World.ObjectsManagementStrategies.ObjectsManagementStrategy"/>.
                    /// </summary>
                    /// <returns>The counterpart type</returns>
                    protected abstract Type GetCounterpartType();

                    /// <summary>
                    ///   <para>
                    ///     This method is run by the <see cref="ObjectStrategyHolder"/> and there
                    ///       is no need or use to invoke it by hand.
                    ///   </para>
                    ///   <para>
                    ///     Initializes the strategy, after the positionable is initialized.
                    ///   </para>
                    /// </summary>
                    public virtual void Initialize()
                    {
                    }

                    /// <summary>
                    ///   Use this method inside properties' logic to notify the value change.
                    /// </summary>
                    /// <param name="property">The property being changed</param>
                    /// <param name="oldValue">The old value</param>
                    /// <param name="newValue">The new value</param>
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
