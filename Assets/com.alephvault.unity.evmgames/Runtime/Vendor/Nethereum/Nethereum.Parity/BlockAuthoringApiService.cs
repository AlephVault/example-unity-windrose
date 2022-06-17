using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.BlockAuthoring;
using AlephVault.Unity.EVMGames.Nethereum.RPC;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public class BlockAuthoringApiService : RpcClientWrapper, IBlockAuthoringApiService
    {
        public BlockAuthoringApiService(IClient client) : base(client)
        {
            DefaultExtraData = new ParityDefaultExtraData(client);
            ExtraData = new ParityExtraData(client);
            GasFloorTarget = new ParityGasFloorTarget(client);
            GasCeilTarget = new ParityGasCeilTarget(client);
            MinGasPrice = new ParityMinGasPrice(client);
            TransactionsLimit = new ParityTransactionsLimit(client);
        }

        public IParityDefaultExtraData DefaultExtraData { get; }
        public IParityExtraData ExtraData { get; }
        public IParityGasCeilTarget GasCeilTarget { get; }
        public IParityGasFloorTarget GasFloorTarget { get; }
        public IParityMinGasPrice MinGasPrice { get; }
        public IParityTransactionsLimit TransactionsLimit { get; }
    }
}