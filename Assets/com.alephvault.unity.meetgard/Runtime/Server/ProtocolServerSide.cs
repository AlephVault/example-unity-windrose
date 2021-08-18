using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
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
            // The protocol definition instance is created on construction.
            private Definition definition = new Definition();

            public Action<ulong, ISerializable> GetIncomingMessageHandler(ushort tag)
            {
                throw new NotImplementedException();
            }

            public ushort? GetOutgoingMessageTag(string message)
            {
                throw new NotImplementedException();
            }

            public Type GetOutgoingMessageType(ushort tag)
            {
                throw new NotImplementedException();
            }

            public ISerializable NewMessageContainer(ushort tag)
            {
                throw new NotImplementedException();
            }

            public void OnConnected(ulong clientId)
            {
                throw new NotImplementedException();
            }

            public void OnDisconnected(ulong clientId, Exception reason)
            {
                throw new NotImplementedException();
            }

            public void OnServerStarted()
            {
                throw new NotImplementedException();
            }

            public void OnServerStopped(Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}