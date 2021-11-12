using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Server
        {
            public class EmptyModelServerSide : NetRoseModelServerSide<Nothing, Nothing>
            {
                Nothing nothing = new Nothing();

                protected override Nothing GetFullData(ulong connection)
                {
                    return nothing;
                }

                protected override Nothing GetRefreshData(ulong connection, string context)
                {
                    return nothing;
                }
            }
        }
    }
}
