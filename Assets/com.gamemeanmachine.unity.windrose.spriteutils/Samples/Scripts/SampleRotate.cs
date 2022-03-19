using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(MapObject))]
        public class SampleRotate : MonoBehaviour
        {
            private float time = 0;
            private int index = 0;
            private MapObject mapObject;
            private static Direction[] directions = {
                Direction.LEFT, Direction.UP, Direction.RIGHT, Direction.DOWN
            };
            
            private void Awake()
            {
                mapObject = GetComponent<MapObject>();
            }

            // Start is called before the first frame update
            void Start()
            {
                mapObject.Orientation = Direction.DOWN;
            }

            // Update is called once per frame
            void Update()
            {
                time += Time.deltaTime;
                if (time >= 1f)
                {
                    time -= 1f;
                    index += 1;
                    if (index == directions.Length) index = 0;
                    mapObject.Orientation = directions[index];
                }
            }
        }
    }
}
