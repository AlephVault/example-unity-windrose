using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            [RequireComponent(typeof(UnityEngine.UI.Image))]
            public class InteractiveInterface : MonoBehaviour
            {
                private InteractiveMessage interactiveMessage;

                public enum SimplePromptButtonTypes { OK, OKCANCEL, YESNO, CONTINUE, END }
                public enum TextInputButtonTypes { OK, OKCANCEL }
                public enum ListInputButtonTypes { OK, OKCANCEL }

                void Start()
                {
                    interactiveMessage = Utils.Layout.RequireComponentInChildren<InteractiveMessage>(this.gameObject);
                }

                void StartSimpleMessage(string message, SimplePromptButtonTypes buttons = SimplePromptButtonTypes.CONTINUE)
                {
                    // display text
                    // wait for button
                    // access the input button
                }

                void StartTextInput(string message, TextInputButtonTypes buttons = TextInputButtonTypes.OK)
                {
                    // display text
                    // enable text input
                    // wait for button
                    // access the input value and button
                }

                void StartListInput(string message, ListInputButtonTypes buttons = ListInputButtonTypes.OK)
                {
                    // display text
                    // enable text input
                    // wait for selected item and OK button, or just cancel button
                    // access the input value and button
                }
            }
        }
    }
}