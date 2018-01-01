using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        public abstract class BaseWaitForQuickOrSlowSeconds : CustomYieldInstruction
        {
            /**
             * Tests whether we should end or not, according to whether we are using a quick timeout or a
             *   slow timeout. Descendants will use Time.deltaTime and Time.unscaledDeltaTime respectively.
             */
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
