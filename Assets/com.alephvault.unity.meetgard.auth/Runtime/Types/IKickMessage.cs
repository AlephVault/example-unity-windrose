using AlephVault.Unity.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Types
    {
        /// <summary>
        ///   This interface provides methods to hidrate
        ///   the object as one of the standard kick
        ///   messages.
        /// </summary>
        public interface IKickMessage<T> : ISerializable where T : IKickMessage<T>
        {

        }
    }
}
