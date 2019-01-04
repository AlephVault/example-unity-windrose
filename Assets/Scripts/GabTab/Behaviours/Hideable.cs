using UnityEngine;

namespace GabTab
{
    namespace Behaviours
    {
        /**
         * <summary>
         *   This behaviour makes use of a RectTransform of a component to hide it.
         * </summary>
         * <remarks>
         *   Actually, this behaviour has only one member (`Hidden`) which hides or
         *     shows the RectTransform (by changing scale to (0,0,0) or (1,1,1)
         *     respectively).
         * </remarks>
         */
        [ExecuteInEditMode]
        [RequireComponent(typeof(RectTransform))]
        public class Hideable : MonoBehaviour
        {
            private RectTransform rectTransform;
            /**
             * <summary>
             *   You will change this value to <c>true</c> to hide the object with
             *     this component, and <c>false</c> to show it.
             * </summary>
             */
            public bool Hidden = false;

            void Awake()
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
