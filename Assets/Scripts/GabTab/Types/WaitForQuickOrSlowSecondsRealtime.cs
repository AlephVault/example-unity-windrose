using UnityEngine;

namespace GabTab
{
    namespace Types
    {
        /// <summary>
        ///   This is a subclass of <see cref="BaseWaitForQuickOrSlowSeconds"/> that takes the delta
        ///     time in an unscaled fashion.
        /// </summary>
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
