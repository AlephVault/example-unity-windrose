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
                    public class ParentMustBeMapException : Types.Exception
                    {
                        public ParentMustBeMapException() : base() { }
                        public ParentMustBeMapException(string message) : base(message) { }
                    }

                    private SortingGroup sortingGroup;
                    public Map Map { get; private set; }

                    protected virtual void Awake()
                    {
                        sortingGroup = GetComponent<SortingGroup>();
                        try
                        {
                            Map = Support.Utils.Layout.RequireComponentInParent<Map>(this);
                        }
                        catch (Types.Exception)
                        {
                            Destroy(gameObject);
                            throw new ParentMustBeMapException();
                        }
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
