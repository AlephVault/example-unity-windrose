using AlephVault.Unity.Support.Generic.Vendor.IUnified.Authoring.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Protocols;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                namespace World
                {
                    /// <summary>
                    ///   Related to a <see cref="Scope"/>, this behaviour
                    ///   serves as a link between a particular protocol
                    ///   server (which is a subclass of NetRose's one:
                    ///   <see cref="NetRoseProtocolServerSide{T}"/>) and
                    ///   the related Maps and Objects on itself, aside
                    ///   of being actually a Scope on itself (and thus
                    ///   having the mean to retrieve the children maps
                    ///   by itself).
                    /// </summary>
                    [RequireComponent(typeof(Scope))]
                    public partial class ScopeServerSide : MonoBehaviour
                    {
                        /// <summary>
                        ///   This class is a reference to an instance of
                        ///   any subtype of <see cref="NetRoseProtocolServerSide{Definition}"/>.
                        /// </summary>
                        [System.Serializable]
                        public class INetRoseProtocolServerSideSenderReference : IUnifiedContainer<INetRoseProtocolServerSideSender> {} 

                        /// <summary>
                        ///   This is the ID of the scope. Populated before
                        ///   awakening (i.e. populated while inactive and
                        ///   via our utility function), contains the id
                        ///   it will use for its messages in the game.
                        ///   This id should never be hacked into.
                        /// </summary>
                        [SerializeField]
                        private uint id;

                        /// <summary>
                        ///   This is the index of the prefab this scope was
                        ///   created with, inside a specified protocol server
                        ///   side. This field is also populated before the
                        ///   awakening, along the others.
                        /// </summary>
                        [SerializeField]
                        private uint prefabIndex;

                        /// <summary>
                        ///   This is the underlying protocol server side that
                        ///   spawned this scope object. In particular, this
                        ///   reference will only be used to forward any needed
                        ///   message to the clients. As with the id field, this
                        ///   is populated before awakening.
                        /// </summary>
                        [SerializeField]
                        private INetRoseProtocolServerSideSenderReference sender;
                    }
                }
            }
        }
    }
}
