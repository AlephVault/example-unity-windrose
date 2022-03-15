using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            ///   A multi-state oriented & animated selector involves a list of sprites per direction & state.
            /// </summary>
            public class MultiRoseAnimatedSelector : MappedSpriteGridSelection<
                Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>>,
                Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>
            >
            {
                public MultiRoseAnimatedSelector(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="RoseTuple{ReadOnlyCollection{Vector2Int}}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions lists to select, for each direction (mapped from type)</param>
                /// <returns>The sprites lists, for each direction (mapped from type)</returns>
                protected override Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>> ValidateAndMap(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>> selection)
                {
                    Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>> result = new Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>();
                    foreach (KeyValuePair<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>> pair in selection)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }

                        if (pair.Value == null || pair.Value.Up == null || pair.Value.Left == null ||
                            pair.Value.Right == null || pair.Value.Down == null)
                        {
                            throw new ArgumentException(
                                $"A null value was given to the sprite list rose tuple dictionary by key: " +
                                $"{pair.Key.FullName}, or any of its fields is null"
                            );
                        }

                        result[pair.Key] = new RoseTuple<ReadOnlyCollection<Sprite>>(
                            new ReadOnlyCollection<Sprite>(
                                (IList<Sprite>) from position in pair.Value.Up select ValidateAndMapSprite(sourceGrid, position)
                            ),
                            new ReadOnlyCollection<Sprite>(
                                (IList<Sprite>) from position in pair.Value.Left select ValidateAndMapSprite(sourceGrid, position)
                            ),
                            new ReadOnlyCollection<Sprite>(
                                (IList<Sprite>) from position in pair.Value.Right select ValidateAndMapSprite(sourceGrid, position)
                            ),
                            new ReadOnlyCollection<Sprite>(
                                (IList<Sprite>) from position in pair.Value.Down select ValidateAndMapSprite(sourceGrid, position)
                            )
                        );
                    }

                    return result;
                }
            }
        }
    }
}