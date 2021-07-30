using System;

namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   A standard state, implemented as
        ///   middle/inner with departure and arrival
        ///   logic(s), and automatic mode. The departure
        ///   and arrival methods are provided on construction,
        ///   as the options as well.
        /// </summary>
        public class InnerAutoState : State, IArrival, IDeparture, IAutomatic
        {
            private string m_defaultState;
            private Tuple<Func<StateMachine, bool>, string>[] m_options;
            private Action<StateMachine> m_onArrival;
            private Action<StateMachine> m_onDeparture;

            /// <summary>
            ///   Initializes this state with its key and
            ///   a set of options to use automatically.
            ///   Also, the callbacks are allowed.
            /// </summary>
            /// <param name="key">The state key</param>
            /// <param name="defaultState">The default state, for when all the options' callbacks evaluate to false</param>
            /// <param name="options">The options</param>
            /// <param name="onArrival">The onArrival callback</param>
            /// <param name="onDeparture">The onDeparture callback</param>
            public InnerAutoState(
                string key, string defaultState, Tuple<Func<StateMachine, bool>, string>[] options,
                Action<StateMachine> onArrival = null, Action<StateMachine> onDeparture = null
            ) : base(key)
            {
                m_defaultState = defaultState;
                m_options = options;
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

            /// <summary>
            ///   Returns the by construction options.
            /// </summary>
            /// <returns>The by construction options</returns>
            public Tuple<string, Tuple<Func<StateMachine, bool>, string>[]> Options()
            {
                return new Tuple<string, Tuple<Func<StateMachine, bool>, string>[]>(m_defaultState, m_options);
            }
        }
    }
}
