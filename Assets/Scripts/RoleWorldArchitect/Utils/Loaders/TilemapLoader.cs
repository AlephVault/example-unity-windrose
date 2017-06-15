using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Utils
    {
        namespace Loaders
        {
            public class TilemapLoader
            {
                /* Width of the map, expressed in cells. Guaranteed to be between 1 and 100 */
                public readonly uint Width;

                /* Height of the map, expressed in cells. Guaranteed to be between 1 and 100 */
                public readonly uint Height;

                /* Width/Height of a tile, expressed in pixels (pixelsToUnit conversion rate) */
                public readonly uint TileSize;

                /* Free-marking char (i.e. char for a "cleared" bit) */
                public readonly char FreeMarkingChar;

                /* Used-marking char (i.e. char for presence or blocking bit) */
                public readonly char UsedMarkingChar;

                public TilemapLoader(uint width, uint height, uint tileSize = 32, char freeMarkingChar = '0', char usedMarkingChar = '1')
                {
                    if (width * height * tileSize == 0)
                    {
                        throw new ArgumentException("width, height, and tile size must all be > 0");
                    }

                    Width = width;
                    Height = height;
                    TileSize = tileSize;
                    FreeMarkingChar = freeMarkingChar;
                    UsedMarkingChar = usedMarkingChar;
                }

                /**
                 * Entirely loads a map according to its layers, and created the needed GameObjects.
                 */
                public GameObject Load(List<TilemapLayer> layers)
                {
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

                    char[,] blockMask = InitBlockMask();
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
                    string finalBlockMask = DumpBlockMask(blockMask);
                    GL.PopMatrix();
                    RenderTexture.active = oldTarget;

                    // And then, we create the damn GameObjects to be returned
                    GameObject tileMap = CreateTilemap(finalBlockMask);
                    GameObject background = CreateBackground(finalTexture);
                    background.transform.parent = tileMap.transform;
                    background.transform.localPosition = Vector3.zero;
                    return tileMap;
                }

                /**
                 * Initializes the raw block mask filles with char '0', with dimensions Width x Height.
                 */
                private char[,] InitBlockMask()
                {
                    char[,] blockMask = new char[Width, Height];
                    for (uint x = 0; x < Width; x++)
                    {
                        for (uint y = 0; y < Height; y++)
                        {
                            blockMask[x, y] = FreeMarkingChar;
                        }
                    }
                    return blockMask;
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
                private void ProcessLayers(List<TilemapLayer> layers, RenderTexture target, char[,] blockMask)
                {
                    Action<uint, uint> setter = (uint x, uint y) => { blockMask[x, y] = UsedMarkingChar; };
                    Action<uint, uint> clearer = (uint x, uint y) => { blockMask[x, y] = FreeMarkingChar; };
                    Action<uint, uint> inverter = (uint x, uint y) => { blockMask[x, y] = (blockMask[x, y] == FreeMarkingChar) ? UsedMarkingChar : FreeMarkingChar; };
                    Action<uint, uint, Texture2D, Rect> painter = (uint x, uint y, Texture2D texture, Rect normalizedRect) => {
                        Graphics.DrawTexture(new Rect(x * TileSize, y * TileSize, TileSize, TileSize), texture, normalizedRect, 0, 0, 0, 0);
                    };

                    foreach (TilemapLayer layer in layers)
                    {
                        layer.Process(painter, setter, clearer, inverter);
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
                private string DumpBlockMask(char[,] blockMask)
                {
                    char[] builtString = new char[Width * (Height + 1) - 1];
                    int idx = 0;
                    for(uint y = 0; y < Height; y++)
                    {
                        // Dumping each row is straightforward.
                        for(uint x = 0; x < Width; x++)
                        {
                            builtString[idx++] = blockMask[x, y];
                        }
                        // After each row, except for the last (H - 1), we add a newline character.
                        if (y < Height - 1)
                        {
                            builtString[idx++] = '\n';
                        }
                    }
                    return new string(builtString);
                }

                /**
                 * Creates an instance of a Tilemap. The behavior is initialized with these parameters.
                 */
                private GameObject CreateTilemap(string blockMask)
                {
                    GameObject tilemap = new GameObject();
                    Layout.AddComponent<Behaviors.Map>(tilemap, new Dictionary<string, object>() {
                        { "width", Width },
                        { "height", Height },
                        { "blockMask", blockMask },
                        { "freeMarkingChar", FreeMarkingChar },
                        { "blockMarkingChar", UsedMarkingChar },
                        { "maskApplicationOffsetX", 0 },
                        { "maskApplicationOffsetY", 0 },
                    });
                    return tilemap;
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
                    spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, TileSize);
                    return background;
                }
            }
        }
    }
}
