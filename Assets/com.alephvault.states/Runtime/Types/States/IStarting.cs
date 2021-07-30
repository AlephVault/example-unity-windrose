using UnityEngine;
using System.Collections;


namespace AlephVault.States
{
    namespace Types.States
    {
        /// <summary>
        ///   Represents an initial state. It has
        ///   a particular start callback and marks
        ///   this state as initial.
        /// </summary>
        public interface IStarting
        {
            /// <summary>
            ///   Executes start logic for the given
            ///   state machine.
            /// </summary>
            /// <param name="machine">The state machine starting in this state</param>
            void OnStart(StateMachine machine);
        }
    }
}
