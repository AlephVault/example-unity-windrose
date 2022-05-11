using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.SymKey
{
    public class ShhGetSymKey : GenericRpcRequestResponseHandlerParamString<string>, IShhGetSymKey
    {
        public ShhGetSymKey(IClient client) : base(client, ApiMethods.shh_getSymKey.ToString())
        {
        }
    }
}
