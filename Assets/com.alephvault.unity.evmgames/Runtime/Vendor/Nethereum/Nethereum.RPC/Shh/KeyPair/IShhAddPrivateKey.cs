using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.KeyPair
{
    public interface IShhAddPrivateKey
    {
        Task<string> SendRequestAsync(string privateKey, object id = null);
        RpcRequest BuildRequest(string privateKey, object id = null);
    }
}