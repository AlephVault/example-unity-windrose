using System.Text;
using System.Collections;
using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
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
             * 
             * The behaviour is implemented by StartTextMessage(string, bool, bool). The second parameter is
             *   optional and defaults to true. When true, it will clear the former text in the display before
             *   starting a new message. The third parameter is also optional and tells whether to wait a delay
             *   or not (according to `slowDelayAfterMessage` and `quickDelayAfterMessage`) after the message
             *   is fully displayed.
             * 
             * The stated behaviour should not be invoked on its own, but only by InteractiveMessage lifecycle
             *   (which in turn, should only be called inside implementation of methods in Interactor class
             *   and subclasses).
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

                private UnityEngine.UI.Text textComponent;
                private bool textBeingSent = false;
                public bool QuickTextMovement = false;

                void Start()
                {
                    textComponent = GetComponent<UnityEngine.UI.Text>();
                }

                /**
                 * Starts a text message in the display. It can be stated whether to clear the former text in the display, and whether
                 *   to wait a time so the user ends reading it, or not.
                 */
                public Coroutine StartTextMessage(string text, bool clearFormerTextBeforeStart = true, bool delayAfterFinish = true)
                {
                    return StartCoroutine(TextMessageCoroutine(text, clearFormerTextBeforeStart, delayAfterFinish));
                }

                private IEnumerator TextMessageCoroutine(string text, bool clearFormerTextBeforeStart = true, bool delayAfterFinish = true)
                {
                    if (textBeingSent)
                    {
                        throw new Types.Exception("Cannot start a text message: A previous text message is already being sent");
                    }

                    textBeingSent = true;
                    text = text ?? "";
                    float slowInterval = Values.Max(0.0001f, slowTimeBetweenLetters);
                    float quickInterval = Values.Max(0.00001f, quickTimeBetweenLetters);
                    StringBuilder builder = new StringBuilder(clearFormerTextBeforeStart ? "" : textComponent.text);

                    foreach (char current in text)
                    {
                        builder.Append(current);
                        textComponent.text = builder.ToString();
                        yield return new WaitForSeconds(QuickTextMovement ? quickInterval : slowInterval);
                    }
                    if (delayAfterFinish)
                    {
                        yield return new WaitForSeconds(QuickTextMovement ? quickDelayAfterMessage : slowDelayAfterMessage);
                    }
                    textBeingSent = false;
                }
            }
        }
    }
}