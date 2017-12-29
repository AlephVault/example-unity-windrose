using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            namespace Interactors
            {
                /**
                 *
                 */
                [RequireComponent(typeof(Image))]
                public class TextInteractor : Interactor
                {
                    /**
                     * The first thing we need to define, is a dictionary of buttons to register.
                     * Such dictionary will be like (Button) => (string).
                     */

                    [SerializeField]
                    private Button continueButton;

                    [SerializeField]
                    private Button cancelButton;

                    [SerializeField]
                    private InputField textInput;

                    [SerializeField]
                    private bool trimText = true;

                    [SerializeField]
                    private bool allowEmptyText = false;

                    /**
                     * When interacting with the component, these two properties
                     *   should be considered when retrieving the result of the
                     *   interaction.
                     */
                    public bool? Result { get; private set; }
                    public string Content { get; private set; }

                    /**
                     * Just a quick way to set the prompt on the textbox.
                     */
                    public string PlaceholderPrompt { get { return ((Text)textInput.placeholder).text; } set { ((Text)textInput.placeholder).text = value; } }

                    /**
                     * Just a quick way to set the content on the textbox.
                     */
                    public string RawInputText { get { return textInput.text; } set { textInput.text = value; } }

                    /**
                     * Sets the click events for the buttons in the role of cancel and continue.
                     * Sets the submission event for the text input (it will act as "continue").
                     * Sets the change event for the text, to disable the continue button if text is empty
                     *   and allowEmptyText is false.
                     */
                    void Start()
                    {
                        base.Start();
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
                                textInput.onValueChange.AddListener(delegate (string inputText)
                                {
                                    string text = TrimText(inputText);
                                    continueButton.interactable = IsTextAllowed(text);
                                });
                                textInput.onValueChange.Invoke(textInput.text);
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

                    /**
                     * The implementation of this input does three important stuff:
                     * 1. Sets the result in null, and content in null.
                     * 2. Waits until the result is no longer null.
                     */
                    protected override IEnumerator Input(InteractiveMessage interactiveMessage)
                    {
                        Result = null;
                        Content = null;
                        yield return new WaitWhile(delegate() { return Result == null; });
                    }
                }
            }
        }
    }
}
