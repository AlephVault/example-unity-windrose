using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        /// <summary>
        ///   Base class of a waiter coroutine that waits for "quick" or "slow" timers.
        /// </summary>
        /// <remarks>
        ///   This class is different to the WaitForSeconds or WaitForSecondsRealtime.
        ///   The wait time may and WILL change as long as the condition passed as
        ///     argument changes between true and false.
        /// </remarks>
        public abstract class BaseWaitForQuickOrSlowSeconds : CustomYieldInstruction
        {
            /// <summary>
            ///   This is the core of the waiter: will accumulate the time against the
            ///     quick or slow timing depending on the execution of the given
            ///     predicate.
            /// </summary>
            public override bool keepWaiting
            {
                get
                {
                    if (accumulatedTime >= (usingQuickMovement() ? quickSeconds : slowSeconds))
                    {
                        return false;
                    }
                    else
                    {
                        accumulatedTime += deltaTime();
                        return true;
                    }
                }
            }

            public delegate bool Predicate();

            private readonly Predicate usingQuickMovement;
            private float quickSeconds;
            private float slowSeconds;
            private float accumulatedTime;

            /// <summary>
            ///   Asks for the quick and slow wait times, and the predicate to tell whether to use the slow
            ///     and wait times.
            /// </summary>
            /// <param name="quickSeconds">The quick time</param>
            /// <param name="slowSeconds">The slow time - usually 10 times bigger than <paramref name="quickSeconds"/></param>
            /// <param name="usingQuickMovement">Predicate that checks whether to use the quick/slow time</param>
            public BaseWaitForQuickOrSlowSeconds(float quickSeconds, float slowSeconds, Predicate usingQuickMovement)
            {
                this.quickSeconds = quickSeconds;
                this.slowSeconds = slowSeconds;
                this.usingQuickMovement = usingQuickMovement;
                this.accumulatedTime = 0f;
            }

            protected abstract float deltaTime();
        }
    }
}
