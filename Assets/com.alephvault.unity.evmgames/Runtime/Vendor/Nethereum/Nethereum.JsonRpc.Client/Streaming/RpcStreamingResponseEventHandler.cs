using System;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client.Streaming;

namespace AlephVault.Unity.EVMGames.Nethereum.JsonRpc.WebSocketStreamingClient
{
    public class RpcStreamingResponseEventHandler<TResponse> : RpcStreamingRequestResponseHandler<TResponse>
    {

        public event EventHandler<StreamingEventArgs<TResponse>> Response;

        protected RpcStreamingResponseEventHandler(IStreamingClient streamingClient) : base(streamingClient)
        {
        }

        protected override void HandleResponse(TResponse subscriptionDataResponse)
        {
            Response?.Invoke(this, new StreamingEventArgs<TResponse>(subscriptionDataResponse));
        }

        protected override void HandleResponseError(RpcResponseException exception)
        {
            Response?.Invoke(this, new StreamingEventArgs<TResponse>(exception));
        }
    }
}