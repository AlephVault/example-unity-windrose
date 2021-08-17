using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        // TODO Later, ensure NetworkClient is abstract and we have
        // TODO both NetworkRemoteClient and NetworkLocalClient.
        [DisallowMultipleComponent]
        public abstract class ProtocolClientSide<Definition> : MonoBehaviour, IProtocolClientSide where Definition : ProtocolDefinition, new()
        {
            // The related network client.
            private NetworkClient client;

            // The protocol definition instance is created on construction.
            private Definition definition = new Definition();

            // The handlers for this protocol. The action is already wrapped
            // to refer the current protocol.
            private Action<ISerializable>[] incomingMessageHandlers = null;

            // Initializes the handlers, according to its definition.
            private void Awake()
            {
                client = GetComponent<NetworkClient>();
                incomingMessageHandlers = new Action<ISerializable>[definition.ServerMessagesCount()];
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
            ///   Gets the handler for a given requested tag. The returned
            ///   handler already wraps an original handler also referencing
            ///   the current protocol.
            /// </summary>
            /// <param name="tag">The message tag to get the handler for</param>
            /// <returns>The message container</returns>
            public Action<ISerializable> GetIncomingMessageHandler(ushort tag)
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

            /// <summary>
            ///   Sends a message using another protocol. The type must match
            ///   whatever was used to register the message. Also, the protocol
            ///   specified in the type must exist as a sibling component.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="message">The name of the message being sent</param>
            /// <param name="content">The content of the message being sent</param>
            public Task Send<T>(string message, T content) where T : ISerializable
            {
                return client.Send(this, message, content);
            }

            /// <summary>
            ///   Sends a message using this protocol. The type must match
            ///   whatever was used to register the message.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="message">The name of the message being sent</param>
            /// <param name="content">The content of the message being sent</param>
            public Task Send<ProtocolType, T>(string message, T content)
                where ProtocolType : IProtocolClientSide
                where T : ISerializable
            {
                return client.Send<ProtocolType, T>(message, content);
            }
        }
    }
}