using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.BlockAuthoring
{
    /// <Summary>
    ///     parity_gasCeilTarget
    ///     Returns current target for gas ceiling.
    ///     Parameters
    ///     None
    ///     Returns
    ///     Quantity - Gas ceiling target.
    ///     Example
    ///     Request
    ///     curl --data '{"method":"parity_gasCeilTarget","params":[],"id":1,"jsonrpc":"2.0"}' -H "Content-Type:
    ///     application/json" -X POST localhost:8545
    ///     Response
    ///     {
    ///     "id": 1,
    ///     "jsonrpc": "2.0",
    ///     "result": "0x5fdfb0" // 6283184
    ///     }
    /// </Summary>
    public class ParityGasCeilTarget : GenericRpcRequestResponseHandlerNoParam<HexBigInteger>, IParityGasCeilTarget
    {
        public ParityGasCeilTarget(IClient client) : base(client, ApiMethods.parity_gasCeilTarget.ToString())
        {
        }
    }

    public interface IParityGasCeilTarget : IGenericRpcRequestResponseHandlerNoParam<HexBigInteger>
    {


    }
}