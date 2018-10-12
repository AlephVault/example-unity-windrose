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
                 * An object strategy holder will reference its positionable and also will find
                 *   its way to initialize its strategy. When initializing the strategy, it should
                 *   provide itself to the strategy constructor (alongside any needed data).
                 */
                [RequireComponent(typeof(Positionable))]
                public abstract class ObjectStrategyHolder : MonoBehaviour
                {
                    /**
                     * Each strategy holder knows its positionable.
                     */
                    public Positionable Positionable { get; private set; }

                    /**
                     * Each strategy holder knows its strategy.
                     */
                    public ObjectStrategy ObjectStrategy { get; private set; }

                    /**
                     * And also each strategy holder knows how to instantiate its strategy.
                     */
                    protected abstract ObjectStrategy BuildStrategy();

                    /**
                     * Initializing a strategy will be done on positionable initialization. This means: the map calls this
                     *   method from its own behaviour.
                     */
                    public void Initialize()
                    {
                        ObjectStrategy.Initialize();
                    }

                    /**
                     * On initialization, the strategy will fetch its positionable to, actually, know it.
                     */
                    protected virtual void Awake()
                    {
                        Positionable = GetComponent<Positionable>();
                        ObjectStrategy = BuildStrategy();
                    }
                }
            }
        }
    }
}
