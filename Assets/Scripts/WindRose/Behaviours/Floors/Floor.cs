using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Floors
        {
            [RequireComponent(typeof(Tilemap))]
            [RequireComponent(typeof(TilemapRenderer))]
            [RequireComponent(typeof(Support.Behaviours.Normalized))]
            class Floor : MonoBehaviour
            {
                /**
                 * A floor can only exist in a floor layer, and will require a tilemap.
                 * It will also be normalized in position on initialization.
                 */
                private void Awake()
                {
                    Support.Utils.Layout.RequireComponentInParent<World.Layers.FloorLayer>(this);
                }
            }
        }
    }
}
