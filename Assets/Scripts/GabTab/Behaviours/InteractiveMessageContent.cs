using System.Text;
using System.Collections;
using UnityEngine;
using Support.Utils;

namespace GabTab
{
    namespace Behaviours
    {
        /**
         * This behavior is the one that fills the message to show to the user. It needs two components
         *   to works properly:
         *   1. A text component. Indeed, this is the element that will show the text to the user.
         *      Recommended settings:
         *      > Paragraph
         *        > Alignment: Left and Top
         *        > Horizontal Overflow: Wrap
         *        > Vertical Overflow: Overflow
         *      > Character:
         *        > Line Spacing: 1
         *   2. A content size fitter component. This component is used to scroll the text.
         *      Recommended settings:
         *      > Horizontal Fit: Unconstrained
         *      > Vertical Fit: Preferred Size
         * 
         * This component provides the behaviour to the parent(s) component(s) to start a text message.
         * The text message will be filled at different speeds (you can configure a slow and a quick speed).
         * You can also change (at runtime) whether the text should be filled using the quick or slow
         *   speed (this is useful if, e.g., having a button that accelerates the text filling).
         */
        [RequireComponent(typeof(UnityEngine.UI.Text))]
        [RequireComponent(typeof(UnityEngine.UI.ContentSizeFitter))]
        public class InteractiveMessageContent : MonoBehaviour
        {
            [SerializeField]
            private float slowTimeBetweenLetters = 0.05f;

            [SerializeField]
            private float quickTimeBetweenLetters = 0.005f;

            [SerializeField]
            private float slowDelayAfterMessage = 0.5f;

            [SerializeField]
            private float quickDelayAfterMessage = 0.05f;

            public bool QuickTextMovement = false;

            public Types.WaitForQuickOrSlowSeconds CharacterWaiterCoroutine()
            {
                return new Types.WaitForQuickOrSlowSeconds(
                    Values.Max(0.00001f, quickTimeBetweenLetters),
                    Values.Max(0.0001f, slowTimeBetweenLetters),
                    delegate () { return QuickTextMovement; }
                );
            }

            public Types.WaitForQuickOrSlowSeconds ExplicitWaiterCoroutine(float? seconds = null)
            {
                if (seconds == null)
                {
                    return new Types.WaitForQuickOrSlowSeconds(
                        Values.Max(0.00001f, quickDelayAfterMessage),
                        Values.Max(0.0001f, slowDelayAfterMessage),
                        delegate () { return QuickTextMovement; }
                    );
                }
                else
                {
                    return new Types.WaitForQuickOrSlowSeconds(
                        Values.Max(0.00001f, seconds.Value / 10),
                        Values.Max(0.0001f, seconds.Value),
                        delegate () { return QuickTextMovement; }
                    );
                }
            }
        }
    }
}