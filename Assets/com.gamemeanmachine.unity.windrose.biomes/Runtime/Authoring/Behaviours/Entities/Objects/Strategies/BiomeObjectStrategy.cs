using System;
using Codice.CM.Common.Serialization;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.Strategies.Base;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies.Base;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.ScriptableObjects.Tiles;
using GameMeanMachine.Unity.WindRose.Biomes.Types;
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
                    namespace Base
                    {
                        /// <summary>
                        ///   This strategy is just the counterpart of <see cref="BiomeObjectsManagementStrategy"/>.
                        /// </summary>
                        [RequireComponent(typeof(LayoutObjectStrategy))]
                        public class BiomeObjectStrategy : ObjectStrategy
                        {
                            /// <summary>
                            ///   The counterpart type is <see cref="BiomeObjectsManagementStrategy"/>.
                            /// </summary>
                            protected override Type GetCounterpartType()
                            {
                                return typeof(BiomeObjectsManagementStrategy);
                            }
                            
                            /// <summary>
                            ///   The biome set this strategy relates to.
                            /// </summary>
                            [SerializeField]
                            internal BiomeSet biomeSet;

                            // The current biome.
                            [SerializeField]
                            private byte biome = 0;

                            /// <summary>
                            ///   The current biome for this object.
                            /// </summary>
                            public byte Biome
                            {
                                get => biome;
                                set
                                {
                                    if (value >= biomeSet.Count)
                                    {
                                        throw new IndexOutOfRangeException(
                                            "The new biome index is not valid"
                                        );
                                    }

                                    byte oldBiome = biome;
                                    biome = value;
                                    PropertyWasUpdated("biome", oldBiome, biome);
                                }
                            }

                            protected override void Awake()
                            {
                                base.Awake();
                                if (biomeSet == null)
                                {
                                    Destroy(gameObject);
                                    throw new MissingBiomeSetException(
                                        "A biome set must be added to this object strategy"
                                    );
                                }

                                if (biome >= biomeSet.Count)
                                {
                                    Destroy(gameObject);
                                    throw new IndexOutOfRangeException(
                                        "The default biome index is not valid"
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}