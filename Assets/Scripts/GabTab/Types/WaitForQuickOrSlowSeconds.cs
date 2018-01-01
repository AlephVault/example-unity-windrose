using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        public class WaitForQuickOrSlowSeconds : BaseWaitForQuickOrSlowSeconds
        {
            public WaitForQuickOrSlowSeconds(float quickSeconds, float slowSeconds, Predicate usingQuickMovement) : base(quickSeconds, slowSeconds, usingQuickMovement) {}

            protected override float deltaTime()
            {
                return Time.deltaTime;
            }
        }
    }
}
