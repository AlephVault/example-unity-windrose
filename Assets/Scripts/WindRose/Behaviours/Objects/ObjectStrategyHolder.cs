using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            using Strategies;

            /**
                * An object strategy holder will reference its positionable and also will find
                *   its way to initialize its strategy. When initializing the strategy, it should
                *   provide itself to the strategy constructor (alongside any needed data).
                */
            [RequireComponent(typeof(Positionable))]
            public class ObjectStrategyHolder : MonoBehaviour
            {
                /**
                    * All the needed exceptions go here.
                    */
                public class InvalidStrategyComponentException : Types.Exception
                {
                    public InvalidStrategyComponentException() { }
                    public InvalidStrategyComponentException(string message) : base(message) { }
                    public InvalidStrategyComponentException(string message, Exception inner) : base(message, inner) { }
                }

                /**
                    * Each strategy holder knows its positionable.
                    */
                public Positionable Positionable { get; private set; }

                /**
                    * The root strategy that can be picked in the editor.
                    */
                [SerializeField]
                private ObjectStrategy objectStrategy;

                /**
                    * Each strategy holder tells its strategy.
                    */
                public ObjectStrategy ObjectStrategy { get { return objectStrategy; } }

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
                    if (objectStrategy == null || !(new HashSet<ObjectStrategy>(GetComponents<ObjectStrategy>()).Contains(objectStrategy))) {
                        Destroy(gameObject);
                        throw new InvalidStrategyComponentException("The selected strategy component must be non-null and present among the current object's components");
                    }
                }
            }
        }
    }
}
