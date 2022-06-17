using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Besu.RPC.Clique;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Besu
{
    public class CliqueApiService : RpcClientWrapper, ICliqueApiService
    {
        public CliqueApiService(IClient client) : base(client)
        {
            Discard = new CliqueDiscard(client);
            GetSigners = new CliqueGetSigners(client);
            GetSignersAtHash = new CliqueGetSignersAtHash(client);
            Proposals = new CliqueProposals(client);
            Propose = new CliquePropose(client);
        }

        public ICliqueDiscard Discard { get; }
        public ICliqueGetSigners GetSigners { get; }
        public ICliqueGetSignersAtHash GetSignersAtHash { get; }
        public ICliqueProposals Proposals { get; }
        public ICliquePropose Propose { get; }
    }
}