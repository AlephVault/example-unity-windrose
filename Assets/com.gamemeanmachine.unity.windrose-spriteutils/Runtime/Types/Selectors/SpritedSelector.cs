using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A simple sprited selector involves just one sprite.
            /// </summary>
            public class SpritedSelector : MappedSpriteGridSelection<Vector2Int, Sprite>
            {
                public SpritedSelector(SpriteGrid sourceGrid, Vector2Int selection) : base(sourceGrid, selection)
                {
                }

                /// <summary>
                ///   The validation and mapping process involves a single sprite
                ///   from a single vector.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The position to select</param>
                /// <returns>The mapped sprite</returns>
                protected override Sprite ValidateAndMap(SpriteGrid sourceGrid, Vector2Int selection)
                {
                    return ValidateAndMapSprite(sourceGrid, selection);
                }
            }
        }
    }
}