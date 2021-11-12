using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Samples.Common.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Server
        {
            public class OwnableModelServerSide : NetRoseModelServerSide<Ownable, Ownable>
            {
                public ulong ConnectionId;

                private static Ownable Owned = new Ownable() { IsOwned = true };
                private static Ownable NotOwned = new Ownable() { IsOwned = false };

                protected override Ownable GetInnerFullData(ulong connectionId)
                {
                    return (connectionId == ConnectionId) ? Owned : NotOwned;
                }

                protected override Ownable GetInnerRefreshData(ulong connectionId, string context)
                {
                    return (connectionId == ConnectionId) ? Owned : NotOwned;
                }
            }
        }
    }
}
