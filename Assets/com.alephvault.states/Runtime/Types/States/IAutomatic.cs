using System;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents a state with automatic transition.
        ///   It provides options that will be evaluated.
        ///   The first option that manages to satisfy its
        ///   condition for a given machine state is matched.
        ///   If no option is matched, a default state will
        ///   be matched instead.
        /// </summary>
        public interface IAutomatic
        {
            /// <summary>
            ///   The options consist of:
            ///   - The key of the default next state.
            ///   - An array of tuples (condition, next state).
            ///     They are meant for sequential evaluation.
            /// </summary>
            /// <returns>The options of this state</returns>
            Tuple<string, Tuple<Func<StateMachine, bool>, string>[]> Options();
        }
    }
}
