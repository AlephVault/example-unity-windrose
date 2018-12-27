﻿using System.Linq;
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
            [RequireComponent(typeof(SpriteRenderer))]
            public class SimpleDropContainerRenderer : MonoBehaviour
            {
                /**
                 * This is a primary behaviour for the drop containers. Simple drop containers are
                 *   related to the DropLayer.
                 *   
                 * First of all, this component will require AT LEAST ONE SPRITE RENDERER. It is better
                 *   if there are more than one, but at least ONE renderer is needed. Also, a sorting
                 *   group will also be required: it will help us ordering the attached renderers.
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
                    renderers = GetComponents<SpriteRenderer>();
                    int order = 0;
                    foreach(SpriteRenderer renderer in renderers)
                    {
                        renderer.sortingLayerID = 0;
                        renderer.sortingOrder = order++;
                    }
                }

                private void Refresh()
                {
                    int currentSize = elements.Count;
                    int maxSize = renderers.Length;

                    if (currentSize > maxSize)
                    {
                        renderers[0].sprite = backgroundBulkImage;
                        int baseElementIndex = currentSize - maxSize - 1;
                        for(int index = 1; index < maxSize; index++)
                        {
                            renderers[index].sprite = elements[index + baseElementIndex].First;
                            renderers[index].enabled = true;
                        }
                    }
                    else if (currentSize == maxSize)
                    {
                        for (int index = 0; index < maxSize; index++)
                        {
                            renderers[index].sprite = elements[index].First;
                            renderers[index].enabled = true;
                        }
                    }
                    else
                    {
                        for (int index = 0; index < currentSize; index++)
                        {
                            renderers[index].sprite = elements[index].First;
                            renderers[index].enabled = true;
                        }
                        for(int index = currentSize; index < maxSize; index++)
                        {
                            renderers[index].sprite = elements[index].First;
                            renderers[index].enabled = false;
                        }
                    }
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
