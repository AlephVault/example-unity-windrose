using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Drops
        {
            using Support.Types;

            [RequireComponent(typeof(SortingGroup))]
            public class SimpleDropContainerRenderer : MonoBehaviour
            {
                /**
                 * This is a primary behaviour for the drop containers. Simple drop containers are
                 *   related to the DropLayer.
                 *   
                 * First of all, this component will require AT LEAST ONE SPRITE RENDERER*. It is better
                 *   if there are more than one, but at least ONE renderer is needed. Also, a sorting
                 *   group will also be required: it will help us ordering the attached renderers.
                 * 
                 * (* this will imply several children with sprite renderers)
                 * 
                 * Renderers will be ordered in an array. Say we have N renderers:
                 *   [0][1]...[N-1]
                 * 
                 * This drop container will have M elements, in 3 different cases:
                 * - M > N:
                 *   [0] will have the background bulk image sprite.
                 *   [1]..[N-1] will have images from positions [M-N]..[M-1].
                 * - M = N:
                 *   [0]..[N-1] will have the appropriate images.
                 * - M < N:
                 *   [0]..[M-1] will have the appropriate images.
                 *   [M]..[N-1] will be enabled = false.
                 */

                // The default bulk image. Ideally, you'll set this one on prefab.
                [SerializeField]
                private Sprite backgroundBulkImage;

                // The renderers to manage.
                private SpriteRenderer[] renderers;

                // The stuff being rendered.
                private SortedDictionary<int, Tuple<Sprite, string, object>> elements;

                private void Awake()
                {
                    elements = new SortedDictionary<int, Tuple<Sprite, string, object>>();
                    // Gets all the renderers and assigns them
                    //   a different sorting order
                    renderers = GetComponentsInChildren<SpriteRenderer>();
                    int order = 0;
                    Debug.Log(string.Format("Initializing SDC Renderer with {0} renderers ...", renderers.Length));
                    foreach(SpriteRenderer renderer in renderers)
                    {
                        renderer.sortingLayerID = 0;
                        renderer.sortingOrder = order++;
                        renderer.transform.localPosition = Vector3.zero;
                    }
                }

                private void DebugContentToRefresh()
                {
                    Debug.Log("Contents: " + string.Join(",", (from element in elements select string.Format("{0} -> ({1}: {2})", element.Key, element.Value.Second, element.Value.Third)).ToArray()));
                }

                private void Refresh()
                {
                    DebugContentToRefresh();
                    List<string> debugElements = new List<string>();

                    int currentSize = elements.Count;
                    int renderingSlots = renderers.Length;

                    if (currentSize > renderingSlots)
                    {
                        debugElements.Add(string.Format("background image"));
                        renderers[0].sprite = backgroundBulkImage;
                        int baseElementIndex = currentSize - renderingSlots;
                        for(int index = 1; index < renderingSlots; index++)
                        {
                            debugElements.Add(string.Format("{0} -> {1}", index, elements[index + baseElementIndex].Second));
                            renderers[index].sprite = elements[index + baseElementIndex].First;
                            renderers[index].enabled = true;
                        }
                    }
                    else if (currentSize == renderingSlots)
                    {
                        for (int index = 0; index < renderingSlots; index++)
                        {
                            debugElements.Add(string.Format("{0} -> {1}", index, elements[index].Second));
                            renderers[index].sprite = elements[index].First;
                            renderers[index].enabled = true;
                        }
                    }
                    else
                    {
                        for (int index = 0; index < currentSize; index++)
                        {
                            debugElements.Add(string.Format("{0} -> {1}", index, elements[index].Second));
                            renderers[index].sprite = elements[index].First;
                            renderers[index].enabled = true;
                        }
                        for(int index = currentSize; index < renderingSlots; index++)
                        {
                            renderers[index].enabled = false;
                        }
                    }

                    Debug.Log("Rendered contents: " + string.Join(",", debugElements.ToArray()));
                }

                public IEnumerable<KeyValuePair<int, Tuple<Sprite, string, object>>> Elements()
                {
                    return elements.AsEnumerable();
                }

                public void RefreshWithPutting(int index, Sprite icon, string caption, object quantity)
                {
                    elements[index] = new Tuple<Sprite, string, object>(icon, caption, quantity);
                    Refresh();
                }

                public void RefreshWithRemoving(int index)
                {
                    if (elements.ContainsKey(index))
                    {
                        elements.Remove(index);
                        Refresh();
                    }
                }

                public bool Empty()
                {
                    return elements.Count == 0;
                }
            }
        }
    }
}
