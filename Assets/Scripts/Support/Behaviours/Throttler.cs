using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Support
{
    namespace Behaviours
    {
        public class Throttler : MonoBehaviour
        {
            /**
             * Utility to limit the execution of certain body functions.
             */

            [SerializeField]
            private float lapse = 1f;

            public float Lapse { get { return lapse; } }

            void Awake()
            {
                if (lapse <= 0) lapse = 1f;
            }

            public bool Locked { get; private set; }

            private IEnumerator Unlock()
            {
                yield return new WaitForSeconds(Lapse);
                Locked = false;
            }

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
