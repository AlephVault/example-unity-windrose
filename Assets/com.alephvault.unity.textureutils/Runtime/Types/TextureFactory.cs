using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace AlephVault.Unity.TextureUtils
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     This is not just a texture factory, but also a texture
        ///     pool. Textures hereby can be created and released at
        ///     will, but on release they might be not immediately
        ///     destroyed but kept in a pool (which stands for some
        ///     memory of size) until they are finally destroyed when
        ///     not anymore used.
        ///   </para>
        ///   <para>
        ///     This factory may be configured to one out of the three
        ///     texture types (2D - the usual one & default; 3D; CubeMap)
        ///     and also whether to create standard textures or objects
        ///     of type <see cref="RenderTexture"/>.
        ///   </para>
        /// </summary>
        public class TextureFactory
        {
            /// <summary>
            ///   The dimension of the texture to accept. Either 2D, 3D
            ///   or CubeMap only.
            /// </summary>
            public readonly TextureDimension Dimension;

            /// <summary>
            ///   Whether the textures are standard ones or instances of
            ///   <see cref="RenderTexture"/>.
            /// </summary>
            public readonly bool UsesRenderTextures;

            /// <summary>
            ///   The size, in bytes, of the pool. See <see cref="PoolSize"/>
            ///   for more details of the purpose of the pool size.
            /// </summary>
            private int sizeInBytes;

            /// <summary>
            ///   The maximum size of the pool. When controlling pool size,
            ///   previous stuff is only fixed if the size of the would-be
            ///   remaining elements, added together, still passes or equals
            ///   this pool size. This size is expressed in MB.
            /// </summary>
            public int PoolSize
            {
                get => sizeInBytes >> 20;
                set => sizeInBytes = value << 20;
            }

            /// <summary>
            ///   Creates an instance with certain type
            /// </summary>
            /// <param name="size">The size of the pool, in MB</param>
            /// <param name="dimension">The dimensions of the textures being created</param>
            /// <param name="renderTextures">Whether to use standard textures or <see cref="RenderTexture"/></param>
            /// <exception cref="ArgumentException">An invalid dimensions setting is specified</exception>
            public TextureFactory(
                int size = 50, TextureDimension dimension = TextureDimension.Tex2D,
                bool renderTextures = false
            )
            {
                switch (dimension)
                {
                    case TextureDimension.Tex2D:
                    case TextureDimension.Tex3D:
                    case TextureDimension.Cube:
                        Dimension = dimension;
                        break;
                    default:
                        throw new ArgumentException(
                            "Only 2D, 3D or CubeMap settings are allowed in factory dimension type"
                        );
                }
                UsesRenderTextures = renderTextures;
                PoolSize = size;
            }

            // Gets the size, in bytes, of a pixel in a texture format.
            private static int TextureFormatSize(TextureFormat format)
            {
                switch (format)
                {
                    case TextureFormat.Alpha8:
                    case TextureFormat.R8:
                        return 1;
                    case TextureFormat.ARGB4444:
                    case TextureFormat.RGBA4444:
                    case TextureFormat.RGB565:
                    case TextureFormat.R16:
                    case TextureFormat.RHalf:
                    case TextureFormat.RG16:
                        return 2;
                    case TextureFormat.RGB24:
                        return 3;
                    case TextureFormat.RGBA32:
                    case TextureFormat.ARGB32:
                    case TextureFormat.RGHalf:
                    case TextureFormat.RFloat:
                    case TextureFormat.RG32:
                        return 4;
                    case TextureFormat.RGB48:
                        return 6;
                    case TextureFormat.RGBA64:
                    case TextureFormat.RGFloat:
                        return 8;
                    case TextureFormat.RGBAFloat:
                        return 16;
                    default:
                        throw new ArgumentException($"This texture format is not yet supported: {format}");
                }
            }
            
            // Gets the size, in bytes, of a texture. This is approximate
            // and not necessarily the actual size. The involved memory is
            // the video one, and not the RAM one, since the mip maps are
            // loaded in VRAM, not in RAM.
            private static ulong TextureSize(Texture tx)
            {
                float mipMapFactor = tx.mipmapCount > 0 ? 1.3333333333f : 1;

                if (tx is Texture2D tx2d)
                {
                    ulong w = (ulong)tx2d.width;
                    ulong h = (ulong)tx2d.height;
                    ulong px = (ulong)TextureFormatSize(tx2d.format);
                    return (ulong)(px * w * h * mipMapFactor);
                }

                if (tx is Texture3D tx3d)
                {
                    ulong w = (ulong)tx3d.width;
                    ulong h = (ulong)tx3d.height;
                    ulong d = (ulong) tx3d.depth;
                    ulong px = (ulong)TextureFormatSize(tx3d.format);
                    return (ulong)(px * w * h * d * mipMapFactor);
                }

                if (tx is Cubemap cubeMap)
                {
                    ulong w = (ulong)cubeMap.width;
                    ulong h = (ulong)cubeMap.height;
                    ulong px = (ulong)TextureFormatSize(cubeMap.format);
                    return (ulong)(px * w * h * 6 * mipMapFactor);
                }

                throw new ArgumentException(
                    $"The given texture {tx} is not of an allowed type (2D, 3D, CubeMap)"
                );
            }
        }
    }
}