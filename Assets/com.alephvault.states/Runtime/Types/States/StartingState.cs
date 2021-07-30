using System;
using System.Collections.Generic;

namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   A standard state, implemented as
        ///   starting with departure and arrival
        ///   logic(s), and manual mode. The starting,
        ///   departure and arrival methods are provided
        ///   on construction, as the options as well.
        /// </summary>
        public class StartingState : State, IStarting, IArrival, IDeparture, IManual
        {
            private HashSet<string> m_options;
            private Action<StateMachine> m_onStart;
            private Action<StateMachine> m_onArrival;
            private Action<StateMachine> m_onDeparture;

            /// <summary>
            ///   Initializes this state with its key and
            ///   a set of options to use manually. Also,
            ///   the callbacks are allowed.
            /// </summary>
            /// <param name="key">The state key</param>
            /// <param name="options">The options</param>
            /// <param name="onArrival">The onArrival callback</param>
            /// <param name="onDeparture">The onDeparture callback</param>
            /// <param name="onStart">The onStart callback</param>
            public StartingState(
                string key, HashSet<string> options, Action<StateMachine> onStart = null,
                Action<StateMachine> onArrival = null, Action<StateMachine> onDeparture = null
            ) : base(key)
            {
                m_options = options;
                m_onStart = onStart;
                m_onArrival = onArrival;
                m_onDeparture = onDeparture;
            }

            public virtual void OnArrival(StateMachine machine)
            {
                m_onArrival?.Invoke(machine);
            }

            public virtual void OnDeparture(StateMachine machine)
            {
                m_onDeparture?.Invoke(machine);
            }

            public void OnStart(StateMachine machine)
            {
                m_onStart?.Invoke(machine);
            }

            /// <summary>
            ///   Returns the by construction options.
            /// </summary>
            /// <returns>The by construction options</returns>
            public HashSet<string> Options()
            {
                return m_options;
            }
        }
    }
}
