using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Teleport;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Types;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Server
        {
            [RequireComponent(typeof(NetRoseScopeServerSide))]
            public class TeleportSpawner : MonoBehaviour
            {
                [Serializable]
                public struct TeleportSetting
                {
                    public string TeleportName;
                    public uint MapIdx;
                    public ushort X;
                    public ushort Y;
                    public string TeleportTarget;
                    public bool ForcesDirection;
                    public Direction Direction;
                }

                [SerializeField]
                private TeleportSetting[] teleports;

                [SerializeField]
                private uint teleportIndex;

                private ScopeServerSide ScopeServerSide;
                private Scope Scope;

                private void Awake()
                {
                    ScopeServerSide = GetComponent<ScopeServerSide>();
                    ScopeServerSide.OnLoad += ScopeServerSide_OnLoad;
                    Scope = GetComponent<Scope>();
                }

                private void OnDestroy()
                {
                    ScopeServerSide.OnLoad -= ScopeServerSide_OnLoad;
                }

                private async Task ScopeServerSide_OnLoad()
                {
                    foreach(TeleportSetting teleport in teleports)
                    {
                        ObjectServerSide obj = ScopeServerSide.Protocol.InstantiateHere(teleportIndex);
                        DoorLinker doorLinker = obj.GetComponent<DoorLinker>();
                        TeleportTarget teleportTarget = obj.GetComponent<TeleportTarget>();
                        Behaviours.SetObjectFieldValues(doorLinker, new System.Collections.Generic.Dictionary<string, object>()
                        {
                            { "doorName", teleport.TeleportName },
                            { "targetName", teleport.TeleportTarget }
                        });
                        doorLinker.transform.localPosition = new Vector3(teleport.X, teleport.Y, 0);
                        doorLinker.transform.SetParent(Scope[(int)teleport.MapIdx].transform);
                        teleportTarget.ForceOrientation = teleport.ForcesDirection;
                        teleportTarget.NewOrientation = teleport.Direction;
                        var _ = ScopeServerSide.AddObject(obj);
                    }
                }
            }
        }
    }
}
