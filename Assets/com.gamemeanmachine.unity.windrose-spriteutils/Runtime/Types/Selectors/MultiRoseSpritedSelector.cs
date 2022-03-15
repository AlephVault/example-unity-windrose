using System;
using System.Collections.Generic;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;

namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A multi-state oriented & sprited selector involves just one sprite per state and direction.
            /// </summary>
            public class MultiRoseSpritedSelector : MappedSpriteGridSelection<Dictionary<Type, RoseTuple<Vector2Int>>, Dictionary<Type, RoseTuple<Sprite>>>
            {
                public MultiRoseSpritedSelector(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<Vector2Int>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="RoseTuple{Vector2Int}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The rose tuples of positions to select (mapped from type)</param>
                /// <returns>The rose tuples of sprites (mapped from type)</returns>
                protected override Dictionary<Type, RoseTuple<Sprite>> ValidateAndMap(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<Vector2Int>> selection)
                {
                    Dictionary<Type, RoseTuple<Sprite>> result = new Dictionary<Type, RoseTuple<Sprite>>();
                    foreach (KeyValuePair<Type, RoseTuple<Vector2Int>> pair in selection)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }

                        if (pair.Value == null) throw new ArgumentException(
                            $"A null value was given to the sprite rose-tuple dictionary in key: {pair.Key.FullName}"
                        );
                        result[pair.Key] = new RoseTuple<Sprite>(
                            ValidateAndMapSprite(sourceGrid, pair.Value.Up),
                            ValidateAndMapSprite(sourceGrid, pair.Value.Left),
                            ValidateAndMapSprite(sourceGrid, pair.Value.Right),
                            ValidateAndMapSprite(sourceGrid, pair.Value.Down)
                        );
                    }

                    return result;
                }
            }
        }
    }
}