using AlephVault.Unity.Binary;
using System;
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
            /// <summary>
            ///   Updates the kick reason to use a message telling there
            ///   was an error while loading the account.
            /// </summary>
            /// <returns>The same object</returns>
            public T WithAccountLoadErrorReason();

            /// <summary>
            ///   Updates the kick reason to use a message telling there
            ///   was an error while initializing the session.
            /// </summary>
            /// <returns>The same object</returns>
            public T WithSessionInitializationErrorReason();

            /// <summary>
            ///   Updates the kick reason to use a message telling there
            ///   was an abrupt disconnection and that caused the kick.
            ///   This kick reason is never sent to the user.
            /// </summary>
            /// <param name="e">The exception that was triggered</param>
            /// <returns>The same object</returns>
            public T WithNonGracefulDisconnectionErrorReason(Exception reason);
        }
    }
}
