using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.SymKey
{
    public class ShhGenerateSymKeyFromPassword : GenericRpcRequestResponseHandlerParamString<string>, IShhGenerateSymKeyFromPassword
    {
        public ShhGenerateSymKeyFromPassword(IClient client) : base(client, ApiMethods.shh_generateSymKeyFromPassword.ToString())
        {
        }
    }
}
