using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;
using AlephVault.Unity.Meetgard.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        /// <summary>
        ///   <para>
        ///     A protocol server side is the implementation
        ///     for the servers using this protocol.
        ///   </para>
        ///   <para>
        ///     It is related to a particular protocol definition.
        ///   </para>
        /// </summary>
        [RequireComponent(typeof(NetworkServer))]
        [DisallowMultipleComponent]
        public abstract class ProtocolServerSide<Definition> : MonoBehaviour, IProtocolServerSide where Definition : ProtocolDefinition, new()
        {
            // The related network server.
            private NetworkServer server;

            // The protocol definition instance is created on construction.
            private Definition definition = new Definition();

            // The handlers for this protocol. The action is already wrapped
            // to refer the current protocol.
            private Action<ulong, ISerializable>[] incomingMessageHandlers = null;

            // Initializes the handlers, according to its definition.
            protected void Awake()
            {
                server = GetComponent<NetworkServer>();
                incomingMessageHandlers = new Action<ulong, ISerializable>[definition.ClientMessagesCount()];
                try
                {
                    SetIncomingMessageHandlers();
                }
                catch (System.Exception)
                {
                    Destroy(gameObject);
                    throw;
                }
            }

            /// <summary>
            ///   Implement this method with several calls to <see cref="AddIncomingMessageHandler{T}(string, Action{ProtocolServerSide{Definition}, ulong, T})"/>.
            /// </summary>
            protected abstract void SetIncomingMessageHandlers();

            /// <summary>
            ///   Adds a handler to a defined incoming message. The handler to
            ///   add must also allow a reference to the protocol as a generic
            ///   parent class reference.
            /// </summary>
            /// <typeparam name="T">The tpye of the message's content</typeparam>
            /// <param name="message">The message name</param>
            /// <param name="handler">The handler to register</param>
            protected void AddIncomingMessageHandler<T>(string message, Action<ProtocolServerSide<Definition>, ulong, T> handler) where T : ISerializable
            {
                if (message == null || message.Trim().Length == 0)
                {
                    throw new ArgumentException("The message name must not be null or empty");
                }

                if (handler == null)
                {
                    throw new ArgumentNullException("handler");
                }

                ushort incomingMessageTag;
                Type expectedIncomingMessageType;
                try
                {
                    incomingMessageTag = definition.GetClientMessageTagByName(message);
                    expectedIncomingMessageType = definition.GetClientMessageTypeByName(message);
                }
                catch (KeyNotFoundException)
                {
                    throw new UnexpectedMessageException($"The protocol definition of type {typeof(Definition).FullName} does not define a message: {message}");
                }

                if (expectedIncomingMessageType != typeof(T))
                {
                    throw new IncomingMessageTypeMismatchException($"Incoming message ({message}) in protocol {GetType().FullName} was attempted to handle with type {typeof(T).FullName} when {expectedIncomingMessageType.FullName} was expected");
                }

                if (incomingMessageHandlers[incomingMessageTag] == null)
                {
                    throw new HandlerAlreadyRegisteredException($"Incoming message ({message}) is already handled by {GetType().FullName} - cannot set an additional handler");
                }
                else
                {
                    incomingMessageHandlers[incomingMessageTag] = (clientId, content) => handler(this, clientId, (T)content);
                }
            }

            /// <summary>
            ///   Creates a sender shortcut, intended to send the message multiple times
            ///   and spend time on message mapping only once. Intended to be used on
            ///   lazy initialization of senders, or eager initializationin some sort of
            ///   extended <see cref="Awake"/> or similar method.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <param name="message">The message (as it was registered) that this sender will send</param>
            protected Func<ulong, T, Task<bool>> MakeSender<T>(string message) where T : ISerializable
            {
                return server.MakeSender<T>(this, message);
            }

            /// <summary>
            ///   Creates a sender shortcut, intended to send the message multiple times
            ///   and spend time on message mapping only once. Intended to be used on
            ///   lazy initialization of senders, or eager initializationin some sort of
            ///   extended <see cref="Awake"/> or similar method.
            /// </summary>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <typeparam name="T">The type of the message this sender will send</typeparam>
            /// <param name="message">The name of the message this sender will send</param>
            /// <returns>A function that takes the message to send, of the appropriate type, and sends it (asynchronously)</returns>
            protected Func<ulong, T, Task<bool>> MakeSender<ProtocolType, T>(string message) where ProtocolType : IProtocolServerSide where T : ISerializable
            {
                return server.MakeSender<ProtocolType, T>(message);
            }

            /// <summary>
            ///   Creates a broadcaster shortcut, intended to send the message multiple times
            ///   and spend time on message mapping only once. Intended to be used on
            ///   lazy initialization of senders, or eager initializationin some sort of
            ///   extended <see cref="Awake"/> or similar method.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <param name="message">The message (as it was registered) that this sender will send</param>
            protected Func<ulong[], T, HashSet<ulong>, Task> MakeBroadcaster<T>(string message) where T : ISerializable
            {
                return server.MakeBroadcaster<T>(this, message);
            }

            /// <summary>
            ///   Creates a broadcaster shortcut, intended to send the message multiple times
            ///   and spend time on message mapping only once. Intended to be used on
            ///   lazy initialization of senders, or eager initializationin some sort of
            ///   extended <see cref="Awake"/> or similar method.
            /// </summary>
            /// <typeparam name="ProtocolType">The protocol type for this message. One instance of it must be an already attached component</param>
            /// <typeparam name="T">The type of the message this sender will send</typeparam>
            /// <param name="message">The name of the message this sender will send</param>
            /// <returns>A function that takes the message to send, of the appropriate type, and sends it (asynchronously)</returns>
            protected Func<ulong[], T, HashSet<ulong>, Task> MakeBroadcaster<ProtocolType, T>(string message) where ProtocolType : IProtocolServerSide where T : ISerializable
            {
                return server.MakeBroadcaster<ProtocolType, T>(message);
            }

            /// <summary>
            ///   Creates a message container for an incoming client message,
            ///   with a particular inner message tag.
            /// </summary>
            /// <param name="tag">The message tag to get the container for</param>
            /// <returns>The message container</returns>
            public ISerializable NewMessageContainer(ushort tag)
            {
                try
                {
                    Type messageType = definition.GetClientMessageTypeByTag(tag);
                    return (ISerializable)Activator.CreateInstance(messageType);
                }
                catch (IndexOutOfRangeException)
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
                    return definition.GetServerMessageTagByName(message);
                }
                catch (KeyNotFoundException)
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
                    return definition.GetServerMessageTypeByTag(tag);
                }
                catch (IndexOutOfRangeException)
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
            public Action<ulong, ISerializable> GetIncomingMessageHandler(ushort tag)
            {
                try
                {
                    return incomingMessageHandlers[tag];
                }
                catch (IndexOutOfRangeException)
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
            /// <param name="clientId">The id of the client to send the message to</param>
            /// <param name="content">The content of the message being sent</param>
            public Task Send<T>(ulong clientId, string message, T content) where T : ISerializable
            {
                return server.Send(this, message, clientId, content);
            }

            /// <summary>
            ///   Sends a message using this protocol. The type must match
            ///   whatever was used to register the message.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="message">The name of the message being sent</param>
            /// <param name="clientId">The id of the client to send the message to</param>
            /// <param name="content">The content of the message being sent</param>
            public Task Send<ProtocolType, T>(string message, ulong clientId, T content)
                where ProtocolType : IProtocolServerSide
                where T : ISerializable
            {
                return server.Send<ProtocolType, T>(message, clientId, content);
            }

            /// <summary>
            ///   Broadcasts a message using another protocol. The type must match
            ///   whatever was used to register the message. Also, the protocol
            ///   specified in the type must exist as a sibling component.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="clientIds">The ids to send the same message - use null to specify ALL the available ids</param>
            /// <param name="content">The message content</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public Task Broadcast<T>(string message, ulong[] clientIds, T content, HashSet<ulong> failedEndpoints) where T : ISerializable
            {
                return server.Broadcast(this, message, clientIds, content, failedEndpoints);
            }

            /// <summary>
            ///   Broadcasts a message using this protocol. The type must match
            ///   whatever was used to register the message.
            /// </summary>
            /// <typeparam name="T">The type of the message being sent</typeparam>
            /// <param name="message">The message (as it was registered) being sent</param>
            /// <param name="clientIds">The ids to send the same message - use null to specify ALL the available ids</param>
            /// <param name="content">The message content</param>
            /// <param name="failedEndpoints">The output list of the endpoints that are not found or raised an error on send</param>
            public Task Broadcast<ProtocolType, T>(string message, ulong[] clientIds, T content, HashSet<ulong> failedEndpoints)
                where ProtocolType : IProtocolServerSide
                where T : ISerializable
            {
                return server.Broadcast<ProtocolType, T>(message, clientIds, content, failedEndpoints);
            }

            /// <summary>
            ///   <para>
            ///     This is a callback that gets invoked when a client successfully
            ///     established a connection to this server.
            ///   </para>
            ///   <para>
            ///     Override it at need.
            ///   </para>
            /// </summary>
            public virtual void OnConnected(ulong clientId)
            {
            }

            /// <summary>
            ///   <para>
            ///     This is a callback that gets invoked when a client is disconnected
            ///     from the server. This can happen gracefully locally, gracefully
            ///     remotely, or abnormally.
            ///   </para>
            ///   <para>
            ///     Override it at need.
            ///   </para>
            /// </summary>
            /// <param name="reason">If not null, tells the abnormal reason of closure</param>
            public virtual void OnDisconnected(ulong clientId, System.Exception reason)
            {
            }

            /// <summary>
            ///   This is a callback that gets invoked when the server has just started.
            /// </summary>
            public virtual void OnServerStarted()
            {
            }

            /// <summary>
            ///   This is a callback that gets invoked when the server is (and previously
            ///   all the client connections are as well) told to stop.
            /// </summary>
            /// <param name="e"></param>
            public virtual void OnServerStopped(System.Exception e)
            {
            }
        }
    }
}
