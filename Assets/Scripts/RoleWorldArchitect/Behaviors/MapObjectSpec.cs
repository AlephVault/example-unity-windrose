using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        using Types;
        using Types.Tilemaps;
        public class MapObjectSpec : MonoBehaviour
        {
            public enum MapObjectType { Representable, Oriented, Movable }

            public MapObjectType ObjectType;

            // For Representable
            public uint Width = 1;
            public uint Height = 1;
            public AnimationSpec DefaultAnimation;
            public SolidnessStatus SolidnessStatus;

            // For Oriented
            public AnimationSet IdleAnimationSet;

            // For Movable
            public AnimationSet MovingAnimationSet;
            public uint Speed;

            public void BringToLife(Map map)
            {
                uint x = (uint)Utils.Values.Max<float>(0, transform.localPosition.x);
                uint y = (uint)Utils.Values.Min<float>(0, -transform.localPosition.y);

                switch(ObjectType)
                {
                    case MapObjectType.Representable:
                        Utils.Loaders.TilemapObjectLoader.CreateRepresented(map, DefaultAnimation, x, y, Width, Height, SolidnessStatus);
                        break;
                    case MapObjectType.Oriented:
                        Utils.Loaders.TilemapObjectLoader.CreateOriented(map, DefaultAnimation, IdleAnimationSet, x, y, Width, Height, SolidnessStatus);
                        break;
                    case MapObjectType.Movable:
                        Utils.Loaders.TilemapObjectLoader.CreateMovable(map, DefaultAnimation, IdleAnimationSet, MovingAnimationSet, Speed, x, y, Width, Height, SolidnessStatus);
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}