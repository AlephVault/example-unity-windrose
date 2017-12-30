using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
             *   2. A hideable component that will be used to determine whether this component
             *        should be hidden or not.
             *   3. An interactive message (in children). This behavior (documented on its own) has
             *        the duty of displaying a message the user can read.
             *   4. A manager of components used for single text, user input, and
             *      more complex cases.
             * 
             * This behaviour exposes a method to run this magic: RunInteraction(Interaction func).
             * The interaction must be a generator function (remember that C# does not allow anonymous
             *   functions working as generators). For more information, see RunInteraction method's
             *   documentation in this class.
             */
            [RequireComponent(typeof(UnityEngine.UI.Image))]
            [RequireComponent(typeof(Hideable))]
            [RequireComponent(typeof(Interactors.InteractorsManager))]
            class InteractiveInterface : MonoBehaviour
            {
                private InteractiveMessage interactiveMessage;
                private Interactors.InteractorsManager interactorsManager;
                /**
                 * See Update() and WrappedInteraction(IEnumerator generator) on how are these
                 *   variables used.
                 */
                private bool interactionRunning = false;
                private Hideable hideable;

                public readonly UnityEvent beforeRunningInteraction = new UnityEvent();
                public readonly UnityEvent afterRunningInteraction = new UnityEvent();

                void Start()
                {
                    interactiveMessage = Utils.Layout.RequireComponentInChildren<InteractiveMessage>(gameObject);
                    interactorsManager = GetComponent<Interactors.InteractorsManager>();
                    hideable = GetComponent<Hideable>();
                }

                /**
                 * The component will remain hidden as long as an interaction is running.
                 */
                void Update()
                {
                    hideable.Hidden = !interactionRunning;
                }

                /**
                 * This part is the damn core of the whole.
                 * 
                 * We may call like RunInteraction(someObject.AGeneratorMethod).
                 * 
                 * The object may come from any class (will be considered external code to our purpose).
                 * The only conditions are that the method being passed:
                 *   1. Return an IEnumerator. The ideal case is that the method is a generator (i.e. contains `yield return` expressions).
                 *      Remember that anonymous methods cannot have `yield return` expressions.
                 *      INSIDE THIS FUNCTION you will `yield return` expressions like manager["a-registered-interactor"].RunInteraction(...).
                 *      Aside from that, you will create a usual flow of code (e.g. asking for a returned value in one specific interactor
                 *        registered in the manager, and branching your code accordinly).
                 *      PLEASE FOR GOD'S SAKE take a look to the implementation of RunInteraction in Interactor classes.
                 *   2. Accept two parameters: Interactors.InteractorsManager and InteractiveMessage.
                 *      The first one is the inputs component finder, while the second one is the component that shows messages.
                 *      The components found in the first parameter will make use of the message shower in the second parameter.
                 */
                public Coroutine RunInteraction(Func<Interactors.InteractorsManager, InteractiveMessage, IEnumerator> runnable)
                {
                    return StartCoroutine(WrappedInteraction(runnable(interactorsManager, interactiveMessage)));
                }

                private IEnumerator WrappedInteraction(IEnumerator innerInteraction)
                {
                    if (interactionRunning)
                    {
                        throw new Types.Exception("Cannot run the interaction: A previous interaction is already running");
                    }
                    interactionRunning = true;
                    beforeRunningInteraction.Invoke(); // GetMap().Pause(freezeAlsoAnimations);
                    yield return StartCoroutine(innerInteraction);
                    afterRunningInteraction.Invoke(); // GetMap().Resume();
                    interactionRunning = false;
                }
            }
        }
    }
}
