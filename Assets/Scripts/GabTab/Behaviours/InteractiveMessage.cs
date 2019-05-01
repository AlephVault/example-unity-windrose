using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Support.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GabTab
{
    namespace Behaviours
    {
        /**
         *  It requires two components
         *   in the same object, and one component in a child. The components go like this:
         *   3. This component has also recommended settings since it inherits from ScrollRect:
         *      > Viewport: None
         *      > Horizontal Scrollbar: None
         *      > Vertical Scrollbar: None
         *   4. InteractiveMessageContent (in child). It is already documented.
         */
        /// <summary>
        ///   This component handles the message being displayed to the user. It is intended to be a child of
        ///     an <see cref="InteractiveInterface"> component.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This class provides features to display text in an animated fashion instead of suddenly the entire
        ///       bulk. An example is provided in <see cref="InteractiveInterface"/>.
        ///   </para>
        ///   <para>
        ///     Pay special attention to <see cref="PromptMessages(Prompt[])"/> since it is the most interesting
        ///       method of this class, aside from the inner classes <see cref="PromptBuilder"/> and
        ///       <see cref="Prompt"/>. 
        ///   </para>
        ///   <para>
        ///     This component should:
        ///     <list type="bullet">
        ///       <item>
        ///         <description>
        ///           Configure its <see cref="Image"/> dependency appropriately:
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Image Type</term>
        ///               <description>
        ///                 Slice
        ///                 <list type="bullet">
        ///                   <item>
        ///                     <term>Fill Center</term>
        ///                     <description>Set it to <c>true</c></description>
        ///                   </item>
        ///                 </list>
        ///               </description>
        ///             </item>
        ///           </list>
        ///         </description>
        ///       </item>
        ///       <item>
        ///         <description>
        ///           Configure its <see cref="Mask"/> dependency appropriately. Since the mask
        ///             is used to clip the text content while it scrolls, it will be needed
        ///             here as well and appropriately configure:
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Show Mask Graphic</term>
        ///               <description>
        ///                 Set it to <c>true</c> or the background image for the text content
        ///                 will not appear.
        ///               </description>
        ///             </item>
        ///           </list> 
        ///         </description>
        ///       </item>
        ///       <item>
        ///         <description>
        ///           Configure this component, unless you want a different behaviour, like this:
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Viewport</term>
        ///               <description>None.</description>
        ///             </item>
        ///             <item>
        ///               <term>Vertical scrollbar</term>
        ///               <description>None.</description>
        ///             </item>
        ///             <item>
        ///               <term>Horizontal scrollbar</term>
        ///               <description>None.</description>
        ///             </item>
        ///             <item>
        ///               <term>Message Content</term>
        ///               <description>A valid <see cref="messageContent"/> instance. Usually among its direct children.</description>
        ///             </item>
        ///           </list>
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        /// </remarks>
        [RequireComponent(typeof(Mask))]
        [RequireComponent(typeof(Image))]
        public class InteractiveMessage : ScrollRect
        {
            /// <summary>
            ///   This class is the base for any prompt type from the <see cref="InteractiveMessage"/> to the user.
            /// </summary>
            /// <seealso cref="NewlinePrompt"/>
            /// <seealso cref="ClearPrompt"/>
            /// <seealso cref="MessagePrompt"/>
            /// <seealso cref="WaiterPrompt"/>
            public abstract class Prompt
            {
                public abstract IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null);
            }

            /// <summary>
            ///   This prompt adds a newline to the message display. Created via <see cref="PromptBuilder.NewlinePrompt(bool)"/>.
            /// </summary>
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

            /// <summary>
            ///   This prompt clears the display. Created via <see cref="PromptBuilder.Clear"/>.
            /// </summary>
            public class ClearPrompt : Prompt
            {
                public override IEnumerator ToDisplay(InteractiveMessageContent content, StringBuilder builder, char? lastChar = null)
                {
                    builder.Remove(0, builder.Length);
                    content.GetComponent<Text>().text = "";
                    yield return content.CharacterWaiterCoroutine();
                }
            }

            /// <summary>
            ///   This prompt sends a string message, letter by letter, to the message display. Created via <see cref="PromptBuilder.Write(string)"/>. 
            /// </summary>
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

            /// <summary>
            ///   This class just waits some time before doing anything else with the display. Created via <see cref="PromptBuilder.Wait(float?)"/>.
            /// </summary>
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

            /// <summary>
            ///   A prompt builder lets you build an array of <see cref="Prompt"/> elements to be passed to <see cref="Interactors.Interactor.RunInteraction(InteractiveMessage, Prompt[])"/>. 
            /// </summary>
            /// <remarks>
            ///   It starts containing an empty array and then you can call the methods of this class
            ///     to add elements to the array following a fluent or chained pattern. To obtain the
            ///     final array you simply call <see cref="End"/> method when done adding stuff. 
            /// </remarks>
            public class PromptBuilder
            {
                private System.Collections.Generic.List<Prompt> list = new System.Collections.Generic.List<Prompt>();

                /// <summary>
                ///   Creates a prompt that produces a timeout.
                /// </summary>
                /// <param name="seconds">Amount of seconds to wait. If absent, will be taken according to <see cref="InteractiveMessageContent"/></param>
                /// <returns>The same <see cref="PromptBuilder"/> instance.</returns>
                public PromptBuilder Wait(float? seconds = null)
                {
                    list.Add(new WaiterPrompt(seconds));
                    return this;
                }

                /// <summary>
                ///   Creates a prompt that sends a string to the display.
                /// </summary>
                /// <param name="message">The message to display letter by letter.</param>
                /// <returns>The same <see cref="PromptBuilder"/> instance.</returns>
                public PromptBuilder Write(string message)
                {
                    list.Add(new MessagePrompt(message));
                    return this;
                }

                /// <summary>
                ///   Creates a prompt that puts a newline on the display. If the argument is true, it only puts the newline if there was not a former newline at the
                ///     end of the current content.
                /// </summary>
                /// <param name="onlyIfSignificant">
                ///   Wether only add the newline if the displayed string did not have a newline at the end, or put the newline anyway.
                /// </param>
                /// <returns>The same <see cref="PromptBuilder"/> instance.</returns>
                public PromptBuilder NewlinePrompt(bool onlyIfSignificant)
                {
                    list.Add(new NewlinePrompt(onlyIfSignificant));
                    return this;
                }

                /// <summary>
                ///   Creates a prompt that clears the display.
                /// </summary>
                /// <returns>The same <see cref="PromptBuilder"/> instance.</returns>
                public PromptBuilder Clear()
                {
                    list.Add(new ClearPrompt());
                    return this;
                }

                /// <summary>
                ///   Returns the list of created prompts.
                /// </summary>
                /// <returns>An array of <see cref="Prompt"/> objects, ready to pass as second argument to <see cref="Interactors.Interactor.RunInteraction(InteractiveMessage, Prompt[])"/>.</returns>
                public Prompt[] End()
                {
                    return list.ToArray();
                }
            }

            private RectTransform me;

            /// <summary>
            ///   You must set this reference to a valid, non-null, <see cref="InteractiveMessageContent"/> component.
            /// </summary>
            [SerializeField]
            private InteractiveMessageContent messageContent;

            /// <summary>
            ///   By setting this to <c>true</c> the prompted text will appear faster. See <see cref="InteractiveMessageContent"/>
            ///     for more details on that topic. However, you can manage this public member AS and WHEN you like.
            /// </summary>
            public bool QuickTextMovement
            {
                get { return messageContent.QuickTextMovement; }
                set { messageContent.QuickTextMovement = value; }
            }

            protected override void Awake()
            {
                base.Awake();
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

            /// <summary>
            ///   Taking an array of <see cref="Prompt"/>, it executes them sequentially: one
            ///     prompt will not run until previous prompts have runned.
            /// </summary>
            /// <param name="prompt">The list of <see cref="Prompt"/> objects to execute.</param>
            /// <returns>The just-started coroutine.</returns>
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

#if UNITY_EDITOR
        [CustomEditor(typeof(InteractiveMessage), true)]
        [CanEditMultipleObjects]
        public class InteractiveMessageEditor : Editor
        {
            SerializedProperty m_Content;

            protected virtual void OnEnable()
            {
                m_Content = serializedObject.FindProperty("messageContent");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(m_Content);

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}