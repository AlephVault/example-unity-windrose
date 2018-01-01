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
            /**
             * This interactor is the easiest non-trivial interactor.
             * 
             * This interactor has an internal record in the form of a dictionary mapping:
             * - Key: A Button (UI.Button) component.
             * - Value: A string that would be the returned value in the case the button gets
             *   clicked.
             * 
             * It is recommended that the buttons be children or descendants of the object
             *   holding this component. However, this is not required to work.
             * 
             * When initialized (in Start event), this component will take those buttons and add
             *   a listener for the click event. In such listener, it will set the return value
             *   to the assigned key in the editor.
             * 
             * The implementation of Input() will yield a WaitUntil object, with just a predicate
             *   asking for the value being non-null. Later, you can ask for the return value
             *   (i.e. after calling myButtonsInteractor.RunInteraction(...), you will be able to
             *   retrieve (ButtonsInteractor)myButtonsInteractor.Result) to know what to do next.
             */
            [RequireComponent(typeof(Image))]
            public class ButtonsInteractor : Interactor
            {
                /**
                 * The first thing we need to define, is a dictionary of buttons to register.
                 * Such dictionary will be like (Button) => (string).
                 */
                [Serializable]
                public class ButtonKeyDictionary : SerializableDictionary<Button, string> { }
                [SerializeField]
                private ButtonKeyDictionary buttons = new ButtonKeyDictionary();

                public string Result { get; private set; }

                void Start()
                {
                    base.Start();
                    Result = null;
                    foreach(System.Collections.Generic.KeyValuePair<Button, string> kvp in buttons)
                    {
                        kvp.Key.onClick.AddListener(delegate () { Result = kvp.Value; });
                    }
                }

                /**
                 * The implementation of this input does three important stuff:
                 * 1. Sets the result in null.
                 * 2. Waits until the result is no longer null.
                 */
                protected override IEnumerator Input(InteractiveMessage interactiveMessage)
                {
                    Result = null;
                    yield return new WaitWhile(delegate() { return Result == null; });
                }
            }
        }
    }
}
