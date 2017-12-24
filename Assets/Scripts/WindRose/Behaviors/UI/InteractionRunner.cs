using System.Collections;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            /**
             * This component provides behavior to run an interaction.
             * 
             * Running an interaction involves pausing and unpausing a map to wrap the
             *   interaction with the user in a way that does not interfere with the game
             *   flow. An example of this, is Pokemon games (where the whole game stops
             *   while interacting with the user).
             */
            public class InteractionRunner : MonoBehaviour
            {
                /**
                 * We also need a Map object to relate.
                 * 
                 * When we set it in Design Time, we may reference an object having a MapLoader instead of
                 *   having a Map object. For this reason, we are not requiring a Map right now (it will be
                 *   required later).
                 * 
                 * A Map object will be required from this object to pause and resume the activity of game
                 *   objects. We will store such reference as well. If we cannot get the reference, it will
                 *   remain null and no pause/resume of animations will occur.
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
                 * Gets the Map component according to what is described in `map` and `mapHolder` members.
                 */
                private Map GetMap()
                {
                    if (map == null)
                    {
                        map = GetComponent<Map>();
                    }
                    return map;
                }

                /**
                 * Runs the lifecycle of an interaction. Running an interaction involves three steps:
                 *   1. Pausing everything, according to our decision of also freezing animations, or not.
                 *   2. Running the actual interaction.
                 *   3. Resuming everything.
                 */
                public void RunInteraction(IEnumerator interaction)
                {
                    StartCoroutine(WrappedInteraction(interaction));
                }

                private IEnumerator WrappedInteraction(IEnumerator innerInteraction)
                {
                    GetMap().Pause(freezeAlsoAnimations);
                    yield return innerInteraction;
                    GetMap().Resume();
                }
            }
        }
    }
}