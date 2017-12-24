using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            /**
             * An interactive interface will need three components to get started:
             *   1. An image component. Images are components that provide background.
             *      Otherwise, any component could be used as a parent, but since we
             *      care about the fact that an interactive interface has a background,
             *      we require an image.
             *      Recommended settings:
             *        > Image Type: Slice
             *          > Fill Center: True
             *   2. An interaction runner. It has the behavior to wrap any interaction
             *        given as a generator. This behavior is needed to pause a related
             *        game map.
             *      Recommended settings:
             *        > Map Holder: A GameObject having a MapLoader or Map component.
             *   3. An interactive message. This behavior (documented on its own) has
             *        the duty of displaying a message the user can read.
             */
            [RequireComponent(typeof(UnityEngine.UI.Image))]
            [RequireComponent(typeof(InteractionRunner))]
            [RequireComponent(typeof(InteractiveMessage))]
            class InteractiveInterface : MonoBehaviour
            {
                private InteractionRunner interactionRunner;
                private InteractiveMessage interactiveMessage;

                private void Start()
                {
                    interactionRunner = GetComponent<InteractionRunner>();
                    interactiveMessage = GetComponent<InteractiveMessage>();
                }
            }
        }
    }
}
