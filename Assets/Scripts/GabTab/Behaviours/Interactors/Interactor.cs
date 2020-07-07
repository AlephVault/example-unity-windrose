using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /// <summary>
            ///   An interactor is an UI component that has a special method:
            ///     <see cref="RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/>.
            ///     Yow will make use of standard subclasses, or create your own.
            ///   
            ///   Interactors make sense only in the scope of an <see cref="InteractiveInterface"/>.
            ///     Interactors are registered (in the editor) inside an <see cref="InteractorsManager"/> instance,
            ///     and accessed/used as in the example given in
            ///     <see cref="InteractiveInterface.RunInteraction(Func{InteractorsManager, InteractiveMessage, Task})"/>.
            /// </summary>
            /// <remarks>
            ///   The only relevant method of this class is <see cref="RunInteraction(InteractiveMessage, InteractiveMessage.Prompt[])"/>,
            ///     which invokes <see cref="Input"/>, that must be implemented.
            /// </remarks>
            [RequireComponent(typeof(Hideable))]
            public abstract class Interactor : MonoBehaviour
            {
                private bool interactionRunning = false;
                private bool interactionDisplaying = false;
                private Hideable hideable;

                [SerializeField]
                private uint newlinesToAddWhenShowing = 0;

                protected void Awake()
                {
                    hideable = GetComponent<Hideable>();
                }

                /// <summary>
                ///   Runs the interaction. The first part is displawing the texts, letter by letter. After that,
                ///     the result of <see cref="Input"/> (which is an enumerator) is run. 
                /// </summary>
                /// <param name="interactiveMessage">
                ///   An instance of <see cref="InteractiveInterface"/>, first referenced by the instance of
                ///     <see cref="InteractiveInterface"/> that ultimately triggered this interaction. 
                /// </param>
                /// <param name="prompt">
                ///   You will usually build this array using a <see cref="InteractiveMessage.PromptBuilder"/> as
                ///     you can read in the example section of <see cref="InteractiveInterface"/>. 
                /// </param>
                /// <returns>A task to be run.</returns>
                public Task RunInteraction(InteractiveMessage interactiveMessage, InteractiveMessage.Prompt[] prompt)
                {
                    return WrappedInteraction(interactiveMessage, prompt);
                }

                private async Task WrappedInteraction(InteractiveMessage interactiveMessage, InteractiveMessage.Prompt[] prompt)
                {
                    if (interactionRunning)
                    {
                        throw new Types.Exception("Cannot run the interaction: A previous interaction is already running");
                    }

                    interactionRunning = true;
                    // We may add extra spaces to the last message to be rendered.
                    // This helps us allocating more visual space so the displayed
                    //   interface does not hide the text in the message.
                    int length = prompt.Length;
                    if (length > 0 && (prompt[length - 1] is InteractiveMessage.MessagePrompt))
                    {
                        // Adds newlines to the last prompt.
                        string extraSpaces = new String('\n', (int)newlinesToAddWhenShowing);
                        InteractiveMessage.MessagePrompt lastPrompt = prompt[length - 1] as InteractiveMessage.MessagePrompt;
                        prompt[length - 1] = new InteractiveMessage.MessagePrompt(lastPrompt.Message + extraSpaces);
                    }
                    await interactiveMessage.PromptMessages(prompt);
                    interactionDisplaying = true;
                    await Input(interactiveMessage);
                    interactionDisplaying = false;
                    interactionRunning = false;
                }

                void Update()
                {
                    hideable.Hidden = !interactionDisplaying;
                }

                /// <summary>
                ///   Please override this method to return an enumerable (usually using <c>yield</c> keyword) that
                ///     will be run inside a <c>Coroutine</c> that will interact with the UI.
                /// </summary>
                /// <param name="interactiveMessage">
                ///   An instance of <see cref="InteractiveInterface"/>, first referenced by the instance of
                ///     <see cref="InteractiveInterface"/> that ultimately triggered this interaction. 
                /// </param>
                /// <returns>A task to be run asynchronously.</returns>
                protected abstract Task Input(InteractiveMessage interactiveMessage);
            }
        }
    }
}
