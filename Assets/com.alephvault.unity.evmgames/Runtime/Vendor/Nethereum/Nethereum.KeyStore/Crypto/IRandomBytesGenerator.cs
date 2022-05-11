namespace AlephVault.Unity.EVMGames.Nethereum.KeyStore.Crypto
{
    public interface IRandomBytesGenerator
    {
        byte[] GenerateRandomInitialisationVector();
        byte[] GenerateRandomSalt();
    }
}