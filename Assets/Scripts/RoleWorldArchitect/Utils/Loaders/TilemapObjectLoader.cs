using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect.Utils.Loaders
{
    using Types;
    using Types.Tilemaps;
    class TilemapObjectLoader
    {
        public static GameObject CreateRepresented(Behaviors.Map map, AnimationSpec defaultAnimation, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = map.gameObject.transform;
            Layout.AddComponent<Behaviors.Positionable>(obj, new Dictionary<string, object>() {
                { "width", width },
                { "height", height },
                { "initialX", x },
                { "initialY", y },
                { "initialSolidness", solidness }
            });
            Layout.AddComponent<Behaviors.Snapped>(obj);
            Layout.AddComponent<SpriteRenderer>(obj);
            Layout.AddComponent<Behaviors.Represented>(obj, new Dictionary<string, object>() {
                { "defaultAnimation", defaultAnimation }
            });
            return obj;
        }

        public static GameObject CreateOriented(Behaviors.Map map, AnimationSpec defaultAnimation, AnimationSet idleAnimationSet, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
        {
            GameObject obj = CreateRepresented(map, defaultAnimation, x, y, width, height, solidness);
            Layout.AddComponent<Behaviors.Oriented>(obj, new Dictionary<string, object>() {
                { "idleAnimationSet", idleAnimationSet }
            });
            return obj;
        }

        public static GameObject CreateMovable(Behaviors.Map map, AnimationSpec defaultAnimation, AnimationSet idleAnimationSet, AnimationSet movingAnimationSet, uint speed, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
        {
            GameObject obj = CreateOriented(map, defaultAnimation, idleAnimationSet, x, y, width, height, solidness);
            Layout.AddComponent<Behaviors.Movable>(obj, new Dictionary<string, object>() {
                { "movingAnimationSet", movingAnimationSet },
                { "speed", speed }
            });
            return obj;
        }
    }
}
