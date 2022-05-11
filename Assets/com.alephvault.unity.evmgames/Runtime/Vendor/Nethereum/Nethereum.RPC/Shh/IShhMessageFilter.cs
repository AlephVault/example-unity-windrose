using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.MessageFilter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh
{
    public interface IShhMessageFilter
    {
        IShhNewMessageFilter NewMessageFilter { get; }
        IShhDeleteMessageFilter DeleteMessageFilter { get; }
        IShhGetFilterMessages GetFilterMessages { get; }
    }
}
