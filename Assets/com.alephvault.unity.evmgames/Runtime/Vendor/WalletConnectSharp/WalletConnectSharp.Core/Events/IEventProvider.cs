namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events
{
    public interface IEventProvider
    {
        void PropagateEvent(string topic, string responseJson);
    }
}