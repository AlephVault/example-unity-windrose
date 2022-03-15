using System;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   An oriented & sprited selector involves just one sprite per direction.
            /// </summary>
            public class RoseSpritedSelector : MappedSpriteGridSelection<RoseTuple<Vector2Int>, RoseTuple<Sprite>>
            {
                public RoseSpritedSelector(SpriteGrid sourceGrid, RoseTuple<Vector2Int> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   The validation and mapping involves a single sprite from a single
                ///   vector for each direction.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions rose tuple to select</param>
                /// <returns>The mapped sprites rose tuple</returns>
                protected override RoseTuple<Sprite> ValidateAndMap(SpriteGrid sourceGrid, RoseTuple<Vector2Int> selection)
                {
                    return new RoseTuple<Sprite>(
                        ValidateAndMapSprite(sourceGrid, selection.Up),
                        ValidateAndMapSprite(sourceGrid, selection.Left),
                        ValidateAndMapSprite(sourceGrid, selection.Right),
                        ValidateAndMapSprite(sourceGrid, selection.Down)
                    );
                }
            }
        }
    }
}
