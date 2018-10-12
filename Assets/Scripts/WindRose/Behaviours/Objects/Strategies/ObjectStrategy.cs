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
                 */
                public abstract class ObjectStrategy
                {
                    /**
                     * Each strategy knows its holder.
                     */
                    public ObjectStrategyHolder StrategyHolder { get; private set; }

                    public ObjectStrategy(ObjectStrategyHolder StrategyHolder)
                    {
                        this.StrategyHolder = StrategyHolder;
                    }

                    /**
                     * Sends an event to the strategy holder. Even for combined strategies,
                     *   this method will be available but triggered accordingly.
                     */
                    public void TriggerEvent(string targetEvent, params object[] args)
                    {
                        StrategyHolder.SendMessage(targetEvent, args, SendMessageOptions.DontRequireReceiver);
                    }

                    /**
                     * You may define this method to perform custom initialization of your component (even
                     *   perhaps interacting back with the positionable).
                     * 
                     * This method is invoked externally, by the positionable.
                     */
                    public virtual void Initialize() {}

                    /**
                     * Sends the property update to the map strategy.
                     */
                    protected void PropertyWasUpdated(string property, object oldValue, object newValue)
                    {
                        Behaviours.Strategies.StrategyHolder mapStrategyHolder = null;
                        try
                        {
                            mapStrategyHolder = StrategyHolder.Positionable.ParentMap.StrategyHolder;
                        }
                        catch(NullReferenceException)
                        {
                            mapStrategyHolder = null;
                        }
                        if (mapStrategyHolder != null) { mapStrategyHolder.PropertyWasUpdated(StrategyHolder, property, oldValue, newValue); }
                    }
                }
            }
        }
    }
}
