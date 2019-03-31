using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   Allows specifying a state to be picked. State change will be
            ///     notified to visuals' <see cref="Visuals.MultiState{StateType}"/>
            ///     components and every component attending the key change event.
            /// </summary>
            [RequireComponent(typeof(Object))]
            public class StatePicker : MonoBehaviour
            {
                public class StateKeyEvent : UnityEvent<string> {}

                /// <summary>
                ///   Notofies when the state key property changes.
                /// </summary>
                public readonly StateKeyEvent onStateKeyChanged = new StateKeyEvent();

                private string selectedKey = "";

                /// <summary>
                ///   Gets or sets the selected state key. Notifies the interested
                ///     behaviours of the key change.
                /// </summary>
                public string SelectedKey
                {
                    get
                    {
                        return selectedKey;
                    }
                    set
                    {
                        selectedKey = value;
                        onStateKeyChanged.Invoke(selectedKey);
                    }
                }

                private void DoStart()
                {
                    onStateKeyChanged.Invoke(selectedKey);
                }
            }
        }
    }
}
