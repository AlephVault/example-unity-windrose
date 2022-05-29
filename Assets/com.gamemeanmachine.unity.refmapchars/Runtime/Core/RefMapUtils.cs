using System;
using System.Collections.Generic;
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
            /// <summary>
            ///   Takes a target texture and the relevant parts of a character
            ///   that may be built using RefMap textures to build the final
            ///   character texture asset. All the parts are optional, even the
            ///   body of the character (e.g. for partial invisibility).
            /// </summary>
            /// <param name="target">The target texture to render everything into</param>
            /// <param name="skilledHandItem">The weapon or tool in the right / skilled hand</param>
            /// <param name="dumbHandItem">The shield or tool in the left / dumb hand</param>
            /// <param name="hat">The hat</param>
            /// <param name="cloak">The cloak</param>
            /// <param name="hair">The hair</param>
            /// <param name="hairTail">For long hairs, the "tail" of that hair</param>
            /// <param name="arms">The arms (clothes' extensions)</param>
            /// <param name="waist">The waist</param>
            /// <param name="shoulder">The shoulders (clothes' extensions)</param>
            /// <param name="necklace">The necklace</param>
            /// <param name="longShirt">The long shirt / tunic</param>
            /// <param name="chest">The chest (a light armor)</param>
            /// <param name="shirt">The shirt</param>
            /// <param name="boots">The boots / shoes</param>
            /// <param name="pants">The pants</param>
            /// <param name="body">The overall body</param>
            /// <param name="maskD">A mask that only shows down-oriented frames</param>
            /// <param name="maskLRU">A mask that does not show down-oriented frames</param>
            /// <param name="maskLR">A mask that only shows side-oriented frames</param>
            /// <param name="maskU">A mask that only shows up-oriented frames</param>
            /// <param name="bootsOverPants">Whether the boots should be rendered on top of the pants</param>
            /// <param name="necklaceOverLongShirt">
            ///   Whether the necklace should be rendered on top of the long shirt
            /// </param>
            /// <exception cref="ArgumentNullException">A null target is provided</exception>
            /// <exception cref="ArgumentException">A target not being 128x192 is provided</exception>
            public static void Paste(
                Texture target, RefMapSource skilledHandItem, RefMapSource dumbHandItem, RefMapSource hat,
                RefMapSource cloak, RefMapSource hair, RefMapSource hairTail, RefMapSource arms, RefMapSource waist,
                RefMapSource shoulder, RefMapSource necklace, RefMapSource longShirt, RefMapSource chest,
                RefMapSource shirt, RefMapSource boots, RefMapSource pants, RefMapSource body, Texture2D maskD,
                Texture2D maskLRU, Texture2D maskLR, Texture2D maskU, bool bootsOverPants = false,
                bool necklaceOverLongShirt = false
            )
            {
                if (target == null) throw new ArgumentNullException(nameof(target));
                if (target.width != 128 || target.height != 192)
                {
                    throw new ArgumentException("The target texture must be 128x192");
                }
                List<Texture2DSource> elements = new List<Texture2DSource>();
                AddIfPresent(elements, skilledHandItem?.ToTexture2DSource(maskU));
                AddIfPresent(elements, dumbHandItem?.ToTexture2DSource(maskLRU));
                AddIfPresent(elements, hairTail?.ToTexture2DSource(maskD));
                AddIfPresent(elements, cloak?.ToTexture2DSource(maskD));
                AddIfPresent(elements, body?.ToTexture2DSource());
                AddIfPresent(elements, (bootsOverPants ? pants : boots)?.ToTexture2DSource());
                AddIfPresent(elements, (bootsOverPants ? boots : pants)?.ToTexture2DSource());
                AddIfPresent(elements, shirt?.ToTexture2DSource());
                AddIfPresent(elements, chest?.ToTexture2DSource());
                AddIfPresent(elements, (necklaceOverLongShirt ? longShirt : necklace)?.ToTexture2DSource());
                AddIfPresent(elements, (necklaceOverLongShirt ? necklace : longShirt)?.ToTexture2DSource());
                AddIfPresent(elements, shoulder?.ToTexture2DSource());
                AddIfPresent(elements, waist?.ToTexture2DSource());
                AddIfPresent(elements, arms?.ToTexture2DSource());
                AddIfPresent(elements, hair?.ToTexture2DSource());
                AddIfPresent(elements, skilledHandItem?.ToTexture2DSource(maskLR));
                AddIfPresent(elements, cloak?.ToTexture2DSource(maskLRU));
                AddIfPresent(elements, hairTail?.ToTexture2DSource(maskLRU));
                AddIfPresent(elements, hat?.ToTexture2DSource());
                AddIfPresent(elements, dumbHandItem?.ToTexture2DSource(maskD));
                AddIfPresent(elements, skilledHandItem?.ToTexture2DSource(maskD));
                Textures.Paste2D(target, true, elements.ToArray());
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
                Texture target, RefMapComposite composite, Texture2D maskD, Texture2D maskLRU,
                Texture2D maskLR, Texture2D maskU
            ) {
                Paste(
                    target, composite?.SkilledHandItem, composite?.DumbHandItem, composite?.Hat,
                    composite?.Cloak, composite?.Hair, composite?.HairTail, composite?.Arms,
                    composite?.Waist, composite?.Shoulder, composite?.Necklace, composite?.LongShirt,
                    composite?.Chest, composite?.Shirt, composite?.Boots, composite?.Pants, composite?.Body,
                    maskD, maskLRU, maskLR, maskU, composite?.BootsOverPants ?? false,
                    composite?.NecklaceOverLongShirt ?? false
                );
            }

            private static void AddIfPresent(List<Texture2DSource> into, Texture2DSource element)
            {
                if (element != null) into.Add(element);
            }
        }
    }
}
