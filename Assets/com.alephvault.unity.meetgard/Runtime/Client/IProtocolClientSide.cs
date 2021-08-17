using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;
using System;
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
            ///   For a given message name, gets the tag it acquired when
            ///   it was registered. Returns null if absent.
            /// </summary>
            /// <param name="message">The name of the message to get the tag for</param>
            /// <returns>The tag (nullable)</returns>
            public ushort? GetOutgoingMessageTag(string message);

            /// <summary>
            ///   Gets the type of a particular outgoing message tag. Returns
            ///   null if the tag is not valid.
            /// </summary>
            /// <param name="tag">The tag to get the type for</param>
            /// <returns>The type for the given tag</returns>
            public Type GetOutgoingMessageType(ushort tag);

            /// <summary>
            ///   Creates a message container for an incoming server message,
            ///   with a particular inner message tag.
            /// </summary>
            /// <param name="tag">The message tag to get the container for</param>
            /// <returns>The message container</returns>
            public ISerializable NewMessageContainer(ushort tag);

            /// <summary>
            ///   Gets a registered client side handler for a given tag.
            /// </summary>
            /// <param name="tag">The message tag to get the handler for</param>
            /// <returns>The message handler</returns>
            public Action<NetworkClient, ISerializable> GetIncomingMessageHandler(ushort tag);
        }
    }
}