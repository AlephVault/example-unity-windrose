namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.ProgressRepositories
{
    public interface IBlockProgressRepositoryFactory
    {
        IBlockProgressRepository CreateBlockProgressRepository();
    }
}
