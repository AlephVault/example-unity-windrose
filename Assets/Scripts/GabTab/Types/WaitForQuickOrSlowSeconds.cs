using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        /// <summary>
        ///   This is a subclass of <see cref="BaseWaitForQuickOrSlowSeconds"/> that takes the delta
        ///     time in a -perhaps- scaled fashion.
        /// </summary>
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
