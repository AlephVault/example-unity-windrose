namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Repositories
{
    public interface IBlockchainStoreRepositoryFactory
    {
        IAddressTransactionRepository CreateAddressTransactionRepository();
        IBlockRepository CreateBlockRepository();
        IContractRepository CreateContractRepository();
        ITransactionLogRepository CreateTransactionLogRepository();
        ITransactionRepository CreateTransactionRepository();
        ITransactionVMStackRepository CreateTransactionVmStackRepository();
    }
}
