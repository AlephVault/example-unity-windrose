using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            public class AddOn : MonoBehaviour, Common.Pausable.IPausable
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

                /// <summary>
                ///   Pauses the add-on update.
                /// </summary>
                /// <param name="fullFreeze">Whether to also pause the animations or not</param>
                public void Pause(bool fullFreeze)
                {

                }

                /// <summary>
                ///   Resumes the add-on update and animations.
                /// </summary>
                public void Resume()
                {

                }

                /// <summary>
                ///   Executes all the enable logic here. A replacement of the Start() or the
                ///     OnEnable() method.
                ///   This will be run not at component start but instead at component attach
                ///     to an <see cref="AddOnGroup"/>. This method will be invoked externally
                ///     and it is not intended to be manually invoked by the user.
                /// </summary>
                public void Attached(AddOnGroup group)
                {

                }

                /// <summary>
                ///   Executes all the disable logic here. A replacement of the OnDestroy() or
                ///     the OnDisable() method.
                ///   This will be run not at component destroy but instead at component detach
                ///     from an <see cref="AddOnGroup"/>. This method will be invoked externally
                ///     and it is not intended to be manually invoked by the user.
                /// </summary>
                public void Detached()
                {

                }

                /// <summary>
                ///   Performs the actual add-on update.
                /// </summary>
                public void UpdatePipeline()
                {

                }
            }
        }
    }
}
