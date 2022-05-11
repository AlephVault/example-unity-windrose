using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.SymKey
{
    public class ShhAddSymKey : GenericRpcRequestResponseHandlerParamString<string>, IShhAddSymKey
    {
        public ShhAddSymKey(IClient client) : base(client, ApiMethods.shh_addSymKey.ToString())
        {
        }
    }
}