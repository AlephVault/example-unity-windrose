using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.KeyPair;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.KeyPair
{
    public class ShhHasKeyPair : GenericRpcRequestResponseHandlerParamString<bool>, IShhHasKeyPair
    {
        public ShhHasKeyPair(IClient client) : base(client, ApiMethods.shh_hasKeyPair.ToString())
        {
        } 
    }
}