namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events
{
    public interface IEvent<in T>
    {
        void SetData(T data);
    }
}