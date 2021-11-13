using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Teleport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Server
        {
            /// <summary>
            ///   Links the door to a certain target given by its key settings.
            /// </summary>
            [RequireComponent(typeof(Door))]
            public class DoorLinker : EmptyModelServerSide
            {
                private static Dictionary<string, DoorLinker> doorLinkersByName = new Dictionary<string, DoorLinker>();

                [SerializeField]
                private string doorName;

                [SerializeField]
                private string targetName;

                protected void Start()
                {
                    base.Start();
                    OnSpawned += DoorLinker_OnSpawned;
                    OnDespawned += DoorLinker_OnDespawned;
                }

                private async Task DoorLinker_OnSpawned()
                {
                    if (doorName == "")
                    {
                        Destroy(gameObject);
                        throw new Exception("Door linkers must have a name - destroying");
                    }

                    if (doorLinkersByName.ContainsKey(doorName))
                    {
                        Destroy(gameObject);
                        throw new Exception("Door linker name already in use: " + doorName);
                    }

                    if (targetName != "")
                    {
                        try
                        {
                            GetComponent<LocalTeleporter>().Target = doorLinkersByName[targetName].GetComponent<TeleportTarget>();
                        }
                        catch (KeyNotFoundException)
                        {
                            Destroy(gameObject);
                            throw new Exception("Invalid door linker name for target: " + targetName);
                        }
                    }

                    doorLinkersByName.Add(doorName, this);
                }

                private async Task DoorLinker_OnDespawned()
                {
                    doorLinkersByName.Remove(doorName);
                }
            }
        }
    }
}