using System;
using System.Threading.Tasks;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   A standard state, implemented as
        ///   starting with departure and arrival
        ///   logic(s), and automatic mode. The starting,
        ///   departure and arrival methods are provided
        ///   on construction, as the options as well.
        /// </summary>
        public class StartingAutoState : State, IStarting, IArrival, IDeparture, IAutomatic
        {
            private string m_defaultState;
            private Tuple<Func<StateMachine, bool>, string>[] m_options;
            private Func<StateMachine, Task> m_onStart;
            private Func<StateMachine, Task> m_onArrival;
            private Func<StateMachine, Task> m_onDeparture;

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
            /// <param name="onStart">The onStart callback</param>
            public StartingAutoState(
                string key, string defaultState, Tuple<Func<StateMachine, bool>, string>[] options,
                Func<StateMachine, Task> onStart = null, Func<StateMachine, Task> onArrival = null,
                Func<StateMachine, Task> onDeparture = null
            ) : base(key)
            {
                m_defaultState = defaultState;
                m_options = options;
                m_onStart = onStart;
                m_onArrival = onArrival;
                m_onDeparture = onDeparture;
            }

            public async Task OnArrival(StateMachine machine)
            {
                await (m_onArrival?.Invoke(machine) ?? Task.CompletedTask);
            }

            public async Task OnDeparture(StateMachine machine)
            {
                await (m_onDeparture?.Invoke(machine) ?? Task.CompletedTask);
            }

            public async Task OnStart(StateMachine machine)
            {
                await (m_onStart?.Invoke(machine) ?? Task.CompletedTask);
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
