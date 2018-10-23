using UnityEngine;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                /**
                 * Map layers know their position with respect to the map.
                 * They sort themselves in the DEFAULT layer, but this will
                 *   occur in the context of their parent Map.
                 */
                [RequireComponent(typeof(Support.Behaviours.Normalized))]
                [RequireComponent(typeof(SortingGroup))]
                public abstract class MapLayer : MonoBehaviour
                {
                    private SortingGroup sortingGroup;

                    protected virtual void Awake()
                    {
                        sortingGroup = GetComponent<SortingGroup>();
                        Support.Utils.Layout.RequireComponentInParent<Map>(this);
                    }

                    protected virtual void Start()
                    {
                        sortingGroup.sortingLayerID = 0;
                        sortingGroup.sortingOrder = GetSortingOrder();
                    }

                    protected abstract int GetSortingOrder();
                }
            }
        }
    }
}
