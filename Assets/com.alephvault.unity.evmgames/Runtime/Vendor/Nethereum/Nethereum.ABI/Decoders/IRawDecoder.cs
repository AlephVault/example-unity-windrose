namespace AlephVault.Unity.EVMGames.Nethereum.ABI.Decoders
{
    public interface ICustomRawDecoder<T>
    {
        T Decode(byte[] output);
    }
}