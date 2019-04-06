using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                namespace Highlight
                {
                    /// <summary>
                    ///   This layer is only used in edit mode, and will be removed when running.
                    ///   It will add an outline or background to correctly tell you where will
                    ///     the map be in terms of bounds.
                    /// </summary>
                    [RequireComponent(typeof(SpriteRenderer))]
                    [ExecuteInEditMode]
                    class HighlightLayer : MapLayer
                    {
                        [SerializeField]
                        private Color32 highlightColor1 = Color.white;

                        [SerializeField]
                        private Color32 highlightColor2 = Color.gray;

                        protected override int GetSortingOrder()
                        {
                            return 0;
                        }

                        protected override void Start()
                        {
                            if (Application.isPlaying)
                            {
                                Destroy(gameObject);
                            }
                            else
                            {
                                base.Start();
                            }
                        }

                        // Checks if the parent map has another highlight layer
                        //   than this one. If so, immediately destroy this one.
                        private bool HasAnotherHighlightLayer(Map parentMap)
                        {
                            foreach(Transform child in parentMap.transform)
                            {
                                HighlightLayer highlightLayer = child.GetComponent<HighlightLayer>();
                                if (highlightLayer != null && highlightLayer != this) return true;
                            }
                            return false;
                        }

                        // Creates a background sprite with the given colors. It will be initially:
                        //   XY
                        //   YX
                        // And with a size of 1x1 squared game units, but will repeat by width/height.
                        private void InitBackgroundImage(Map parentMap, Vector3 scaleBy)
                        {
                            // Destroy all children of this layer.
                            List<Transform> children = new List<Transform>();
                            foreach(Transform child in transform)
                            {
                                children.Add(child);
                            }
                            foreach(Transform child in children)
                            {
                                DestroyImmediate(child.gameObject);
                            }

                            // Create the quad and add it as the only child.
                            Texture2D texture = new Texture2D(2, 2);
                            texture.SetPixels32(0, 0, 2, 2, new Color32[] { highlightColor1, highlightColor2, highlightColor2, highlightColor1 });
                            texture.wrapMode = TextureWrapMode.Repeat;
                            texture.filterMode = FilterMode.Point;
                            texture.Apply(false);
                            GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                            quadObject.transform.parent = transform;
                            quadObject.transform.localRotation = Quaternion.identity;
                            Vector3 localScale = new Vector3(parentMap.Width, parentMap.Height, 1);
                            localScale.Scale(scaleBy);
                            quadObject.transform.localScale = localScale;
                            quadObject.transform.localPosition = localScale / 2;
                            MeshRenderer quadMesh = quadObject.GetComponent<MeshRenderer>();
                            Material tiledMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
                            tiledMaterial.mainTexture = texture;
                            tiledMaterial.mainTextureScale = new Vector2(parentMap.Width, parentMap.Height);
                            quadMesh.material = tiledMaterial;
                        }

                        // This one will only be executed in Editor mode.
                        protected override void Update()
                        {
                            base.Update();
                            if (Application.isPlaying) return;
                            // Normalize the transform.
                            transform.localScale = Vector3.one;
                            transform.localRotation = Quaternion.identity;
                            transform.localPosition = Vector3.zero;
                            // Get the parent map and the current object's sprite renderer.
                            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                            Map map = transform.parent ? transform.parent.GetComponent<Map>() : null;
                            if (map)
                            {
                                // If a sibling highlight layer is found, destroy this one.
                                // Otherwise, update this one.
                                if (HasAnotherHighlightLayer(map))
                                {
                                    DestroyImmediate(gameObject);
                                }
                                else
                                {
                                    // Updating implies taking map's cellSize (*) floor layer's scale
                                    //   to consider the tridimensional scale factor of a new sprite.
                                    // The new sprite will be generated and scaled with that.
                                    Floor.FloorLayer floorLayer = map.GetComponent<Floor.FloorLayer>();
                                    Vector3 cellSize = new Vector3();
                                    cellSize.Set(map.CellSize.x, map.CellSize.y, map.CellSize.z);
                                    if (floorLayer) cellSize.Scale(floorLayer.transform.localScale);
                                    InitBackgroundImage(map, cellSize);
                                }
                            }
                            else
                            {
                                renderer.enabled = false;
                            }
                        }

                        public void ForceUpdate()
                        {
                            Update();
                        }
                    }
                }
            }
        }
    }
}
