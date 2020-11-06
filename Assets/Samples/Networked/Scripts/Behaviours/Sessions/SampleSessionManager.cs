using UnityEngine;
using System.Collections;
using NetRose.Behaviours.Sessions;
using NetRose.Behaviours.Sessions.Messages;
using NetRose.Behaviours.Sessions.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using NetworkedSamples.Behaviours.Sessions.Messages;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            [RequireComponent(typeof(SampleDatabase))]
            public class SampleSessionManager : SessionManager<int, SampleDatabase.Account, int, string, SampleDatabase.Character, SampleChooseCharacter, SampleUsingCharacter, SampleInvalidCharacterID, SampleCharacterDoesNotExist>
            {
                // Notes: This is a DUMB manager and the full data will be the prefab. Typically, game objects are not passed but,
                //        instead, arbitrary serializable data structures. In this case, the asset GUID will be passed as string.

                private SampleDatabase database;

                protected override void Awake()
                {
                    base.Awake();
                    database = GetComponent<SampleDatabase>();
                }
                
                protected override AccountCharacterFetcher<int, int, string, SampleDatabase.Character> GetAccountCharacterFetcher()
                {
                    return database;
                }

                protected override AccountFetcher<int, SampleDatabase.Account> GetAccountFetcher()
                {
                    return database;
                }
            }
        }
    }
}