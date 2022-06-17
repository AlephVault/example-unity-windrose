using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.Geth;
using AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Services;
using AlephVault.Unity.EVMGames.Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum
{
    public interface IWeb3Quorum: IWeb3Geth
    {
        List<string> PrivateFor { get; }
        string PrivateFrom { get; }
        IQuorumChainService Quorum { get; }

        IPermissionService Permission { get;}
        IPrivacyService Privacy { get; }
        IRaftService Raft { get;  }
        IIBFTService IBFT { get; }

        IContractExtensionsService ContractExtensions { get;}
        IDebugQuorumService DebugQuorum { get; }

        void ClearPrivateForRequestParameters();
        void SetPrivateRequestParameters(IEnumerable<string> privateFor, string privateFrom = null);
    }
}