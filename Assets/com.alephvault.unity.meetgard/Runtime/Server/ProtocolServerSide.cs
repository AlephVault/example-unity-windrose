using AlephVault.Unity.Meetgard.Protocols;
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

            
        }
    }
}