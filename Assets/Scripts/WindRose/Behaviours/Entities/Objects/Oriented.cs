using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   Oriented objects have a direction and will notify when the direction changes.
            ///   Typically, the behaviours that will listen to the changes in the direction
            ///     are <see cref="Visuals.RoseAnimated"/> components inside the visuals
            ///     attached to the object. Also, when paused, they cannot change their
            ///     orientation.
            /// </summary>
            [RequireComponent(typeof(Object))]
            public class Oriented : MonoBehaviour, Common.Pausable.IPausable
            {
                public class OrientationEvent : UnityEvent<Types.Direction> { }

                /// <summary>
                ///   Notofies when the direction property changes.
                /// </summary>
                public readonly OrientationEvent onOrientationChanged = new OrientationEvent();

                private bool paused = false;

                [SerializeField]
                private Types.Direction orientation = Types.Direction.FRONT;

                /// <summary>
                ///   Gets or sets the current orientation. Notifies the interested
                ///     behaviours of the orientation change.
                /// </summary>
                public Types.Direction Orientation
                {
                    get
                    {
                        return orientation;
                    }
                    set
                    {
                        if (paused) return;
                        orientation = value;
                        onOrientationChanged.Invoke(orientation);
                    }
                }

                private void DoStart()
                {
                    onOrientationChanged.Invoke(orientation);
                }

                public void Pause(bool fullFreeze)
                {
                    paused = true;
                }

                public void Resume()
                {
                    paused = false;
                }
            }
        }
    }
}