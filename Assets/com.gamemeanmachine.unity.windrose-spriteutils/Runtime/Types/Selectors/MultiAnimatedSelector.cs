using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            ///   A multi-state & animated selector involves a list of sprites per state.
            /// </summary>
            public class MultiAnimatedSelector
                : MappedSpriteGridSelection<Dictionary<Type, ReadOnlyCollection<Vector2Int>>, Dictionary<Type, ReadOnlyCollection<Sprite>>>
            {
                public MultiAnimatedSelector(SpriteGrid sourceGrid, Dictionary<Type, ReadOnlyCollection<Vector2Int>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="ReadOnlyCollection{Vector2Int}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions lists to select (mapped from type)</param>
                /// <returns>The sprites lists (mapped from type)</returns>
                protected override Dictionary<Type, ReadOnlyCollection<Sprite>> ValidateAndMap(SpriteGrid sourceGrid, Dictionary<Type, ReadOnlyCollection<Vector2Int>> selection)
                {
                    Dictionary<Type, ReadOnlyCollection<Sprite>> result = new Dictionary<Type, ReadOnlyCollection<Sprite>>();
                    foreach (KeyValuePair<Type, ReadOnlyCollection<Vector2Int>> pair in selection)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }
                        if (pair.Value == null) throw new ArgumentException(
                            $"A null value was given to the sprite list dictionary by key: {pair.Key.FullName}"
                        );
                        result[pair.Key] = new ReadOnlyCollection<Sprite>(
                            (IList<Sprite>) from position in pair.Value select ValidateAndMapSprite(sourceGrid, position)
                        );
                    }

                    return result;
                }
            }
        }
    }
}