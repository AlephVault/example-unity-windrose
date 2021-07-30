using System;
using System.Collections.Generic;

namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   A standard state, implemented as
        ///   ending/final with arrival logic.
        ///   The ending and arrival methods are
        ///   provided on construction.
        /// </summary>
        public class EndingState : State, IArrival, IEnding
        {
            private Action<StateMachine> m_onArrival;
            private Action<StateMachine> m_onEnd;

            /// <summary>
            ///   Initializes this state with its key. Also,
            ///   the callback for ending and arrival are allowed.
            /// </summary>
            /// <param name="key">The state key</param>
            /// <param name="onArrival">The onArrival callback</param>
            /// <param name="onEnd">The onEnd callback</param>
            public EndingState(
                string key,
                Action<StateMachine> onArrival = null,
                Action<StateMachine> onEnd = null
            ) : base(key)
            {
                m_onArrival = onArrival;
                m_onEnd = onEnd;
            }

            public virtual void OnArrival(StateMachine machine)
            {
                m_onArrival?.Invoke(machine);
            }

            public void OnEnd(StateMachine machine)
            {
                m_onEnd?.Invoke(machine);
            }
        }
    }
}
