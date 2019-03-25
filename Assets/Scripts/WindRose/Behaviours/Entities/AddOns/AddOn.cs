using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.AddOns
        {
            /// <summary>
            ///   Add-ons go inside <see cref="AddOnGroup"/> instances. They are mostly
            ///     graphic-only components, but they also may not be.
            /// </summary>
            [RequireComponent(typeof(Pausable))]
            public class AddOn : MonoBehaviour
            {
                // The renderer, if any.
                private new SpriteRenderer renderer;

                private void Awake()
                {
                    renderer = GetComponent<SpriteRenderer>();
                    // TODO: register the behaviours in the attach && update pipelines
                }

                /// <summary>
                ///   Gets or sets the sorting order, if any. On set, the sorting layer
                ///     is always set to 0.
                /// </summary>
                public int SortingOrder
                {
                    get
                    {
                        return renderer ? renderer.sortingOrder : 0;
                    }
                    set
                    {
                        if (renderer)
                        {
                            renderer.sortingOrder = value;
                            renderer.sortingLayerID = 0;
                        }
                    }
                }

                public class GroupAttachedEvent : UnityEvent<AddOnGroup> {};

                /// <summary>
                ///   Performs the actual add-on attachment logic. Triggered by the
                ///     add-ons group.
                /// </summary>
                public readonly GroupAttachedEvent onGroupAttached = new GroupAttachedEvent();

                /// <summary>
                ///   Performs the actual add-on detachment logic. Triggered by the
                ///     add-ons group.
                /// </summary>
                public readonly UnityEvent onGroupDetach = new UnityEvent();

                /// <summary>
                ///   Performs the actual add-on update. Triggered by the add-ons group.
                /// </summary>
                public readonly UnityEvent onGroupUpdate = new UnityEvent();
            }
        }
    }
}
