using System;
using System.Reflection;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        /**
         * This behaviour implements a broadcast for pause/resume methods.
         * 
         * This broadcast is executed even if the object is inactive.
         */
        public class Pausable : MonoBehaviour
        {
            /**
             * Invokes a method if it exists and the component asked to is not Pausable
             *   (which would be THIS component and an infinite recursion).
             */
            private void InvokeIfExists(Component component, string methodName, params object[] value)
            {
                Type type = component.GetType();
                if (type != typeof(Pausable))
                {
                    MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(component, value);
                    }
                }
            }

            private void InvokeOnEachComponent(string methodName, params object[] value)
            {
                foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
                {
                    InvokeIfExists(behaviour, methodName, value);
                }
            }

            public void Pause(bool fullFreeze)
            {
                InvokeOnEachComponent("Pause", fullFreeze);
            }

            public void Resume()
            {
                InvokeOnEachComponent("Resume");
            }
        }
    }
}
