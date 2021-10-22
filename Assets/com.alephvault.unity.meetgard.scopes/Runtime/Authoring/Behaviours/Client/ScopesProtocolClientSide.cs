using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Support.Authoring.Behaviours;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   The client side implementation of the scopes-managing protocol.
                ///   It works for a client connection and will be aware of the other
                ///   side (i.e. client) of the scopes the server instantiates over
                ///   the network. It also manages the related objects. For both the
                ///   objects and the scopes, exactly one counterpart will exist in
                ///   the client, and a perfect match must exist to avoid any kind
                ///   of errors and mismatches.
                /// </summary>
                [RequireComponent(typeof(AsyncQueueManager))]
                public partial class ScopesProtocolClientSide : ProtocolServerSide<ScopesProtocolDefinition>
                {
                    // The queue management dependency.
                    private AsyncQueueManager queueManager;

                    /// <summary>
                    /// </summary>
                    [SerializeField]
                    private ScopeClientSide[] defaultScopePrefabs;

                    /// <summary>
                    /// </summary>
                    [SerializeField]
                    private ScopeClientSide[] extraScopePrefabs;

                    /// <summary>
                    /// </summary>
                    [SerializeField]
                    private ObjectClientSide[] objectPrefabs;

                    // TODO implement everyhing.

                    protected override void SetIncomingMessageHandlers()
                    {
                        throw new System.NotImplementedException();
                    }
                }
            }
        }
    }
}