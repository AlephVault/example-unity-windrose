using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.KeyPair
{
    public class ShhGetPublicKey : GenericRpcRequestResponseHandlerParamString<string>, IShhGetPublicKey
    {
        public ShhGetPublicKey(IClient client) : base(client, ApiMethods.shh_getPublicKey.ToString())
        {
        } 
    }
}
