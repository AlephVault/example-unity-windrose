using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.SymKey
{
    public class ShhNewSymKey : GenericRpcRequestResponseHandlerNoParam<string>, IShhNewSymKey
    {
        public ShhNewSymKey(IClient client) : base(client, ApiMethods.shh_newSymKey.ToString())
        {
        }
    }
}
