namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents a state with departure logic.
        ///   A state machine leaving this state will
        ///   have some code being run.
        /// </summary>
        public interface IDeparture
        {
            /// <summary>
            ///   Executes some departure logic for
            ///   the given state machine.
            /// </summary>
            /// <param name="machine">The state machine leaving this state</param>
            void OnDeparture(StateMachine machine);
        }
    }
}
