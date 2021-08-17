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
        ///     A protocol client side is the implementation
        ///     for the clients using this protocol.
        ///   </para>
        ///   <para>
        ///     It is related to a particular protocol definition.
        ///   </para>
        /// </summary>
        [RequireComponent(typeof(NetworkClient))]
        // TODO also require: "Zero-th Protocol Client Side".
        [DisallowMultipleComponent]
        public abstract class ProtocolClientSide<Definition> : MonoBehaviour, IProtocolClientSide where Definition : ProtocolDefinition, new()
        {
            // The protocol definition instance is created on construction.
            private Definition definition = new Definition();

            // The handlers for this protocol.
            private Action<NetworkClient, ISerializable>[] incomingMessageHandlers = null;

            /// <summary>
            ///   Initializes the handlers, according to its definition.
            /// </summary>
            public ProtocolClientSide()
            {
                incomingMessageHandlers = new Action<NetworkClient, ISerializable>[definition.ServerMessagesCount()];
            }

            /// <summary>
            ///   Creates a message container for an incoming server message,
            ///   with a particular inner message tag.
            /// </summary>
            /// <param name="tag">The message tag to get the container for</param>
            /// <returns>The message container</returns>
            public ISerializable NewMessageContainer(ushort tag)
            {
                try
                {
                    Type messageType = definition.GetServerMessageTypeByTag(tag);
                    return (ISerializable)Activator.CreateInstance(messageType);
                }
                catch(IndexOutOfRangeException)
                {
                    return null;
                }
            }

            /// <summary>
            ///   For a given message name, gets the tag it acquired when
            ///   it was registered. Returns null if absent.
            /// </summary>
            /// <param name="message">The name of the message to get the tag for</param>
            /// <returns>The tag (nullable)</returns>
            public ushort? GetOutgoingMessageTag(string message)
            {
                try
                {
                    return definition.GetClientMessageTagByName(message);
                }
                catch(KeyNotFoundException)
                {
                    return null;
                }
            }

            /// <summary>
            ///   Gets the type of a particular outgoing message tag. Returns
            ///   null if the tag is not valid.
            /// </summary>
            /// <param name="tag">The tag to get the type for</param>
            /// <returns>The type for the given tag</returns>
            public Type GetOutgoingMessageType(ushort tag)
            {
                try
                {
                    return definition.GetClientMessageTypeByTag(tag);
                }
                catch(IndexOutOfRangeException)
                {
                    return null;
                }
            }

            /// <summary>
            ///   Gets the handler for a given requested tag.
            /// </summary>
            /// <param name="tag">The message tag to get the handler for</param>
            /// <returns>The message container</returns>
            public Action<NetworkClient, ISerializable> GetIncomingMessageHandler(ushort tag)
            {
                try
                {
                    return incomingMessageHandlers[tag];
                }
                catch(IndexOutOfRangeException)
                {
                    return null;
                }
            }
        }
    }
}