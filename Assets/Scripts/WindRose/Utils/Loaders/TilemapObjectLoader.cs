using System.Collections.Generic;
using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Utils
    {
        namespace Loaders
        {
            using Types;
            using Types.Tilemaps;
            class TilemapObjectLoader
            {
                public static GameObject CreateRepresented(Behaviours.Map map, AnimationSpec defaultAnimation, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
                {
                    GameObject obj = new GameObject();
                    obj.transform.parent = map.gameObject.transform;
                    Layout.AddComponent<Behaviours.Positionable>(obj, new Dictionary<string, object>() {
                        { "width", width },
                        { "height", height },
                        { "initialX", x },
                        { "initialY", y },
                        { "initialSolidness", solidness }
                    });
                    Layout.AddComponent<Behaviours.Snapped>(obj);
                    Layout.AddComponent<SpriteRenderer>(obj);
                    Layout.AddComponent<Behaviours.Represented>(obj, new Dictionary<string, object>() {
                        { "defaultAnimation", defaultAnimation }
                    });
                    return obj;
                }

                public static GameObject CreateOriented(Behaviours.Map map, AnimationSpec defaultAnimation, AnimationSet idleAnimationSet, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
                {
                    GameObject obj = CreateRepresented(map, defaultAnimation, x, y, width, height, solidness);
                    Layout.AddComponent<Behaviours.Oriented>(obj, new Dictionary<string, object>() {
                        { "idleAnimationSet", idleAnimationSet }
                    });
                    return obj;
                }

                public static GameObject CreateMovable(Behaviours.Map map, AnimationSpec defaultAnimation, AnimationSet idleAnimationSet, AnimationSet movingAnimationSet, uint speed, uint x, uint y, uint width, uint height, SolidnessStatus solidness)
                {
                    GameObject obj = CreateOriented(map, defaultAnimation, idleAnimationSet, x, y, width, height, solidness);
                    Layout.AddComponent<Behaviours.Movable>(obj, new Dictionary<string, object>() {
                        { "movingAnimationSet", movingAnimationSet },
                        { "speed", speed }
                    });
                    return obj;
                }
            }

        }
    }
}
