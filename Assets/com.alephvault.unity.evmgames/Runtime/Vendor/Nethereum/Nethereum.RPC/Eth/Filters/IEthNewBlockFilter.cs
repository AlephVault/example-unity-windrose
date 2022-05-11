using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Filters
{
    public interface IEthNewBlockFilter: IGenericRpcRequestResponseHandlerNoParam<HexBigInteger>
    {

    }
}