using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                public partial class ScopesProtocolServerSide : ProtocolServerSide<ScopesProtocolDefinition>
                {
                    // A reverse map between a connection id and the scope it belongs to.
                    // Virtual / reserved scopes can be specified here.
                    private Dictionary<ulong, uint> scopeForConnection = new Dictionary<ulong, uint>();

                    // Clears all of the connections in a given scope.
                    private async Task ClearConnectionsFromScope(ScopeServerSide scope)
                    {
                        // Create the limbo message.
                        var message = new MovedToScope()
                        {
                            PrefabIndex = Scope.LimboPrefab,
                            ScopeIndex = Scope.Limbo
                        };
                        // Then send it to each connection in the scope.
                        foreach (ulong connection in scope.connections)
                        {
                            scopeForConnection[connection] = Scope.Limbo;
                            try
                            {
                                await SendMovedToScope(connection, message);
                            }
                            catch { /* Diaper-ignore */ }
                        }
                        scope.connections.Clear();
                    }
                }
            }
        }
    }
}