using System;
using System.Collections;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Fee1559Suggestions;

namespace AlephVault.Unity.EVMGames.Nethereum.JsonRpc.UnityClient
{
    public interface IFee1559SuggestionUnityRequestStrategy
    {
        IEnumerator SuggestFee(BigInteger? maxPriorityFeePerGas = null);
        Fee1559 Result { get; set; }
        Exception Exception { get; set; }
    }
}