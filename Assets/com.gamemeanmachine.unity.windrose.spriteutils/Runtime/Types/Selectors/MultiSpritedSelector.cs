using System;
using System.Collections.Generic;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A multi-state & sprited selector involves just one sprite per state.
            /// </summary>
            public class MultiSpritedSelector : MappedSpriteGridSelection<MultiSettings<Vector2Int>, MultiSettings<Sprite>>
            {
                public MultiSpritedSelector(SpriteGrid sourceGrid, MultiSettings<Vector2Int> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="Vector2Int"/>. Each value
                ///   must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions to select (mapped from type)</param>
                /// <returns>The sprites (mapped from type, and an idle state)</returns>
                protected override MultiSettings<Sprite> ValidateAndMap(SpriteGrid sourceGrid, MultiSettings<Vector2Int> selection)
                {
                    MultiSettings<Sprite> mapping = new MultiSettings<Sprite>();
                    foreach (KeyValuePair<string, Vector2Int> pair in selection)
                    {
                        mapping[pair.Key] = ValidateAndMapSprite(sourceGrid, pair.Value);
                    }

                    return mapping;
                }
            }
        }
    }
}