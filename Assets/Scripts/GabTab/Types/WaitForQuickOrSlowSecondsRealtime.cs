using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        public class WaitForQuickOrSlowSecondsRealtime : BaseWaitForQuickOrSlowSeconds
        {
            public WaitForQuickOrSlowSecondsRealtime(float quickSeconds, float slowSeconds, Predicate usingQuickMovement) : base(quickSeconds, slowSeconds, usingQuickMovement) {}

            protected override float deltaTime()
            {
                return Time.unscaledDeltaTime;
            }
        }
    }
}
