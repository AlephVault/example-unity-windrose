using System.Numerics;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Fee1559Suggestions
{

    public interface IFee1559SuggestionStrategy
    {
#if !DOTNET35
        Task<Fee1559> SuggestFeeAsync(BigInteger? maxPriorityFeePerGas = null);
#endif
    }

}