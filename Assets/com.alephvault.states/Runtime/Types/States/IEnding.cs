using System.Threading.Tasks;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents an ending state. It has
        ///   a particular end callback and marks
        ///   this state as ending.
        /// </summary>
        public interface IEnding
        {
            /// <summary>
            ///   Executes end logic for the given
            ///   state machine.
            /// </summary>
            /// <param name="machine">The state machine ending in this state</param>
            Task OnEnd(StateMachine machine);
        }
    }
}
