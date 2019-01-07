using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Support
{
    namespace Behaviours
    {
        /// <summary>
        ///   This behaviour allows the object to throttle function executions. This is done
        ///     by invoking <see cref="Throttled(Action)"/>, which will execute the given
        ///     function but disallow further calls of <see cref="Throttled(Action)"/> until
        ///     the time specified in <see cref="lapse"/> passes. 
        /// </summary>
        public class Throttler : MonoBehaviour
        {
            /// <summary>
            ///   <![CDATA[
            ///     Time that must pass after the last call to <see cref="Throttled(Action)"/>
            ///       before another call is allowed. If < 0, this value will be forced to 1.
            ///       This value is expressed in seconds.
            ///   ]]>
            /// </summary>
            [SerializeField]
            private float lapse = 1f;

            /// <summary>
            ///   See <see cref="lapse"/>. This is just a public getter for that property.
            /// </summary>
            public float Lapse { get { return lapse; } }

            void Awake()
            {
                if (lapse <= 0) lapse = 1f;
            }

            /// <summary>
            ///   Tells whether the current throttler is locked or not (this is: the time
            ///     before allowing the next call has not yet passed).
            /// </summary>
            public bool Locked { get; private set; }

            private IEnumerator Unlock()
            {
                yield return new WaitForSeconds(Lapse);
                Locked = false;
            }

            /// <summary>
            ///   Executes a given function in a throttled fasion. This is: this method
            ///     will fail silently if the time after the last call to it was less than
            ///     the value expressed in <see cref="lapse"/>.
            /// </summary>
            /// <param name="action">The function to execute. Usually, an anonymous one.</param>
            public void Throttled(Action action)
            {
                if (Locked) return;

                Locked = true;
                try
                {
                    action();
                    StartCoroutine(Unlock());
                }
                catch (Exception)
                {
                    Locked = false;
                    throw;
                }
            }
        }
    }
}
