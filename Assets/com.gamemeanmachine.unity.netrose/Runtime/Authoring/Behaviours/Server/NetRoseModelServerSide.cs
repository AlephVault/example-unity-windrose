using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Types;
using GameMeanMachine.Unity.NetRose.Types.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                /// <summary>
                ///   Map objects exist in server side, reflecting what is changed
                ///   in them to the client side, and sending those changes to the
                ///   client side by means of the current scope. These ones are also
                ///   related to a single WindRose map object in a single map.
                /// </summary>
                [RequireComponent(typeof(MapObject))]
                public abstract class NetRoseModelServerSide<SpawnData, RefreshData> : ModelServerSide<MapObjectModel<SpawnData>, MapObjectModel<RefreshData>>
                    where SpawnData : class, ISerializable, new()
                    where RefreshData : class, ISerializable, new()
                {
                    /// <summary>
                    ///   The related WindRose MapObject.
                    /// </summary>
                    public MapObject MapObject { get; private set; }

                    /// <summary>
                    ///   The NetRose scope server side this object belongs to.
                    ///   It MAY be null if the scope is not a NetRose one! Be careful with this.
                    /// </summary>
                    public NetRoseScopeServerSide NetRoseScopeServerSide { get; private set; }

                    private void Awake()
                    {
                        MapObject = GetComponent<MapObject>();
                    }

                    private void Start()
                    {
                        OnSpawned += ObjectServerSide_OnSpawned;
                        OnDespawned += ObjectServerSide_OnDespawned;
                        MapObject.onAttached.AddListener(OnAttached);
                        MapObject.onDetached.AddListener(OnDetached);
                        MapObject.onMovementStarted.AddListener(OnMovementStarted);
                        MapObject.onMovementFinished.AddListener(OnMovementFinished);
                        MapObject.onMovementCancelled.AddListener(OnMovementCancelled);
                        MapObject.onTeleported.AddListener(OnTeleported);
                        MapObject.onOrientationChanged.AddListener(OnOrientationChanged);
                        MapObject.onSpeedChanged.AddListener(OnSpeedChanged);
                    }

                    private void OnDestroy()
                    {
                        OnSpawned -= ObjectServerSide_OnSpawned;
                        OnDespawned -= ObjectServerSide_OnDespawned;
                        MapObject.onAttached.RemoveListener(OnAttached);
                        MapObject.onDetached.RemoveListener(OnDetached);
                        MapObject.onMovementStarted.RemoveListener(OnMovementStarted);
                        MapObject.onMovementFinished.RemoveListener(OnMovementFinished);
                        MapObject.onMovementCancelled.RemoveListener(OnMovementCancelled);
                        MapObject.onTeleported.RemoveListener(OnTeleported);
                        MapObject.onOrientationChanged.RemoveListener(OnOrientationChanged);
                        MapObject.onSpeedChanged.RemoveListener(OnSpeedChanged);
                    }

                    private async Task ObjectServerSide_OnSpawned()
                    {
                        NetRoseScopeServerSide = Scope.GetComponent<NetRoseScopeServerSide>();
                    }

                    private async Task ObjectServerSide_OnDespawned()
                    {
                        NetRoseScopeServerSide = null;
                    }

                    private void RunInMainThreadIfSpawned(Func<Task> callback)
                    {
                        Protocol.RunInMainThread(async () =>
                        {
                            if (NetRoseScopeServerSide != null) await callback();
                        });
                    }

                    private void OnAttached(Map map)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            // Please note: By this point, we're in the appropriate scope.
                            // This means that the given map belongs to the current scope.
                            return NetRoseScopeServerSide.BroadcastObjectAttached(
                                Id, (uint)NetRoseScopeServerSide.Maps.MapsToIDs[map],
                                MapObject.X, MapObject.Y
                            );
                        });
                    }

                    private void OnDetached()
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectDetached(Id);
                        });
                    }

                    private void OnMovementStarted(Direction direction)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectMovementStarted(
                                Id, MapObject.X, MapObject.Y, direction
                            );
                        });
                    }

                    private void OnMovementFinished(Direction direction)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectMovementFinished(
                                Id, MapObject.X, MapObject.Y
                            );
                        });
                    }

                    private void OnMovementCancelled(Direction? direction)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectMovementCancelled(
                                Id, MapObject.X, MapObject.Y
                            );
                        });
                    }

                    private void OnTeleported(ushort x, ushort y)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectTeleported(Id, x, y);
                        });
                    }

                    private void OnOrientationChanged(Direction direction)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectOrientationChanged(Id, direction);
                        });
                    }

                    private void OnSpeedChanged(uint speed)
                    {
                        RunInMainThreadIfSpawned(() =>
                        {
                            return NetRoseScopeServerSide.BroadcastObjectSpeedChanged(Id, speed);
                        });
                    }

                    private Status GetCurrentStatus()
                    {
                        if (MapObject.ParentMap != null)
                        {
                            return new Status() {
                                Attachment = new Attachment() {
                                    Position = new Position() {
                                        X = MapObject.X, Y = MapObject.Y
                                    },
                                    MapIndex = (uint)NetRoseScopeServerSide.Maps.MapsToIDs[MapObject.ParentMap]
                                },
                                Movement = MapObject.Movement
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }

                    protected override MapObjectModel<SpawnData> GetFullData(ulong connectionId)
                    {
                        return new MapObjectModel<SpawnData>() {
                            Status = GetCurrentStatus(),
                            Data = GetInnerFullData(connectionId)
                        };
                    }

                    protected abstract SpawnData GetInnerFullData(ulong connectionId);

                    protected override MapObjectModel<RefreshData> GetRefreshData(ulong connectionId, string context)
                    {
                        return new MapObjectModel<RefreshData>() {
                            Status = GetCurrentStatus(),
                            Data = GetInnerRefreshData(connectionId, context)
                        };
                    }

                    protected abstract RefreshData GetInnerRefreshData(ulong connectionId, string context);
                }
            }
        }
    }
}
