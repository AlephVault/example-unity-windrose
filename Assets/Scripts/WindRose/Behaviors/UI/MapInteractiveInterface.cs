using UnityEngine;
using GabTab.Behaviours;

namespace WindRose
{
    namespace Behaviours
    {
        namespace UI
        {
            /**
             * This component provides behaviour to pause a map while running an interaction.
             *   It will be related to an Interactive Interface, and we should use this one
             *   in WindRose if we want to pause the whole map while running an interaction.
             */
            [RequireComponent(typeof(InteractiveInterface))]
            public class MapInteractiveInterface : MonoBehaviour
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
                 *   remain null and no interaction will ever occur (until the map component exists).
                 */
                [SerializeField]
                private GameObject mapHolder;

                [HideInInspector]
                public Map Map;

                /**
                 * This determines whether the animations should also be frozen or not, when pausing the entire
                 *   map.
                 */
                [SerializeField]
                private bool freezeAlsoAnimations;

                /**
                 * Gets the Map component according to what is described in `map` and `mapHolder` members.
                 * If map is null and mapHolder is not null, we retrieve map and release mapHolder.
                 */
                private Map GetMap()
                {
                    if (Map == null && mapHolder != null)
                    {
                        Map = mapHolder.GetComponent<Map>();
                        mapHolder = null;
                    }
                    return Map;
                }

                void Start()
                {
                    InteractiveInterface interactiveInterface = GetComponent<InteractiveInterface>();
                    interactiveInterface.beforeRunningInteraction.AddListener(delegate () {
                        Map map = GetMap();
                        if (map != null) map.Pause(freezeAlsoAnimations);
                    });
                    interactiveInterface.afterRunningInteraction.AddListener(delegate ()
                    {
                        Map map = GetMap();
                        if (map != null) map.Resume();
                    });
                }
            }
        }
    }
}