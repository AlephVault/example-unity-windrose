using System;
using System.Collections;
using UnityEngine;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /**
             * This class serves as a base for an interactor.
             * An interactor is, at a glance, just an UI/Canvas game object (which will
             *   have many children game objects serving as buttons and other UI elements)
             *   that will be integrated with the lifecycle of an interactive interface
             *   to actually communicate to the user.
             * 
             * An interactor is a component in a game object. It is only useful when
             *   the game object belongs to a hierarchy that starts with an interactive
             *   interface. While this is not strictly required, is recommended, so this
             *   object's life (and scaling!) is tied to the interactive interface's
             *   object's life (also, and scaling!).
             * 
             * A typical interaction will have one or more texts being displayed in
             *   an interactive message. After the messages are all shown, the interactive
             *   part (e.g. buttons) will be displayed. This will all be handled by the
             *   interactive interface.
             * 
             * This component becomes relevant when talking about the interactive parts
             *   being displayed.
             *   
             * This behaviour exposes this magic through the RunInteraction method. However, such
             *   method should not be called directly but inside an enumerator/generator funcion passed
             *   to the RunInteraction call on the ancestor InteractiveInterface component.
             * 
             * PLEASE TAKE A LOOK to the implementation of RunInteraction method, since that part is the
             *   core and most important thing you'll ever need to know (aside from having read the
             *   details about calling RunInteraction in the InteractiveInterface class).
             */
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

                /**
                 * Running an interactor involves prompting an initial message (or multiple initial messages, sequentially)
                 *   and then expecting an input from the user.
                 * 
                 * A preparer function can be used to prepare the component status before even prompting the initial messages.
                 * If specified, it will be executed, allowing the user to initialize component properties before the
                 *   interaction starts.
                 * 
                 * The actual implementation is in the Input() method, which must be overriden by the user.
                 * 
                 * ===== This is the most important function you will need to read about. =====
                 * 
                 * Most of the times you will invoke an interaction in the following steps:
                 *   1. You create an instance of InteractiveInterface. Most of the times, you will have a prefab.
                 *      Such instance will have children objects which will, in turn, be prefabs most of the times.
                 *      Those children objects will have Interactor components and will be, perhaps, buttons or
                 *        text inputs or even more complicated cases like nested menus or inventory picking screens.
                 *   2. You go to the InteractorsManager component, and register those components. This will occur
                 *        by using the Properties Editor for this InteractorsManager, which has a dictionary
                 *        property. You will add entries and, for each, set a key (string) and a value (pick an
                 *        Interactor component from the scene, which is recommended to be in the hierarchy under
                 *        the game object having the InteractiveInterface component, although not required).
                 *   3. When you invoke anInteractiveInterface.RunInteraction (inside a custom, external, behaviour)
                 *        you will pass a method reference (remember that C# does not allow anonymous functions to
                 *        make use of `yield` keywords, so you will have to workaround that, e.g., by passing a
                 *        reference to a local method declared in the same custom, external, behaviour you are
                 *        declaring for this purpose) with this signature:
                 *        
                 *        IEnumerator SomeMethod(GabTab.Behaviours.Interactors.InteractorsManager manager,
                 *                               GabTab.Behaviours.InteractiveMessage interactiveMessage)
                 *   4. You will execute a series (perhaps branched, iterated, and anything that you could achieve
                 *        through structured programming) of interactions with the user.
                 *      An interaction will consist on:
                 *        > Finding wich registered interactor will perform the interaction:
                 *          GabTab.Behaviours.Interactors.Interactor continueButton = manager["continue"];
                 *        > Choosing the messages to send:
                 *          GabTab.Behaviours.InteractiveMessage.Prompt[] messages = new GabTab.Behaviours.InteractiveMessage.Prompt[] {
                 *            new GabTab.Behaviours.InteractiveMessage.Prompt("Welcome, stranger, I am Professor Oak, from Palette Town", true),
                 *            new GabTab.Behaviours.InteractiveMessage.Prompt("You are about to start the best journey of your life", false)
                 *          };
                 *          // This setting of messages will show, when executed, two messages -the 2nd after the 1st, without clearing the display-
                 *          //   before the input component appears to the user.
                 *        > Run the magic:
                 *          continueButton.Buttons[0].Text = "End";
                 *          continueButton.RunInteraction(interactiveMessage, messages);
                 *   5. Remember adding the line `using GabTab.Behaviours;` to avoid doing the mess I did in the example I wrote.
                 */
                public Coroutine RunInteraction(InteractiveMessage interactiveMessage, InteractiveMessage.Prompt[] prompt)
                {
                    return StartCoroutine(WrappedInteraction(interactiveMessage, prompt));
                }

                private IEnumerator WrappedInteraction(InteractiveMessage interactiveMessage, InteractiveMessage.Prompt[] prompt)
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
                    yield return interactiveMessage.PromptMessages(prompt);
                    interactionDisplaying = true;
                    yield return StartCoroutine(Input(interactiveMessage));
                    interactionDisplaying = false;
                    interactionRunning = false;
                }

                void Update()
                {
                    hideable.Hidden = !interactionDisplaying;
                }

                protected abstract IEnumerator Input(InteractiveMessage interactiveMessage);
            }
        }
    }
}
