using System.Threading.Tasks;
using System.Collections.Generic;


namespace AlephVault.States
{
    namespace Types
    {
        /// <summary>
        ///   A state machine lives its life through a particular
        ///   set of defined states.
        /// </summary>
        public abstract class StateMachine
        {
            /// <summary>
            ///   The current status of this machine.
            /// </summary>
            public StateMachineStatus Status { get; private set; }

            // All the states this machine traverses.
            private Dictionary<string, State> m_states;

            // All the allowed states as next steps.
            private HashSet<string> nextAvailableStates;

            // Tells whether Start or Go are being run.
            // This avoids re-entrant code.
            private bool busy = false;

            /// <summary>
            ///   The current state key of this machine. This
            ///   only makes sense when the status is either
            ///   Running or Finished.
            /// </summary>
            public string State { get; private set; }

            /// <summary>
            ///   Creates a new state machine, and give it
            ///   the set of states and a <code>New</code>
            ///   machine status.
            /// </summary>
            public StateMachine()
            {
                Status = StateMachineStatus.New;
                State = null;
                nextAvailableStates = null;
                m_states = InitStates();
            }

            /// <summary>
            ///   Gets the states to use for this machine.
            /// </summary>
            /// <returns>The list of available states</returns>
            protected abstract Dictionary<string, State> InitStates();

            /// <summary>
            ///   Initializes the state machine in one of
            ///   the allowed starting states.
            /// </summary>
            /// <param name="initialStateKey"></param>
            public async Task Start(string initialStateKey)
            {
                if (Status != StateMachineStatus.New)
                {
                    throw new Exception("Cannot start a state machine that is not in a new status");
                }

                if (busy)
                {
                    throw new Exception("This state machine is already invoking Start() or Go() - cannot run re-entrant code");
                }

                // Lock the execution.
                busy = true;
                State initialState = GetState(initialStateKey);
                // On Start.
                if (!(initialState is States.IStarting)) throw new Exception(string.Format("State is not initial (IStarting): {0}", initialStateKey));
                await ((States.IStarting)initialState).OnStart(this);
                Status = StateMachineStatus.Running;
                // Arrive to state.
                await ArriveToState(initialState);
                // Release the execution.
                busy = false;
            }

            // Gets a state by its key or returns an informative error.
            private State GetState(string key)
            {
                try
                {
                    return m_states[key];
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception(string.Format("State not found: {0}", key));
                }
            }

            // Arrives to a new state among the expected ones.
            private async Task ArriveToState(State state)
            {
                State = state.Key;
                await ((state as States.IArrival)?.OnArrival(this) ?? Task.CompletedTask);
                nextAvailableStates = null;

                if (state is States.IAutomatic)
                {
                    // Getting the conditions to select the new state.
                    var conditions = ((States.IAutomatic)state).Options();
                    // Selecting the new state.
                    string nextStateKey = null;
                    foreach (var pair in conditions.Item2)
                    {
                        if (pair.Item1(this))
                        {
                            nextStateKey = pair.Item2;
                            break;
                        }
                    }
                    if (nextStateKey == null) nextStateKey = conditions.Item1;
                    // Leaving the current state.
                    await ((state as States.IDeparture)?.OnDeparture(this) ?? Task.CompletedTask);
                    State = null;
                    // Arriving to a new state.
                    await ArriveToState(GetState(nextStateKey));
                }
                else if (state is States.IManual)
                {
                    // Getting the options that can be run.
                    // Those options will be kept for the future.
                    nextAvailableStates = ((States.IManual)state).Options();
                }
                else if (state is States.IEnding)
                {
                    // Finishing the workflow.
                    await ((States.IEnding)state).OnEnd(this);
                    Status = StateMachineStatus.Finished;
                }
                else
                {
                    throw new Exception("Cannot arrive to a state that is neither automatic, manual, or ending");
                }
            }

            /// <summary>
            ///   Moves the state machine to one of the allowed states
            ///   (as given by the current state).
            /// </summary>
            /// <param name="nextStateKey">The next state to go to</param>
            public async Task Go(string nextStateKey)
            {
                if (Status != StateMachineStatus.Running)
                {
                    throw new Exception("Cannot manually transition any state on a state machine that is not running");
                }

                if (busy)
                {
                    throw new Exception("This state machine is already invoking Start() or Go() - cannot run re-entrant code");
                }

                if (!nextAvailableStates.Contains(nextStateKey))
                {
                    throw new Exception(string.Format("Invalid next state to move to: {0}", nextStateKey));
                }

                // Lock the execution.
                busy = true;
                State state = GetState(State);
                State nextState = GetState(nextStateKey);
                // Leaving the current state.
                await ((state as States.IDeparture)?.OnDeparture(this) ?? Task.CompletedTask);
                State = null;
                // Arriving to a new state.
                await ArriveToState(GetState(nextStateKey));
                // Release the execution.
                busy = false;
            }
        }
    }
}
