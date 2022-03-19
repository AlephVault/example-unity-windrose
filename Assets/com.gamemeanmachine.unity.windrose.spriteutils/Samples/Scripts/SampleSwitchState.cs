using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(MapObject))]
        public class SampleSwitchState : MonoBehaviour
        {
            private float time = 0;
            private int index = 0;
            private MapObject mapObject;
            private string[] states = { "", "alt" };
            
            private void Awake()
            {
                mapObject = GetComponent<MapObject>();
            }

            // Start is called before the first frame update
            void Start()
            {
                mapObject.CurrentStateKey = "";
            }

            // Update is called once per frame
            void Update()
            {
                time += Time.deltaTime;
                if (time >= 4f)
                {
                    time -= 4f;
                    index += 1;
                    if (index == states.Length) index = 0;
                    Debug.Log($"Using state: {states[index]}");
                    mapObject.CurrentStateKey = states[index];
                }
            }
        }
    }
}
