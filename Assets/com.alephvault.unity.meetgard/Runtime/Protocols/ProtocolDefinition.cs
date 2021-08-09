using AlephVault.Unity.Binary;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Protocols
    {
        /// <summary>
        ///   <para>
        ///     A protocol has a list of messages that can
        ///     be sent from client to server and vice versa.
        ///     They are different pairs of dictionaries, as
        ///     they are messages in opposite directions.
        ///   </para>
        ///   <para>
        ///     Reverse definitions to get an integer tag
        ///     by the message name, and vice versal will
        ///     be available as well.
        ///   </para>
        ///   <para>
        ///     This all makes this class suitable to be
        ///     distributed with both the server and the
        ///     client projects. It will be also required,
        ///     since the protocol implementations (which
        ///     may belong each to a different project)
        ///     need to make use of the definition to be
        ///     implemented.
        ///   </para>
        /// </summary>
        public abstract class ProtocolDefinition
        {
            // While the protocol will not be defined the
            // handling in this class, the message types
            // will be defined here.

            // The client messages are those that will be
            // sent by the client and handled by the server.
            // While the messages' implementations will not
            // be defined here, their types will.
            private SortedDictionary<string, Type> registeredClientMessageTypes = new SortedDictionary<string, Type>();

            // Each client message name will be mapped against
            // the tag it will have. These tags are known in
            // the object's construction, right after the
            // messages are defined and it is locked from any
            // further definition.
            private Dictionary<string, ushort> registeredClientMessageTag = new Dictionary<string, ushort>();

            // Each client message name will be mapped from
            // the tag it will have. These tags are known in
            // the object's construction, right after the
            // messages are defined and it is locked from any
            // further definition.
            private string[] registeredClientMessageByTag = null;

            // The server messages are those that will be
            // sent by the server and handled by the client.
            // While the messages' implementations will not
            // be defined here, their types will.
            private SortedDictionary<string, Type> registeredServerMessageTypes = new SortedDictionary<string, Type>();

            // Each server message name will be mapped against
            // the tag it will have. These tags are known in
            // the object's construction, right after the
            // messages are defined and it is locked from any
            // further definition.
            private Dictionary<string, ushort> registeredServerMessageTag = new Dictionary<string, ushort>();

            // Each server message name will be mapped from
            // the tag it will have. These tags are known in
            // the object's construction, right after the
            // messages are defined and it is locked from any
            // further definition.
            private string[] registeredServerMessageByTag = null;

            // Tells whether the definition is already done.
            // This flag is true after the object is fully
            // constructed.
            private bool isDefined = false;

            public ProtocolDefinition()
            {
                DefineMessages();
                isDefined = true;
                registeredClientMessageByTag = registeredClientMessageTypes.Keys.ToArray();
                for(ushort i = 0; i < registeredClientMessageByTag.Length; i++)
                {
                    registeredClientMessageTag.Add(registeredClientMessageByTag[i], i);
                }
                registeredServerMessageByTag = registeredServerMessageTypes.Keys.ToArray();
                for (ushort i = 0; i < registeredServerMessageByTag.Length; i++)
                {
                    registeredServerMessageTag.Add(registeredServerMessageByTag[i], i);
                }
            }

            /// <summary>
            ///   Implement this method with several calls
            ///   to <see cref="DefineServerMessage{T}(string)"/>
            ///   and <see cref="DefineClientMessage{T}(string)"/>.
            /// </summary>
            protected abstract void DefineMessages();

            /// <summary>
            ///   Registers a client message using a particular
            ///   serializable type.
            /// </summary>
            /// <typeparam name="T">The tpye of the message's content</typeparam>
            /// <param name="messageKey">The message's key</param>
            protected void DefineClientMessage<T>(string messageKey) where T : ISerializable
            {
                DefineMessage<T>(messageKey, "client", registeredClientMessageTypes);
            }

            /// <summary>
            ///   Registers a server message using a particular
            ///   serializable type.
            /// </summary>
            /// <typeparam name="T">The type of the message's content</typeparam>
            /// <param name="messageKey">The message's key</param>
            protected void DefineServerMessage<T>(string messageKey) where T : ISerializable
            {
                DefineMessage<T>(messageKey, "server", registeredServerMessageTypes);
            }

            // Registers a message using a particular serializable
            // type, and a particular context.
            private void DefineMessage<T>(string messageKey, string scope, SortedDictionary<string, Type> messages) where T : ISerializable
            {
                if (isDefined)
                {
                    throw new InvalidOperationException("Messages cannot defined outside of the protocol definition constructor");
                }

                if (messages.Count >= ushort.MaxValue)
                {
                    throw new InvalidOperationException($"This protocol has already {ushort.MaxValue} values registered in the {scope} side. No more messages can be registered there");
                }

                if (messageKey == null)
                {
                    throw new ArgumentNullException("messageKey");
                }

                messageKey = messageKey.Trim();
                if (messageKey == "")
                {
                    throw new ArgumentException("Message key is empty");
                }
                if (messages.ContainsKey(messageKey))
                {
                    throw new ArgumentException($"Message key already registered as a {scope} message: {messageKey}");
                }
                messages[messageKey] = typeof(T);
            }

            /// <summary>
            ///   Gets a registered client message's type. The type
            ///   will be an ISerializable implementor.
            /// </summary>
            /// <param name="messageKey">The message's key</param>
            /// <returns>The type of the message's content</returns>
            public Type GetClientMessageType(string messageKey)
            {
                return GetMessageType(messageKey, registeredClientMessageTypes);
            }

            /// <summary>
            ///   Gets a registered server message's type. The type
            ///   will be an ISerializable implementor.
            /// </summary>
            /// <param name="messageKey">The message's key</param>
            /// <returns>The type of the message's content</returns>
            public Type GetServerMessageType(string messageKey)
            {
                return GetMessageType(messageKey, registeredServerMessageTypes);
            }

            // Gets a registered message's type. The type will be
            // an ISerializable implementor.
            private Type GetMessageType(string messageKey, SortedDictionary<string, Type> messages)
            {
                return messages[messageKey];
            }

            /// <summary>
            ///   Returns a key-value pair over all of the registered client messages.
            /// </summary>
            /// <returns>An enumerator over all of the client message types</returns>
            public SortedDictionary<string, Type>.Enumerator GetClientMessageTypes()
            {
                return registeredClientMessageTypes.GetEnumerator();
            }

            /// <summary>
            ///   Returns a key-value pair over all of the registered server messages.
            /// </summary>
            /// <returns>An enumerator over all of the server message types</returns>
            public SortedDictionary<string, Type>.Enumerator GetServerMessageTypes()
            {
                return registeredServerMessageTypes.GetEnumerator();
            }

            /// <summary>
            ///   Gets the corresponding tag for a server message.
            ///   This is needed both when sending a message and
            ///   when installing a handler for an incoming message
            ///   (this is specified by its tag).
            /// </summary>
            /// <param name="messageKey">The key of the message of our interest</param>
            /// <returns>The tag that will be sent or mapped</returns>
            public ushort GetServerMessageTag(string messageKey)
            {
                return registeredServerMessageTag[messageKey];
            }

            /// <summary>
            ///   Gets the corresponding tag for a client message.
            ///   This is needed both when sending a message and
            ///   when installing a handler for an incoming message
            ///   (this is specified by its tag).
            /// </summary>
            /// <param name="messageKey">The key of the message of our interest</param>
            /// <returns>The tag that will be sent or mapped</returns>
            public ushort GetClientMessageTag(string messageKey)
            {
                return registeredClientMessageTag[messageKey];
            }
        }
    }
}
