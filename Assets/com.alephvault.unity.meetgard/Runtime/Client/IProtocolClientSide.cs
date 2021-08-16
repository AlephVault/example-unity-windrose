using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        /// <summary>
        ///   <para>
        ///     A contract for all the protocol client sides.
        ///     They serve the purpose of enumerating and
        ///     accesing all of the handlers.
        ///   </para>
        /// </summary>
        public interface IProtocolClientSide
        {
            /// <summary>
            ///   Creates a message container for an incoming server message,
            ///   with a particular inner message tag.
            /// </summary>
            /// <param name="tag">The message tag to get the container for</param>
            /// <returns>The message container</returns>
            public ISerializable NewMessageContainer(ushort tag);
        }
    }
}