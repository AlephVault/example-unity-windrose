using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.VirtualPad
{
    namespace Types
    {
        /// <summary>
        ///   The reason for a control sync rejection.
        /// </summary>
        public enum RejectReason
        {
            AlreadySynced, EmptyLabel, InvalidPassword,
            InvalidIndex, OccupiedIndex, ServerIsFull
        }
    }
}
