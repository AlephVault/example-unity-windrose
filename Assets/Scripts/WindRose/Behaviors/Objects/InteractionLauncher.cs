using UnityEngine;
using GabTab.Behaviours;

namespace WindRose
{
    namespace Behaviours
    {
        public class InteractionLauncher : MonoBehaviour
        {
            /**
             * Retrieves the interaction tab from an ancestor InteractionProvider object.
             */
            public InteractiveInterface InteractionTab
            {
                get
                {
                    return GetComponentInParent<UI.InteractionProvider>().InteractionTab;
                }
            }
        }
    }
}
