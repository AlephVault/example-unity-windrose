using System;
using UnityEngine;
using Support.Types;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /**
             * This component registers all the (other) components that will be used as interactors.
             * 
             * In this component, the user will register them like (string key) => (Interactor component) in the UI.
             * After that, one will be able to invoke something like this to yield an interaction inside a generator
             *   being turned into coroutine by an InteractionInterface:
             * 
             *     yield return im["sample-component"].RunInteraction(... see details of this method in Interactor.cs file ...);
             *     yield return im["another-component"].RunInteraction(... see details of this method in Interactor.cs file ...);
             * 
             * See the Interactor class for more details.
             */
            public class InteractorsManager : MonoBehaviour
            {
                /**
                 * The first thing we need to define, is a dictionary of components to register.
                 * Such dictionary will be like (string) => (Interactor).
                 */
                [Serializable]
                public class InteractorsDictionary : SerializableDictionary<string, Interactor> {}

                [SerializeField]
                private InteractorsDictionary interactors = new InteractorsDictionary();

                public Interactor this[string key]
                {
                    get { return interactors[key]; }
                }
            }
        }
    }
}