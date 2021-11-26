using System;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies.Base;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies.Solidness;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Entities.Objects
            {
                namespace Strategies
                {
                    /// <summary>
                    ///   Biome-Aware Simple object strategy is just a combination of
                    ///   <see cref="LayoutObjectStrategy"/>, <see cref="SolidnessObjectStrategy"/> and
                    ///     <see cref="BiomeObjectStrategy"/>.
                    ///   Its counterpart type is <see cref="BiomeAwareSimpleObjectsManagementStrategy"/>.
                    /// </summary>
                    [RequireComponent(typeof(LayoutObjectStrategy))]
                    [RequireComponent(typeof(SolidnessObjectStrategy))]
                    [RequireComponent(typeof(BiomeObjectStrategy))]
                    public class BiomeAwareSimpleObjectStrategy : ObjectStrategy
                    {
                        /// <summary>
                        ///   Its counterpart type is
                        ///   <see cref="BiomeAwareSimpleObjectsManagementStrategy"/>.
                        /// </summary>
                        protected override Type GetCounterpartType()
                        {
                            return typeof(BiomeAwareSimpleObjectsManagementStrategy);
                        }
                    }
                }
            }
        }
    }
}
