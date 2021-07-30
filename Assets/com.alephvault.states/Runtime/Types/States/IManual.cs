using System.Collections.Generic;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents a state with manual transition.
        ///   Provides keys for the next states that
        ///   may be executed out of it.
        /// </summary>
        public interface IManual
        {
            /// <summary>
            ///   The next states that may be chosen.
            /// </summary>
            /// <returns>A set of state keys</returns>
            HashSet<string> Options();
        }
    }
}
