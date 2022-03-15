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
            public class MultiSpritedSelector : MappedSpriteGridSelection<Dictionary<Type, Vector2Int>, Dictionary<Type, Sprite>>
            {
                public MultiSpritedSelector(SpriteGrid sourceGrid, Dictionary<Type, Vector2Int> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="Vector2Int"/>. Each value
                ///   must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions to select (mapped from type)</param>
                /// <returns>The sprites (mapped from type)</returns>
                protected override Dictionary<Type, Sprite> ValidateAndMap(SpriteGrid sourceGrid, Dictionary<Type, Vector2Int> selection)
                {
                    Dictionary<Type, Sprite> result = new Dictionary<Type, Sprite>();
                    foreach (KeyValuePair<Type, Vector2Int> pair in selection)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }
                        result[pair.Key] = ValidateAndMapSprite(sourceGrid, pair.Value);
                    }

                    return result;
                }
            }
        }
    }
}