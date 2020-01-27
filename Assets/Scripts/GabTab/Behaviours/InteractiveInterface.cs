using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using GMM.Utils;

namespace GabTab
{
    namespace Behaviours
    {
        /// <summary>
        ///   This class provides an interface to interact via UI with the game, with stuff like
        ///     messaging and custom components.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The interface is quite like those in Pokemon: one text where the game or NPCs tell
        ///       you everything, and also UI elements to interact with (e.g. buttons set, text input,
        ///       or list of elements). It require several components:
        ///   </para>
        ///   <para>
        ///     This behaviour exposes a method to run this magic: RunInteraction(Interaction func).
        ///     The interaction must be a generator function(remember that C# does not allow anonymous
        ///       functions working as generators). For more information, see RunInteraction method's
        ///       documentation in this class.
        ///   </para>
        ///   <para>
        ///     This component requires an <see cref="UnityEngine.UI.Image" />, which will hold the background.
        ///     The recommended settings for the image is:
        ///     <list type="bullet">
        ///       <item>
        ///         <term>Image Type</term>
        ///         <description>
        ///           Slice
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Fill Center</term>
        ///               <description>Set it to <c>true</c></description>
        ///             </item>
        ///           </list>
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        ///   <para>
        ///     This component requires an <see cref="Hideable" />, which will hide the interface when not being
        ///       used.
        ///   </para>
        ///   <para>
        ///     This component requires an <see cref="Interactors.InteractorsManager" />, which will manage the available
        ///       components of this interactive interface.
        ///   </para>
        ///   <para>
        ///     Assigned via editor, an object with <see cref="InteractiveMessage" /> must be present. Usually, this object
        ///       will exist among the direct children of this component, but it is not mandatory.
        ///   </para>
        /// </remarks>
        /// <seealso cref="Hideable"/>
        /// <seealso cref="Interactors.InteractorsManager"/>
        /// <seealso cref="InteractiveMessage"/>
        [RequireComponent(typeof(UnityEngine.UI.Image))]
        [RequireComponent(typeof(Hideable))]
        [RequireComponent(typeof(Interactors.InteractorsManager))]
        public class InteractiveInterface : MonoBehaviour
        {
            /// <summary>
            ///   You must set this reference to a valid, non-null, <see cref="InteractiveMessage"/> component.
            /// </summary>
            [SerializeField]
            private InteractiveMessage interactiveMessage;
            private Interactors.InteractorsManager interactorsManager;
            public bool IsRunningAnInteraction { get; private set; }
            private Hideable hideable;

            /// <summary>
            ///   This event triggers before interactions via <see cref="RunInteraction" />.
            /// </summary>
            public readonly UnityEvent beforeRunningInteraction = new UnityEvent();

            /// <summary>
            ///   This event triggers after interactions via <see cref="RunInteraction" />.
            /// </summary>
            public readonly UnityEvent afterRunningInteraction = new UnityEvent();

            void Awake()
            {
                interactorsManager = GetComponent<Interactors.InteractorsManager>();
                hideable = GetComponent<Hideable>();
            }

            void Update()
            {
                hideable.Hidden = !IsRunningAnInteraction;
            }

            /// <summary>
            ///   You can change this property at any time to tell whether the text should
            ///   appear quickly (<c>true</c>) or slow (<c>false</c>).
            /// </summary>
            /// <seealso cref="InteractiveMessageContent.QuickTextMovement"/>
            public bool QuickTextMovement
            {
                get { return interactiveMessage.QuickTextMovement; }
                set { interactiveMessage.QuickTextMovement = value; }
            }

            /// <summary>
            ///   This method runs an interaction procedure.
            /// </summary>
            /// <remarks>
            ///   <para>
            ///     An interaction procedure is a function that returns an IEnumerator (usually, this is a generator function - those
            ///       functions with <c>yield return</c> or <c>yield break</c> statements). Remember: Anonymous functions cannot be
            ///       used as generators.
            ///   </para>
            ///   <para>
            ///     Such function will yield steps of flow. See the example to get a glance of how does this work.
            ///   </para>
            ///   <para>
            ///     Events <see cref="beforeRunningInteraction" /> and <see cref="afterRunningInteraction" /> will be trigger respectively
            ///       to wrap each passed interaction.
            ///   </para>
            /// </remarks>
            /// <param name="runnable">
            ///   The function to pass arguments. The function will accept an <see cref="Interactors.InteractorsManager" /> as first argument
            ///     and an <see cref="InteractiveMessage"> as second argument.
            /// </param>
            /// <example><![CDATA[
            ///   class MyClass {
            ///       void Awake() {
            ///           interactiveInterface = GetComponent<InteractiveInterface>();
            ///       }
            ///   
            ///       IEnumerator SampleInteraction(Interactors.InteractorsManager manager, InteractiveMessage message) {
            ///           ButtonsInteractor yesnoInteractor = (ButtonsInteractor)manager["yesno-input"];
            ///           NullInteractor nullInteractor = (NullInteractor)manager["null"];
            ///           yield return yesnoInteractor.RunInteraction(message, new InteractiveMessage.PromptBuilder().Clear().Write("Do you like GabTab?").End());
            ///           if (yesnoInteractor.Result == "yes") {
            ///               yield return nullInteractor.RunInteraction(message, new InteractiveMessage.PromptBuilder().Clear().Write("Cool!").End());
            ///           } else {
            ///               yield return nullInteractor.RunInteraction(message, new InteractiveMessage.PromptBuilder().Clear().Write("Ooooh :(").End());
            ///           }
            ///       }
            ///   
            ///       void RunSampleInteraction() {
            ///           interactiveInterface.RunInteraction(SampleInteraction);
            ///       }
            ///   }
            /// ]]></example>
            /// <seealso cref="Interactors.InteractorsManager"/>
            /// <seealso cref="InteractiveMessage"/>
            /// <seealso cref="Interactors.Interactor"/> 
            public Coroutine RunInteraction(Func<Interactors.InteractorsManager, InteractiveMessage, IEnumerator> runnable)
            {
                return StartCoroutine(WrappedInteraction(runnable(interactorsManager, interactiveMessage)));
            }

            private IEnumerator WrappedInteraction(IEnumerator innerInteraction)
            {
                if (IsRunningAnInteraction)
                {
                    throw new Types.Exception("Cannot run the interaction: A previous interaction is already running");
                }
                IsRunningAnInteraction = true;
                beforeRunningInteraction.Invoke();
                yield return StartCoroutine(innerInteraction);
                afterRunningInteraction.Invoke();
                IsRunningAnInteraction = false;
            }
        }
    }
}
