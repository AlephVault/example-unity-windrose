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

            
        }
    }
}