using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Support.Types;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /// <summary>
            ///   This interactor registers a list of buttons, each under a key, that
            ///     will be available to be run. This UI element will wait until one
            ///     of the registered buttons is pressed.
            /// </summary>
            [RequireComponent(typeof(Image))]
            public class ButtonsInteractor : Interactor
            {
                /// <summary>
                ///   A dictionary of keys and buttons.
                /// </summary>
                [Serializable]
                public class ButtonKeyDictionary : SerializableDictionary<Button, string> { }
                /// <summary>
                ///   Registered buttons.
                /// </summary>
                /// <remarks>
                ///   Edit this member in the Inspector to tell which buttons will this instance
                ///     have access to.
                /// </remarks>
                [SerializeField]
                private ButtonKeyDictionary buttons = new ButtonKeyDictionary();

                public string Result { get; private set; }

                void Start()
                {
                    Result = null;
                    foreach(System.Collections.Generic.KeyValuePair<Button, string> kvp in buttons)
                    {
                        kvp.Key.onClick.AddListener(delegate () { Result = kvp.Value; });
                    }
                }

                /// <summary>
                ///   This implementation will clear any former result and wait until a result is
                ///     available.
                /// </summary>
                /// <param name="interactiveMessage">
                ///   An instance of <see cref="InteractiveInterface"/>, first referenced by the instance of
                ///     <see cref="InteractiveInterface"/> that ultimately triggered this interaction. 
                /// </param>
                /// <returns>An enumerator to be run inside a coroutine.</returns>
                protected override IEnumerator Input(InteractiveMessage interactiveMessage)
                {
                    Result = null;
                    yield return new WaitWhile(delegate() { return Result == null; });
                }
            }
        }
    }
}
