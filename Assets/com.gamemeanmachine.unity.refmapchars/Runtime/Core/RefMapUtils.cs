using System;
using System.Collections.Generic;
using System.Linq;
using AlephVault.Unity.TextureUtils.Types;
using AlephVault.Unity.TextureUtils.Utils;
using GameMeanMachine.Unity.RefMapChars.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Core
    {
        /// <summary>
        ///   The core function is implemented here, which takes the
        ///   textures (all of them are separate parts) that are to
        ///   be merged into a single texture, using certain rendering
        ///   order, to generate a final character. The textures can
        ///   be given separately or as part of a composite object.
        /// </summary>
        public static class RefMapUtils
        {
            private static void Paste(Texture target, params Texture2DSource[] sources)
            {
                if (target == null) throw new ArgumentNullException(nameof(target));
                if (target.width != 128 || target.height != 192)
                {
                    throw new ArgumentException("The target texture must be 128x192");
                }
                Textures.Paste2D(target, true, (
                    from source in sources
                    where source != null select source
                ).ToArray());
            }
            
            /// <summary>
            ///   Takes a target texture and a composite object to paste all the
            ///   parts (in the appropriate order and using the appropriate masks)
            ///   into it, to form a new texture (to be cached and used).
            /// </summary>
            /// <param name="target">The target texture to render everything into</param>
            /// <param name="composite">The composite object to render</param>
            /// <param name="maskD">A mask that only shows down-oriented frames</param>
            /// <param name="maskLRU">A mask that does not show down-oriented frames</param>
            /// <param name="maskLR">A mask that only shows side-oriented frames</param>
            /// <param name="maskU">A mask that only shows up-oriented frames</param>
            public static void Paste(
                Texture target, IRefMapComposite composite, Texture2D maskD, Texture2D maskLRU,
                Texture2D maskLR, Texture2D maskU
            )
            {
                Paste(
                    target,
                    composite?.SkilledHandItem?.ToTexture2DSource(maskU),
                    composite?.DumbHandItem?.ToTexture2DSource(maskLRU),
                    composite?.HairTail?.ToTexture2DSource(maskD),
                    composite?.Cloak?.ToTexture2DSource(maskD),
                    composite?.Body?.ToTexture2DSource(),
                    (composite?.BootsOverPants ?? false ?
                        composite.Pants : composite?.Boots)?.ToTexture2DSource(),
                    (composite?.BootsOverPants ?? false ?
                        composite.Boots : composite?.Pants)?.ToTexture2DSource(),
                    composite?.Shirt?.ToTexture2DSource(),
                    composite?.Chest?.ToTexture2DSource(),
                    (composite?.NecklaceOverLongShirt ?? false ?
                        composite.LongShirt : composite?.Necklace)?.ToTexture2DSource(),
                    (composite?.NecklaceOverLongShirt ?? false ?
                        composite.Necklace : composite?.LongShirt)?.ToTexture2DSource(),
                    composite?.Shoulder?.ToTexture2DSource(),
                    composite?.Waist?.ToTexture2DSource(),
                    composite?.Arms?.ToTexture2DSource(),
                    composite?.Hair?.ToTexture2DSource(),
                    composite?.SkilledHandItem?.ToTexture2DSource(maskLR),
                    composite?.Cloak?.ToTexture2DSource(maskLRU),
                    composite?.HairTail?.ToTexture2DSource(maskLRU),
                    composite?.Hat?.ToTexture2DSource(),
                    composite?.DumbHandItem?.ToTexture2DSource(maskD),
                    composite?.SkilledHandItem?.ToTexture2DSource(maskD)
                );
            }

            /// <summary>
            ///   Takes a target texture and a simple composite object to paste all the
            ///   parts (in the appropriate order and using the appropriate masks) into
            ///   it, to form a new texture (to be cached and used). In this mode, the
            ///   clothes make use of less iterations since they come in a single sprite
            ///   instead of broken in parts.
            /// </summary>
            /// <param name="target">The target texture to render everything into</param>
            /// <param name="composite">The simple composite object to render</param>
            /// <param name="maskD">A mask that only shows down-oriented frames</param>
            /// <param name="maskLRU">A mask that does not show down-oriented frames</param>
            /// <param name="maskLR">A mask that only shows side-oriented frames</param>
            /// <param name="maskU">A mask that only shows up-oriented frames</param>
            public static void Paste(
                Texture target, IRefMapSimpleComposite composite, Texture2D maskD, Texture2D maskLRU,
                Texture2D maskLR, Texture2D maskU
            )
            {
                Paste(
                    target,
                    composite?.SkilledHandItem?.ToTexture2DSource(maskU),
                    composite?.DumbHandItem?.ToTexture2DSource(maskLRU),
                    composite?.HairTail?.ToTexture2DSource(maskD),
                    composite?.Body?.ToTexture2DSource(),
                    composite?.Cloth?.ToTexture2DSource(),
                    composite?.Necklace?.ToTexture2DSource(),
                    composite?.Hair?.ToTexture2DSource(),
                    composite?.SkilledHandItem?.ToTexture2DSource(maskLR),
                    composite?.HairTail?.ToTexture2DSource(maskLRU),
                    composite?.Hat?.ToTexture2DSource(),
                    composite?.DumbHandItem?.ToTexture2DSource(maskD),
                    composite?.SkilledHandItem?.ToTexture2DSource(maskD)
                );
            }
        }
    }
}
