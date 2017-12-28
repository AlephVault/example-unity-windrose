using System.Collections;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            /**
             * This component provides behaviour to run an interaction.
             * 
             * Running an interaction involves pausing and unpausing a map to wrap the
             *   interaction with the user in a way that does not interfere with the game
             *   flow. An example of this, is Pokemon games (where the whole game stops
             *   while interacting with the user).
             * 
             * When an interaction is run, this component becomes visible so the user
             *   (player) can interact with it.
             * 
             * This behaviour provides a method to run an interaction:
             *   RunInteraction(IEnumerable generator)
             * Such method is not intended to be called on its own, but to be called
             *   from the InteractiveInterface behaviour.
             * If somehow no map is present in the mapHolder object at the time such
             *   method is called, the method will fail silently.
             */
            [RequireComponent(typeof(Hideable))]
            public class InteractionRunner : MonoBehaviour
            {
                /**
                 * See Update() and WrappedInteraction(IEnumerator generator) on how are these
                 *   variables used.
                 */
                private bool interactionRunning = false;
                private Hideable hideable;

                /**
                 * We also need a Map object to relate.
                 * 
                 * When we set it in Design Time, we may reference an object having a MapLoader instead of
                 *   having a Map object. For this reason, we are not requiring a Map right now (it will be
                 *   required later).
                 * 
                 * A Map object will be required from this object to pause and resume the activity of game
                 *   objects. We will store such reference as well. If we cannot get the reference, it will
                 *   remain null and no interaction will ever occur (until the map component exists).
                 */
                [SerializeField]
                private GameObject mapHolder;
                private Map map;

                /**
                 * This determines whether the animations should also be frozen or not, when pausing the entire
                 *   map.
                 */
                [SerializeField]
                private bool freezeAlsoAnimations;

                /**
                 * At startup we assign the Hideable component, that will be used in Update.
                 */
                void Start()
                {
                    hideable = GetComponent<Hideable>();
                }

                /**
                 * Gets the Map component according to what is described in `map` and `mapHolder` members.
                 */
                private Map GetMap()
                {
                    if (map == null)
                    {
                        map = mapHolder.GetComponent<Map>();
                    }
                    return map;
                }

                /**
                 * Runs the lifecycle of an interaction. Running an interaction involves three steps:
                 *   1. Starting the interaction and displays.
                 *   2. Pausing everything, according to our decision of also freezing animations, or not.
                 *   3. Running the actual interaction.
                 *   4. Resuming everything.
                 *   5. Ending the interaction and hides.
                 */
                public Coroutine RunInteraction(IEnumerator interaction)
                {
                    return StartCoroutine(WrappedInteraction(interaction));
                }

                private IEnumerator WrappedInteraction(IEnumerator innerInteraction)
                {
                    if (GetMap() == null)
                    {
                        yield break;
                    }

                    if (interactionRunning)
                    {
                        throw new Types.Exception("Cannot run the interaction: A previous interaction is already running");
                    }
                    interactionRunning = true;
                    GetMap().Pause(freezeAlsoAnimations);
                    yield return StartCoroutine(innerInteraction);
                    GetMap().Resume();
                    interactionRunning = false;
                }

                /**
                 * The component will remain hidden as long as an interaction is running.
                 */
                void Update()
                {
                    hideable.Hidden = !interactionRunning;
                }
            }
        }
    }
}