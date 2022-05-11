using System;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client.RpcMessages;

namespace AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client.Streaming
{
    public interface IRpcStreamingResponseHandler
    {
        void HandleResponse(RpcStreamingResponseMessage rpcStreamingResponse);
        void HandleClientError(Exception ex);
        void HandleClientDisconnection();
    }
}