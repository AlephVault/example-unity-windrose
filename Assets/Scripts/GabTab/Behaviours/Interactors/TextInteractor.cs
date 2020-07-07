using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GMM.Utils;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /// <summary>
            ///   This interactor is an input text. It will also need two buttons to
            ///     be told to work as they do in <see cref="ButtonsInteractor"/>, but
            ///     this time only two buttons are expected: one for Continue, and one
            ///     for Cancel.
            /// </summary>
            /// <remarks>
            ///   <para>
            ///     The interaction will end when either of the buttons is clicked. If
            ///       the text is not allowed (due to being empty, and empty value not
            ///       being allowed), the continue button will be disabled.
            ///   </para>
            ///   
            ///   <para>
            ///     At the end of the interaction, you will have public access to two
            ///       members: <see cref="Result"/> which tells whether the input was
            ///       continued (<c>true</c>) or cancelled (<c>false</c>), and <see cref="Content"/>
            ///       which contains the input text.
            ///   </para>
            ///   
            ///   <para>
            ///     You will have to configure all the required components in the editor:
            ///       two buttons and a text input. Also you may like configuring whether
            ///       the text will be spaces-trimmed and whether you accept an empty input.
            ///   </para>
            /// </remarks>
            [RequireComponent(typeof(Image))]
            public class TextInteractor : Interactor
            {
                /// <summary>
                ///   Set this member in the Inspector to a button you will use as continue
                ///     button.
                /// </summary>
                [SerializeField]
                private Button continueButton;

                /// <summary>
                ///   Set this member in the Inspector to a button you will use as cancel
                ///     button.
                /// </summary>
                [SerializeField]
                private Button cancelButton;

                /// <summary>
                ///   Set this member in the Inspector to a text input.
                /// </summary>
                [SerializeField]
                private InputField textInput;

                /// <summary>
                ///   You may want to set this one to true in the Inspector if you want to
                ///     trim spaces at start and end of your string. This will also count
                ///     when allowing empty or not: a string purely consisting on spaces
                ///     will not be considered valid input.
                /// </summary>
                [SerializeField]
                private bool trimText = true;

                /// <summary>
                ///   You may want to set this one to true in the Inspector. If you do,
                ///     empty strings will not be allowed, to the point that the continue
                ///     button will be disabled if an empty string is on the input.
                /// </summary>
                [SerializeField]
                private bool allowEmptyText = false;

                /// <summary>
                ///   You will get the input string (perhaps trimmed) after running
                ///     <see cref="Interactor.RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/>. 
                /// </summary>
                public bool? Result { get; private set; }

                /// <summary>
                ///   You will get whether the input was continued after running
                ///     <see cref="Interactor.RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/>. 
                /// </summary>
                public string Content { get; private set; }

                /// <summary>
                ///   You can get/set the placeholder of the text input before running
                ///     <see cref="Interactor.RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/> (or after).
                /// </summary>
                public string PlaceholderPrompt { get { return ((Text)textInput.placeholder).text; } set { ((Text)textInput.placeholder).text = value; } }

                /// <summary>
                ///   <para>
                ///     You can get/set the raw input text of the inner input UI element
                ///       before running <see cref="Interactor.RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/>.
                ///   </para>
                ///   <para>
                ///     Useful to set a default value.
                ///   </para>
                /// </summary>
                public string RawInputText { get { return textInput.text; } set { textInput.text = value; } }

                /**
                 * Sets the click events for the buttons in the role of cancel and continue.
                 * Sets the submission event for the text input (it will act as "continue").
                 * Sets the change event for the text, to disable the continue button if text is empty
                 *   and allowEmptyText is false.
                 */
                void Start()
                {
                    Result = null;
                    Content = null;

                    if (continueButton != null)
                    {
                        continueButton.onClick.AddListener(delegate () {
                            string text = GetCurrentTextFromInputField();
                            if (IsTextAllowed(text))
                            {
                                Content = textInput.text;
                                Result = true;
                            }
                        });
                    }

                    if (cancelButton != null)
                    {
                        cancelButton.onClick.AddListener(delegate () {
                            Content = null;
                            Result = false;
                        });
                    }

                    if (textInput)
                    {
                        textInput.onEndEdit.AddListener(delegate (string inputText) {
                            string text = TrimText(inputText);
                            if (IsTextAllowed(text))
                            {
                                Content = text;
                                Result = true;
                            }
                        });
                        if (continueButton != null)
                        {
                            textInput.onValueChanged.AddListener(delegate (string inputText)
                            {
                                string text = TrimText(inputText);
                                continueButton.interactable = IsTextAllowed(text);
                            });
                            textInput.onValueChanged.Invoke(textInput.text);
                        }
                    }
                }

                /**
                 * Trims the text if `trimText` is specified. Otherwise, returns the text unchanged.
                 */
                private string TrimText(string text)
                {
                    return trimText ? text.Trim() : text;
                }

                /**
                 * Gets the current text in the input field and trims it (if `trimText` is specified).
                 */
                private string GetCurrentTextFromInputField()
                {
                    return (textInput.text) != null ? TrimText(textInput.text) : null;
                }

                /**
                 * Tells whether the input text is allowed. This involves:
                 * 1. The text is not null.
                 * 2. The text is not empty, or empty text is allowed.
                 */
                private bool IsTextAllowed(string text)
                {
                    return (text != null) && (text != "" || allowEmptyText);
                }

                /// <summary>
                ///   This implementation will clear any former input text and wait until a result is
                ///     available.
                /// </summary>
                /// <param name="interactiveMessage">
                ///   An instance of <see cref="InteractiveInterface"/>, first referenced by the instance of
                ///     <see cref="InteractiveInterface"/> that ultimately triggered this interaction. 
                /// </param>
                /// <returns>An enumerator to be run inside a coroutine.</returns>
                protected override async Task Input(InteractiveMessage interactiveMessage)
                {
                    Result = null;
                    Content = null;
                    while (Result == null)
                    {
                        await Tasks.Blink();
                    }
                }
            }
        }
    }
}
