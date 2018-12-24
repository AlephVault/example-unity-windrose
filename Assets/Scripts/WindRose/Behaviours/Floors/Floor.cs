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

                public class ParentMustBeFloorLayerException : Types.Exception
                {
                    public ParentMustBeFloorLayerException() : base() { }
                    public ParentMustBeFloorLayerException(string message) : base(message) { }
                }

                private void Awake()
                {
                    try
                    {
                        Support.Utils.Layout.RequireComponentInParent<World.Layers.Floor.FloorLayer>(this);
                    }
                    catch(Types.Exception)
                    {
                        Destroy(gameObject);
                        throw new ParentMustBeFloorLayerException();
                    }
                }
            }
        }
    }
}
