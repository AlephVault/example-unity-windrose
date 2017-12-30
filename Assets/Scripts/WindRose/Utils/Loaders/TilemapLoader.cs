using System;
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
            public class TilemapLoader
            {
                /* Width of the map, expressed in cells. Guaranteed to be between 1 and 100 */
                public readonly uint Width;

                /* Height of the map, expressed in cells. Guaranteed to be between 1 and 100 */
                public readonly uint Height;

                /* Width/Height of a tile, expressed in pixels (pixelsToUnit conversion rate) */
                public readonly uint TileSize;

                public TilemapLoader(uint width, uint height, uint tileSize = 32)
                {
                    if (width * height * tileSize == 0)
                    {
                        throw new ArgumentException("width, height, and tile size must all be > 0");
                    }

                    Width = width;
                    Height = height;
                    TileSize = tileSize;
                }

                /**
                 * Entirely loads a map according to its layers, and created the needed GameObjects.
                 */
                public void Load(GameObject holder, List<TilemapLayer> layers)
                {
                    if (holder == null)
                    {
                        throw new ArgumentNullException("holder");
                    }
                    if (holder.GetComponent<Behaviors.Map>() != null)
                    {
                        throw new ArgumentException("The specified holder already has a loaded map", "holder");
                    }

                    if (layers == null)
                    {
                        throw new ArgumentNullException("layers", "The layers list must not be null");
                    }
                    foreach (TilemapLayer layer in layers)
                    {
                        if (layer.Width != Width || layer.Height != Height)
                        {
                            throw new ArgumentException("Loaded layers must match width/height with the map's width/height", "layers");
                        }
                    }

                    Bitmask blockMask = InitBlockMask();
                    RenderTexture target = InitRenderTexture();

                    // Before processing, we ensure enabling the RenderTexture and related stuff, and change the RT target
                    RenderTexture oldTarget = RenderTexture.active;
                    RenderTexture.active = target;
                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, target.width, target.height, 0);
                    GL.Clear(true, true, new Color(0, 0, 0, 0));

                    // Now we execute the processing
                    ProcessLayers(layers, target, blockMask);

                    // After processing, we dump the contents to the final Texture and restore the RT target
                    Texture2D finalTexture = DumpTexture(target);
                    Texture2D finalBlockMask = DumpBlockMask(blockMask);
                    GL.PopMatrix();
                    RenderTexture.active = oldTarget;

                    // And then, we create/init the needed GameObjects
                    AddTilemap(holder, finalBlockMask);
                    GameObject background = CreateBackground(finalTexture);
                    background.transform.parent = holder.transform;
                    background.transform.localPosition = Vector3.zero;
                }

                /**
                 * Initializes the raw block mask filles with char '0', with dimensions Width x Height.
                 */
                private Bitmask InitBlockMask()
                {
                    return new Bitmask(Width, Height);
                }

                /**
                 * Initializes the render texture, with dimensions Width*TileSize x Height*TileSize
                 */
                private RenderTexture InitRenderTexture()
                {
                    return new RenderTexture((int)(Width * TileSize), (int)(Height * TileSize), 16, RenderTextureFormat.ARGB32);
                }

                /**
                 * Processes the involved layers by executing their rendering in the target texture and raw block mask
                 */
                private void ProcessLayers(List<TilemapLayer> layers, RenderTexture target, Bitmask blockMask)
                {
                    Action<uint, uint, Texture2D, Rect> painter = (uint x, uint y, Texture2D texture, Rect normalizedRect) => {
                        Graphics.DrawTexture(new Rect(x * TileSize, y * TileSize, TileSize, TileSize), texture, normalizedRect, 0, 0, 0, 0);
                    };

                    foreach (TilemapLayer layer in layers)
                    {
                        layer.Process(painter, blockMask);
                    }
                }

                /**
                 * Dumps the RenderTexture contents into a new Texture2D
                 */
                private Texture2D DumpTexture(RenderTexture source)
                {
                    Texture2D result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
                    result.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
                    result.Apply();
                    return result;
                }

                /**
                 * Dumps the block mask as a string, ready to be used by a Map behavior
                 */
                private Texture2D DumpBlockMask(Bitmask blockMask)
                {
                    return blockMask.Export();
                }

                /**
                 * Adds a Tilemap to an existing GameObject. The behavior is initialized with the
                 *   loader's parameters and the block mask.
                 */
                private void AddTilemap(GameObject holder, Texture2D blockMask)
                {
                    Layout.AddComponent<Behaviors.Map>(holder, new Dictionary<string, object>() {
                        { "width", Width },
                        { "height", Height },
                        { "blockMask", blockMask },
                        { "maskApplicationOffsetX", 0 },
                        { "maskApplicationOffsetY", 0 },
                    });
                }

                /**
                 * Creates a background using a texture and specifying a certain ratio of pixels per unit.
                 * The pivot for the new sprite will be at (0, 0) with respect to the texture.
                 * The texture will be fully used as the sprite's source.
                 */
                private GameObject CreateBackground(Texture2D texture)
                {
                    GameObject background = new GameObject();
                    SpriteRenderer spriteRenderer = background.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 1), TileSize);
                    return background;
                }
            }
        }
    }
}
