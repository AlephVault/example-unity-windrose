using UnityEngine;
namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            /**
             * This behaviour makes use of a RectTransform of a component to hide it.
             * 
             * Actually, this behaviour has only one member (`Hidden`) which hides or
             *   shows the RectTransform (by changing scale to (0,0,0) or (1,1,1)
             *   respectively).
             */
            [RequireComponent(typeof(RectTransform))]
            class Hidable : MonoBehaviour
            {
                private RectTransform rectTransform;
                public bool Hidden = false;

                void Start()
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                void Update()
                {
                    rectTransform.localScale = Hidden ? Vector3.zero : Vector3.one;
                }
            }
        }
    }
}
