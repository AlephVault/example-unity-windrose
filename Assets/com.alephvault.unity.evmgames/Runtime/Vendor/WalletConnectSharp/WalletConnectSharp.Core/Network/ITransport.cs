using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events.Request;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events.Response;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Network
{
    public interface ITransport : IDisposable
    {
        bool Connected { get; }
        
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        
        string URL { get; }
        
        Task Open(string bridgeURL, bool clearSubscriptions = true);

        Task Close();

        Task SendMessage(NetworkMessage message);

        Task Subscribe(string topic);

        Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback) where T : JsonRpcResponse;

        Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest;

        void ClearSubscriptions();
    }
}