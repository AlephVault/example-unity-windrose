using System.Threading.Tasks;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents a state with arrival logic.
        ///   A state machine arriving to this state
        ///   will have some code being run.
        /// </summary>
        public interface IArrival
        {
            /// <summary>
            ///   Executes some arrival logic for
            ///   the given state machine.
            /// </summary>
            /// <param name="machine">The state machine arriving to this state</param>
            Task OnArrival(StateMachine machine);
        }
    }
}
