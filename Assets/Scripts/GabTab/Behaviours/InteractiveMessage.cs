using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Support.Utils;

namespace GabTab
{
    namespace Behaviours
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
        [RequireComponent(typeof(Mask))]
        [RequireComponent(typeof(Image))]
        public class InteractiveMessage : ScrollRect
        {
            /**
             * A message-displaying instruction to run in the display.
             */
            public abstract class Prompt
            {
                public abstract IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null);
            }

            /**
             * This instruction sends a newline to the display.
             */
            public class NewlinePrompt : Prompt
            {
                public readonly bool OnlyIfSignificant;
                public NewlinePrompt(bool onlyIfSignificant)
                {
                    OnlyIfSignificant = onlyIfSignificant;
                }

                public override IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null)
                {
                    if (lastChar != null && lastChar != '\n' || !OnlyIfSignificant)
                    {
                        builder.Append('\n');
                        content.GetComponent<Text>().text = builder.ToString();
                        yield return content.CharacterWaiterCoroutine();
                    }
                }
            }

            /**
             * This instruction clears the display.
             */
            public class ClearPrompt : Prompt
            {
                public override IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null)
                {
                    builder.Remove(0, builder.Length);
                    content.GetComponent<Text>().text = "";
                    yield return content.CharacterWaiterCoroutine();
                }
            }

            /**
             * This instruction sends text to the display.
             */
            public class MessagePrompt : Prompt
            {
                public readonly string Message;
                public MessagePrompt(string message)
                {
                    Message = message;
                }

                public override IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null)
                {
                    Text textComponent = content.GetComponent<Text>();
                    foreach (char c in Message)
                    {
                        builder.Append(c);
                        textComponent.text = builder.ToString();
                        yield return content.CharacterWaiterCoroutine();
                    }
                }
            }

            /**
             * This instruction waits a specific time in the display before doing more stuff there.
             * The slow time will be specified, and the quick time calculated by dividing it on 10,
             *   or will be taken, as default, from the content settings.
             */
            public class WaiterPrompt : Prompt
            {
                public readonly float? SecondsToWait;
                public WaiterPrompt(float? secondsToWait)
                {
                    SecondsToWait = secondsToWait;
                }

                public override IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null)
                {
                    yield return content.ExplicitWaiterCoroutine(SecondsToWait);
                }
            }

            /**
             * You can chain many calls like this:
             *   Prompt[] all = PromptBuilder.Clear().Write("Hello world.").Write("How are you?").Newline().Write("Have a nice day").End();
             */
            public class PromptBuilder
            {
                private System.Collections.Generic.List<Prompt> list = new System.Collections.Generic.List<Prompt>();

                public PromptBuilder Wait(float? seconds = null)
                {
                    list.Add(new WaiterPrompt(seconds));
                    return this;
                }

                public PromptBuilder Write(string message)
                {
                    list.Add(new MessagePrompt(message));
                    return this;
                }

                public PromptBuilder NewlinePrompt(bool onlyIfSignificant)
                {
                    list.Add(new NewlinePrompt(onlyIfSignificant));
                    return this;
                }

                public PromptBuilder Clear()
                {
                    list.Add(new ClearPrompt());
                    return this;
                }

                public Prompt[] End()
                {
                    return list.ToArray();
                }
            }

            private Mask mask;
            private RectTransform me;

            /**
             * A big part of the magic is delegated to this component, which actually performs the
             *   display operation for each message.
             */
            [SerializeField]
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

            protected override void Awake()
            {
                base.Awake();
                mask = GetComponent<Mask>();
                me = GetComponent<RectTransform>();
                content = messageContent.GetComponent<RectTransform>();
            }

            /**
             * When starting, the inner message content will be centered horizontally. The fact that
             *   this component inherits ScrollRect helps us to clip it and align it vertically
             *   (see the Update method for more details).
             */
            protected override void Start()
            {
                base.Start();
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
                Text textComponent = messageContent.GetComponent<Text>();
                StringBuilder builder = new StringBuilder(textComponent.text);

                foreach (Prompt prompted in prompt)
                {
                    string currentText = textComponent.text;
                    yield return StartCoroutine(prompted.ToDisplay(messageContent, builder, currentText != "" ? (char?)currentText[currentText.Length - 1] : null));
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