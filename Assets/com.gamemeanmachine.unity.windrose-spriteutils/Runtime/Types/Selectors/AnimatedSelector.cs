using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.SpriteUtils.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A simple & animated selector involves a list of sprites.
            /// </summary>
            public class AnimatedSelector : MappedSpriteGridSelection<ReadOnlyCollection<Vector2Int>, ReadOnlyCollection<Sprite>>
            {
                public AnimatedSelector(SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   The validation and mapping process involves a list of sprites
                ///   from a list of vector.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions to select</param>
                /// <returns>The mapped sprites</returns>
                protected override ReadOnlyCollection<Sprite> ValidateAndMap(SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> selection)
                {
                    return new ReadOnlyCollection<Sprite>(
                        (IList<Sprite>) from position in selection select ValidateAndMapSprite(sourceGrid, position)
                    );
                }
            }
        }
    }
}