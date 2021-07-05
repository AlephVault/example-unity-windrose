using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Objects
            {
                using System;
                using System.Threading.Tasks;
                using Mirror;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies.Solidness;
                using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies.Solidness;

                /// <summary>
                ///   This class synchronizes, via events, the contents
                ///     of a solidness object strategy.
                /// </summary>
                [RequireComponent(typeof(NetworkedMapObject))]
                [RequireComponent(typeof(SolidnessObjectStrategy))]
                public class NetworkedSolidnessObjectStrategy : BaseBehaviour.RelatedBehaviour
                {
                    private class SimpleCommand : ClientRpcCommand
                    {
                        private Action run;

                        public override async Task Invoke(Func<bool> mustAccelerate)
                        {
                            run();
                        }

                        public SimpleCommand(Action logic)
                        {
                            run = logic;
                        }
                    }

                    private MapObject mapObject;
                    private SolidnessObjectStrategy linkedStrategy;

                    private void Awake()
                    {
                        mapObject = GetComponent<MapObject>();
                        linkedStrategy = GetComponent<SolidnessObjectStrategy>();
                    }

                    void Start()
                    {
                        mapObject.onPropertyUpdated.AddListener(OnPropertyUpdated);
                    }

                    void OnDestroy()
                    {
                        mapObject.onPropertyUpdated.RemoveListener(OnPropertyUpdated);
                    }

                    private void OnPropertyUpdated(ObjectStrategy strategy, string property, object oldValue, object newValue)
                    {
                        if (linkedStrategy == strategy)
                        {
                            if (property == "solidness")
                            {
                                RpcOnSolidnessUpdated((SolidnessStatus)oldValue, (SolidnessStatus)newValue);
                            }
                            if (property == "traversesOtherSolids")
                            {
                                RpcOnTraversesOtherSolidsUpdated((bool)oldValue, (bool)newValue);
                            }
                            if (property == "mask")
                            {
                                RpcOnMaskUpdated(((SolidObjectMask)oldValue), ((SolidObjectMask)newValue));
                            }
                        }
                    }

                    [ClientRpc]
                    private void RpcOnSolidnessUpdated(SolidnessStatus oldValue, SolidnessStatus newValue)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new SimpleCommand(delegate () {
                                linkedStrategy.Solidness = newValue;
                            }));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnTraversesOtherSolidsUpdated(bool oldValue, bool newValue)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new SimpleCommand(delegate () {
                                linkedStrategy.TraversesOtherSolids = newValue;
                            }));
                        }
                    }

                    [ClientRpc]
                    private void RpcOnMaskUpdated(SolidObjectMask oldValue, SolidObjectMask newValue)
                    {
                        if (!isServer)
                        {
                            AddToQueue(new SimpleCommand(delegate () {
                                linkedStrategy.Mask = newValue;
                            }));
                        }
                    }
                }
            }
        }
    }
}
