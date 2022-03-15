using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            ///   An oriented & animated selector involves a list of sprites per direction.
            /// </summary>
            public class RoseAnimatedSelector : MappedSpriteGridSelection<RoseTuple<ReadOnlyCollection<Vector2Int>>, RoseTuple<ReadOnlyCollection<Sprite>>>
            {
                public RoseAnimatedSelector(SpriteGrid sourceGrid, RoseTuple<ReadOnlyCollection<Vector2Int>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   The validation and mapping process involves a list of sprites
                ///   from a list of vector for each direction.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions rose tuple to select</param>
                /// <returns>The mapped sprites rose tuple</returns>
                protected override RoseTuple<ReadOnlyCollection<Sprite>> ValidateAndMap(SpriteGrid sourceGrid, RoseTuple<ReadOnlyCollection<Vector2Int>> selection)
                {
                    if (selection.Up == null || selection.Left == null || selection.Right == null ||
                        selection.Down == null)
                    {
                        throw new ArgumentException(
                            $"A null value was given to the sprite list rose tuple in one of the fields"
                        );
                    }
                    
                    return new RoseTuple<ReadOnlyCollection<Sprite>>(
                        new ReadOnlyCollection<Sprite>(
                            (IList<Sprite>) from position in selection.Up select ValidateAndMapSprite(sourceGrid, position)
                        ),
                        new ReadOnlyCollection<Sprite>(
                            (IList<Sprite>) from position in selection.Left select ValidateAndMapSprite(sourceGrid, position)
                        ),
                        new ReadOnlyCollection<Sprite>(
                            (IList<Sprite>) from position in selection.Right select ValidateAndMapSprite(sourceGrid, position)
                        ),
                        new ReadOnlyCollection<Sprite>(
                            (IList<Sprite>) from position in selection.Down select ValidateAndMapSprite(sourceGrid, position)
                        )
                    );
                }
            }
        }
    }
}