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
             * This component handles the message being displayed to the user. It requires two components
             *   in the same object, and one component in a child. The components go like this:
             *   1. Image (in object): Is the background of the message.
             *      Recommended settings:
             *        > Image Type: Slice
             *          > Fill Center: True
             *   2. Mask (in object): Is used to clip the content of the message, which will grow very long.
             *      Recommended settings:
             *        > Show Mask Graphic: True (otherwise the background image in the Image component
             *          will not be seen)
             *   3. This component has also recommended settings since it inherits from ScrollRect:
             *      > Viewport: None
             *      > Horizontal Scrollbar: None
             *      > Vertical Scrollbar: None
             *   4. InteractiveMessageContent (in child). It is already documented.
             * 
             * This behaviour exposes a method to execute its magic: PromptMessages(Prompt[] msgs) that
             *   returns a Coroutine. This method should not be used on its own, but as part of the implementation
             *   of an Interactor's Input() and RunInteraction() methods, since those methods belong to the
             *   main interaction's lifecycle.
             * 
             * For more information, see PromptMessages method in this class, and the relevant methods in the
             *   Interactor class.
             * 
             * A public property is exposed: QuickTextMovement. This serves to accelerate text display. You can
             *   safely set or clear this flag, but the ideal behaviour is that you set or clear this flag
             *   depending on whether a button is pressed or not (however it is a matter of taste).
             */
            [RequireComponent(typeof(UnityEngine.UI.Mask))]
            [RequireComponent(typeof(UnityEngine.UI.Image))]
            public class InteractiveMessage : UnityEngine.UI.ScrollRect
            {
                /**
                 * A message setting to send to the display. It has 3 members:
                 * 
                 * message: text to display.
                 * clearBeforeStart: whether the display should be erased before showing this text.
                 * delayAfterEnd: whether we add an additional timeout so the user can end reading
                 *   the message before it gets erased or another message continues.
                 */
                public class Prompt
                {
                    public readonly string message;
                    public readonly bool clearBeforeStart;
                    public readonly bool delayAfterEnd;
                    public Prompt(string msg, bool clear = true, bool delay = true)
                    {
                        message = msg;
                        clearBeforeStart = clear;
                        delayAfterEnd = delay;
                    }
                }

                private UnityEngine.UI.Mask mask;

                /**
                 * A big part of the magic is delegated to this component, which actually performs the
                 *   display operation for each message.
                 */
                private InteractiveMessageContent messageContent;

                /**
                 * This property was described above. The actual implementation is in the underlying
                 *   InteractiveMessageContent object.
                 */
                public bool QuickTextMovement
                {
                    get { return messageContent.QuickTextMovement; }
                    set { messageContent.QuickTextMovement = value; }
                }

                /**
                 * When starting, the inner message content will be centered horizontally. The fact that
                 *   this component inherits ScrollRect helps us to clip it and align it vertically
                 *   (see the Update method for more details).
                 */
                protected override void Start()
                {
                    base.Start();
                    mask = GetComponent<UnityEngine.UI.Mask>();
                    messageContent = Layout.RequireComponentInChildren<InteractiveMessageContent>(this.gameObject);
                    RectTransform me = GetComponent<RectTransform>();
                    content = messageContent.GetComponent<RectTransform>();
                    float myWidth = me.sizeDelta.x;
                    float itsWidth = content.sizeDelta.x;
                    content.localPosition = new Vector2((myWidth - itsWidth) / 2, 0);
                    content.sizeDelta = new Vector2(itsWidth, content.sizeDelta.y);
                }

                /**
                 * Starts a coroutine iterating over the messages and delegating the display behaviour to the
                 *   message content. As stated above, this method is public but should only be invoked from
                 *   inner methods of Interactor class and subclasses.
                 */
                public Coroutine PromptMessages(Prompt[] prompt)
                {
                    return StartCoroutine(MessagesPrompter(prompt));
                }

                private IEnumerator MessagesPrompter(Prompt[] prompt)
                {
                    foreach (Prompt prompted in prompt)
                    {
                        yield return messageContent.StartTextMessage(prompted.message, prompted.clearBeforeStart, prompted.delayAfterEnd);
                    }
                }

                /**
                 * Since this is a vertical scrolling component, this will happen every frame:
                 *   > No horizontal scroll will occur.
                 *   > Vertical scroll will occur.
                 *   > The vertical position will always be 0 (i.e. always scrolling down).
                 */
                void Update()
                {
                    horizontal = false;
                    vertical = true;
                    if (content)
                    {
                        verticalNormalizedPosition = 0;
                    }
                }
            }
        }
    }
}