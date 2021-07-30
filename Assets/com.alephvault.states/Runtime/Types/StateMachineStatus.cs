using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.States
{
    namespace Types
    {
        /// <summary>
        ///   The status of a state machine:
        ///   - New: It was just created.
        ///   - Running: It has started, but not yet finished.
        ///   - Finished: It has finished.
        /// </summary>
        public enum StateMachineStatus { New, Running, Finished }
    }
}
