using System;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.Animations;
using GameMeanMachine.Unity.WindRose.Biomes.Authoring.Behaviours.Entities.Objects.Strategies;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.Biomes
{
    namespace Samples
    {
        [RequireComponent(typeof(MapObject))]
        [RequireComponent(typeof(BiomeAwareSimpleObjectStrategy))]
        public class SampleBiomeAwareCharacterHandler : MonoBehaviour
        {
            [SerializeField]
            private AnimationRose GroundStaying;
            
            [SerializeField]
            private AnimationRose GroundMoving;
            
            [SerializeField]
            private AnimationRose WaterStaying;
            
            [SerializeField]
            private AnimationRose WaterMoving;

            private MapObject mapObject;

            private BiomeObjectStrategy biomeObjectStrategy;

            private void Awake()
            {
                biomeObjectStrategy = GetComponent<BiomeObjectStrategy>();
                mapObject = GetComponent<MapObject>();
            }

            private void UseGround()
            {
                MultiRoseAnimated multiRoseAnimated = mapObject.MainVisual.GetComponent<MultiRoseAnimated>();
                multiRoseAnimated.ReplaceState("", GroundStaying);
                multiRoseAnimated.ReplaceState("moving", GroundMoving);
                biomeObjectStrategy.Biome = 0;
            }

            private void UseWater()
            {
                MultiRoseAnimated multiRoseAnimated = mapObject.MainVisual.GetComponent<MultiRoseAnimated>();
                multiRoseAnimated.ReplaceState("", WaterStaying);
                multiRoseAnimated.ReplaceState("moving", WaterMoving);
                biomeObjectStrategy.Biome = 1;
            }

            private void Update()
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    mapObject.Orientation = Direction.UP;
                    mapObject.StartMovement(Direction.UP);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    mapObject.Orientation = Direction.DOWN;
                    mapObject.StartMovement(Direction.DOWN);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    mapObject.Orientation = Direction.LEFT;
                    mapObject.StartMovement(Direction.LEFT);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    mapObject.Orientation = Direction.RIGHT;
                    mapObject.StartMovement(Direction.RIGHT);
                }

                if (Input.GetKey(KeyCode.W))
                {
                    UseGround();
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    UseWater();
                }
            }
        }
    }
}