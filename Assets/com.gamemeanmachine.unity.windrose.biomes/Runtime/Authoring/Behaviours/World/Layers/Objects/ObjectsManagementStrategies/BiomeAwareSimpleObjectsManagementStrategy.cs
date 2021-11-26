using System;
using System.Collections.Generic;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies.Simple;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies.Base;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies.Solidness;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    using Types;
    namespace Authoring
    {
        namespace Behaviours
        {
            using Entities.Objects.Strategies;

            namespace World
            {
                namespace Layers
                {
                    namespace Objects
                    {
                        namespace ObjectsManagementStrategies
                        {
                            /// <summary>
                            ///   <para>
                            ///     Combines the power of <see cref="LayoutObjectsManagementStrategy"/>
                            ///       which forbids walking through blocked cells, <see cref="SolidnessObjectsManagementStrategy"/>
                            ///       which forbids solid objects walking through occupied cells, and finally
                            ///       <see cref="BiomeObjectsManagementStrategy"/> which forbids non-biomatic
                            ///       movement for the object.
                            ///   </para>
                            ///   <para>
                            ///     Its counterpart is <see cref="BiomeAwareSimpleObjectStrategy"/>.
                            ///   </para> 
                            /// </summary>
                            [RequireComponent(typeof(LayoutObjectsManagementStrategy))]
                            [RequireComponent(typeof(SolidnessObjectsManagementStrategy))]
                            [RequireComponent(typeof(BiomeObjectsManagementStrategy))]
                            public class BiomeAwareSimpleObjectsManagementStrategy : ObjectsManagementStrategy
                            {
                                protected override ObjectsManagementStrategy[] GetDependencies()
                                {
                                    return new ObjectsManagementStrategy[]
                                    {
                                        GetComponent<LayoutObjectsManagementStrategy>(),
                                        GetComponent<SolidnessObjectsManagementStrategy>(),
                                        GetComponent<BiomeObjectsManagementStrategy>()
                                    };
                                }

                                protected override Type GetCounterpartType()
                                {
                                    return typeof(BiomeAwareSimpleObjectStrategy);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
