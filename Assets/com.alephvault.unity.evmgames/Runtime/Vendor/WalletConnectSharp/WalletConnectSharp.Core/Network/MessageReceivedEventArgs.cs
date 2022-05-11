using System;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public NetworkMessage Message { get; private set; }
        
        public ITransport Source { get; private set; }

        public MessageReceivedEventArgs(NetworkMessage message, ITransport source)
        {
            Message = message;
            Source = source;
        }
    }
}