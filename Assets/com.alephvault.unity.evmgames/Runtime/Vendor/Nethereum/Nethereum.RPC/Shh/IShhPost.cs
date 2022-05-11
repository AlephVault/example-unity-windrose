using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh
{
    public interface IShhPost
    {
        RpcRequest BuildRequest(MessageInput input, object id = null);
        Task<string> SendRequestAsync(MessageInput input, object id = null);
    }
}
