using System;
using System.Collections;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            namespace Interactors
            {
                /**
                 * This component registers all the (other) components that will be used as interactors.
                 * 
                 * In this component, the user will register them like (string key) => (Interactor component) in the UI.
                 * After that, one will be able to invoke something like this to yield an interaction inside a generator
                 *   being turned into coroutine by an InteractionRunner:
                 * 
                 *     yield return im["sample-component"].RunInteraction();
                 *     yield return im["another-component"].RunInteraction(... a delegate to initialize it ...);
                 */
                public class InteractorsManager : MonoBehaviour
                {
                    /**
                     * The first thing we need to define, is a dictionary of components to register.
                     * Such dictionary will be like (string) => (Interactor).
                     */

                    [Serializable]
                    public class InteractorsDictionary : SerializableDictionary<string, Interactor>
                    {
                        public InteractorsDictionary(System.Collections.Generic.IDictionary<string, Interactor> dict) : base(dict) { }
                    }

                    [SerializeField]
                    private InteractorsDictionary interactors;

                    public Interactor this[string key]
                    {
                        get { return interactors[key]; }
                    }
                }
            }
        }
    }
}