using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
             */
            [RequireComponent(typeof(UnityEngine.UI.Text))]
            [RequireComponent(typeof(UnityEngine.UI.ContentSizeFitter))]
            public class InteractiveMessageContent : MonoBehaviour
            {
                [SerializeField]
                private float slowTimeBetweenLetters = 0.05f;

                [SerializeField]
                private float quickTimeBetweenLetters = 0.005f;

                private UnityEngine.UI.Text textComponent;
                private Coroutine currentTextMessageCoroutine;
                public bool QuickTextMovement = false;

                void Start()
                {
                    textComponent = GetComponent<UnityEngine.UI.Text>();
                }

                public Coroutine StartTextMessage(string text)
                {
                    if (currentTextMessageCoroutine == null)
                    {
                        currentTextMessageCoroutine = StartCoroutine(TextMessageCoroutine(text));
                    }
                    return currentTextMessageCoroutine;
                }

                private IEnumerator TextMessageCoroutine(string text)
                {
                    text = text ?? "";
                    float slowInterval = Utils.Values.Max(0.0001f, slowTimeBetweenLetters);
                    float quickInterval = Utils.Values.Max(0.00001f, quickTimeBetweenLetters);
                    StringBuilder builder = new StringBuilder("");

                    foreach (char current in text)
                    {
                        builder.Append(current);
                        textComponent.text = builder.ToString();
                        yield return new WaitForSeconds(QuickTextMovement ? quickInterval : slowInterval);
                    }
                    currentTextMessageCoroutine = null;
                }
            }
        }
    }
}