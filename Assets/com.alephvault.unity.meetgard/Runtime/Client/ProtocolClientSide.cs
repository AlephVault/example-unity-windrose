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
            private Action<NetworkClient, ISerializable>[] handlers = null;

            /// <summary>
            ///   Initializes the handlers, according to its definition.
            /// </summary>
            public ProtocolClientSide()
            {
                handlers = new Action<NetworkClient, ISerializable>[definition.ServerMessagesCount()];
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
            ///   Gets the handler for a given requested tag.
            /// </summary>
            /// <param name="tag">The message tag to get the handler for</param>
            /// <returns>The message container</returns>
            public Action<NetworkClient, ISerializable> GetHandler(ushort tag)
            {
                try
                {
                    return handlers[tag];
                }
                catch(IndexOutOfRangeException)
                {
                    return null;
                }
            }
        }
    }
}