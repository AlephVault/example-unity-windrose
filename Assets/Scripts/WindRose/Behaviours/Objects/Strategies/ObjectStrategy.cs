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
                 * A map strategy requires a map, but will not perform any particular event but, instead, provide
                 *   functionality.
                 * 
                 * The Map behaviour will try to find a strategy (a subclass of this class) or die trying. When
                 *   needed, the map will invoke stuff on this class.
                 */
                [RequireComponent(typeof(Positionable))]
                public abstract class ObjectStrategy : MonoBehaviour
                {
                    /**
                     * Each strategy knows its positionable.
                     */
                    public Positionable Positionable { get; private set; }

                    /**
                     * On initialization, the strategy will fetch its positionable to, actually, know it.
                     */
                    protected virtual void Awake()
                    {
                        Positionable = GetComponent<Positionable>();
                    }

                    /**
                     * Sends an event to the positionable and the whole component.
                     */
                    public void TriggerEvent(string targetEvent, params object[] args)
                    {
                        SendMessage(targetEvent, args, SendMessageOptions.DontRequireReceiver);
                    }

                    /**
                     * Sends the property update to the map strategy.
                     */
                    protected void PropertyWasUpdated(string property, object oldValue, object newValue)
                    {
                        Map map = Positionable == null ? null : Positionable.ParentMap;
                        Behaviours.Strategies.Strategy mapStrategy = map == null ? null : map.Strategy;
                        if (mapStrategy != null) { mapStrategy.PropertyWasUpdated(this, property, oldValue, newValue); }
                    }
                }
            }
        }
    }
}
