namespace AlephVault.States
{
    namespace Types
    {
        /// <summary>
        ///   States are identified by their keys.
        ///   Further traits are to be added later.
        ///   The key must be unique across a given
        ///   state machine.
        /// </summary>
        public abstract class State
        {
            /// <summary>
            ///   The state's key. It must be unique
            ///   across a same state machine's state
            ///   set/list.
            /// </summary>
            public readonly string Key;

            /// <summary>
            ///   Creates a state with a given key.
            /// </summary>
            /// <param name="key">The state's key.</param>
            public State(string key)
            {
                Key = key;
            }
        }
    }
}
