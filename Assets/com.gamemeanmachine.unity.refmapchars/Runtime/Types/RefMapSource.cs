using System;
using AlephVault.Unity.TextureUtils.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Types
    {
        /// <summary>
        ///   A RefMap source uses a texture, and perhaps an offset
        ///   inside that texture, to later apply a mask and use it
        ///   to render a composed RefMap texture.
        /// </summary>
        [Serializable]
        public class RefMapSource
        {
            /// <summary>
            ///   The source texture.
            /// </summary>
            public Texture2D Texture;

            /// <summary>
            ///   The offset inside the texture.
            /// </summary>
            public Vector2Int Offset;

            /// <summary>
            ///   Makes a paste source from this RefMap source
            ///   by specifying a custom mask.
            /// </summary>
            /// <param name="mask"></param>
            /// <returns></returns>
            public Texture2DSource ToTexture2DSource(Texture2D mask = null)
            {
                return new Texture2DSource
                {
                    Texture = Texture,
                    Offset = Offset,
                    Mask = mask,
                };
            }
        }
    }
}