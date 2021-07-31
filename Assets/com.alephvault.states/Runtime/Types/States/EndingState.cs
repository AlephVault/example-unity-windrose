using System;
using System.Threading.Tasks;


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
            private Func<StateMachine, Task> m_onArrival;
            private Func<StateMachine, Task> m_onEnd;

            /// <summary>
            ///   Initializes this state with its key. Also,
            ///   the callback for ending and arrival are allowed.
            /// </summary>
            /// <param name="key">The state key</param>
            /// <param name="onArrival">The onArrival callback</param>
            /// <param name="onEnd">The onEnd callback</param>
            public EndingState(
                string key,
                Func<StateMachine, Task> onArrival = null,
                Func<StateMachine, Task> onEnd = null
            ) : base(key)
            {
                m_onArrival = onArrival;
                m_onEnd = onEnd;
            }

            public async Task OnArrival(StateMachine machine)
            {
                await (m_onArrival?.Invoke(machine) ?? Task.CompletedTask);
            }

            public async Task OnEnd(StateMachine machine)
            {
                await (m_onEnd?.Invoke(machine) ?? Task.CompletedTask);
            }
        }
    }
}
