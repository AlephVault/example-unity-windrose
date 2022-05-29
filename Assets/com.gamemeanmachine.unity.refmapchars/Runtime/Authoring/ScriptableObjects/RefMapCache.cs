using System;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.TextureUtils.Types;
using GameMeanMachine.Unity.RefMapChars.Core;
using GameMeanMachine.Unity.RefMapChars.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            /// <summary>
            ///   This cache builds and keeps textures of size
            ///   128x192 to be composed and used as character
            ///   displays.
            /// </summary>
            public class RefMapCache : ScriptableObject
            {
                /// <summary>
                ///   The size of the last second rescue queue.
                ///   A negative value will imply a shift until
                ///   the size is 0 (same as choosing 0).
                /// </summary>
                [SerializeField]
                private int lastSecondRescueSize = 20;

                /// <summary>
                ///   The pixels per unit for each generated sprite.
                /// </summary>
                [SerializeField]
                private float pixelsPerUnit = 32;

                /// <summary>
                ///   The format for the render textures. Ensure this
                ///   plays nice with <see cref="renderDepth"/> and
                ///   <see cref="finalFormat"/>.
                /// </summary>
                [SerializeField]
                private RenderTextureFormat renderFormat = RenderTextureFormat.ARGB32;

                /// <summary>
                ///   The format for the final textures. Ensure this
                ///   plays nice with <see cref="renderFormat"/>.
                /// </summary>
                [SerializeField]
                private TextureFormat finalFormat = TextureFormat.ARGB32;

                /// <summary>
                ///   The render depth, in pixels. Ensure this plays
                ///   nice with <see cref="renderFormat"/>.
                /// </summary>
                [SerializeField]
                private int renderDepth;

                /// <summary>
                ///   A mask that only shows frames pointing to the
                ///   down side. It is recommended to use the bundled
                ///   texture "mask-d.png" in this property.
                /// </summary>
                [SerializeField]
                private Texture2D maskD;

                /// <summary>
                ///   A mask that does not show frames pointing to
                ///   the down side. It is recommended to use the
                ///   bundled texture "mask-lru.png" in this property.
                /// </summary>
                [SerializeField]
                private Texture2D maskLRU;
                
                /// <summary>
                ///   A mask that only shows frames pointing to the
                ///   left and right sides. It is recommended to use
                ///   the bundled texture "mask-lr.png" in this property.
                /// </summary>
                [SerializeField]
                private Texture2D maskLR;
                
                /// <summary>
                ///   A mask that only shows frames pointing up. It is
                ///   recommended to use the bundled texture "mask-u.png"
                ///   in this property.
                /// </summary>
                [SerializeField]
                private Texture2D maskU;
                
                // The texture pool for this cache.
                private TexturePool<string, Texture2D> texturePool;
                
                // The sprite pool for this cache. It extracts
                // stuff from the texture pool.
                private IdentifiedSpriteGridPool<string> spritePool;

                private const int TextureWidth = 128;
                private const int TextureHeight = 192;
                private const int FrameWidth = 32;
                private const int FrameHeight = 48;
                
                private Texture2D ToTexture2D(RenderTexture rTex)
                {
                    Texture2D tex = new Texture2D(TextureWidth, TextureHeight, finalFormat, false);
                    // ReadPixels looks at the active RenderTexture.
                    RenderTexture.active = rTex;
                    tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
                    tex.Apply();
                    return tex;
                }

                private SpriteGrid GridFromTexture(string key, Func<Texture2D> onAbsent)
                {
                    Texture2D usedTexture = texturePool.Use(key, onAbsent, (t) => Destroy(t));
                    return spritePool.Get(key, () =>
                    {
                        return new Tuple<Texture2D, Rect?, uint, uint, float, Action, Action>(
                            usedTexture, null, FrameWidth, FrameHeight, pixelsPerUnit, () => {}, () =>
                            {
                                texturePool?.Release(key);
                            }
                        );
                    });
                }

                /// <summary>
                ///   Gets the sprite grid associated to a composite. It is
                ///   recommended to use the resulting grid as quick as possible.
                /// </summary>
                /// <param name="composite">The composite to get the grid for</param>
                /// <returns>The appropriate sprite grid</returns>
                public SpriteGrid Get(IRefMapComposite composite)
                {
                    return GridFromTexture(composite.Hash(), () =>
                    {
                        RenderTexture target = new RenderTexture(
                            TextureWidth, TextureHeight, renderDepth, renderFormat
                        );
                        RefMapUtils.Paste(
                            target, composite, maskD, maskLRU, maskLR, maskU
                        );
                        return ToTexture2D(target);
                    });
                }

                /// <summary>
                ///   Gets the sprite grid associated to a simple composite. It is
                ///   recommended to use the resulting grid as quick as possible.
                /// </summary>
                /// <param name="composite">The composite to get the grid for</param>
                /// <returns>The appropriate sprite grid</returns>
                public SpriteGrid Get(IRefMapSimpleComposite composite)
                {
                    return GridFromTexture(composite.Hash(), () =>
                    {
                        RenderTexture target = new RenderTexture(
                            TextureWidth, TextureHeight, renderDepth, renderFormat
                        );
                        RefMapUtils.Paste(
                            target, composite, maskD, maskLRU, maskLR, maskU
                        );
                        return ToTexture2D(target);
                    });
                }
                
                private void OnEnable()
                {
                    texturePool = new TexturePool<string, Texture2D>(lastSecondRescueSize);
                    spritePool = new IdentifiedSpriteGridPool<string>(lastSecondRescueSize);
                }

                private void OnDisable()
                {
                    spritePool = null;
                    texturePool = null;
                }
            }
        }
    }    
}
