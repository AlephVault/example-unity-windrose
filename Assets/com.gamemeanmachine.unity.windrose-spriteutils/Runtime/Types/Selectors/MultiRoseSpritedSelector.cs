using System;
using System.Collections.Generic;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A multi-state oriented & sprited selector involves just one sprite per state and direction.
            /// </summary>
            public class MultiRoseSpritedSelector : MappedSpriteGridSelection<MultiSettings<RoseTuple<Vector2Int>>, MultiSettings<SpriteRose>>
            {
                public MultiRoseSpritedSelector(SpriteGrid sourceGrid, MultiSettings<RoseTuple<Vector2Int>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="RoseTuple{Vector2Int}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The rose tuples of positions to select (mapped from type)</param>
                /// <returns>The mapped WindRose sprite roses (mapped from type)</returns>
                protected override MultiSettings<SpriteRose> ValidateAndMap(SpriteGrid sourceGrid, MultiSettings<RoseTuple<Vector2Int>> selection)
                {
                    if (selection.Item1 == null) throw new ArgumentException(
                        $"A null value was given to the sprite rose-tuple dictionary in idle state"
                    );
                    SpriteRose idle = ValidateAndMapSpriteRose(
                        sourceGrid, selection.Item1.Up, selection.Item1.Down, selection.Item1.Left,
                        selection.Item1.Right
                    );
                    Dictionary<Type, Tuple<SpriteRose, string>> mapping = new Dictionary<Type, Tuple<SpriteRose, string>>();
                    foreach (KeyValuePair<Type, Tuple<RoseTuple<Vector2Int>, string>> pair in selection.Item2)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }

                        if (pair.Value == null) throw new ArgumentException(
                            $"A null value was given to the sprite rose-tuple dictionary by key: {pair.Key.FullName}"
                        );

                        mapping[pair.Key] = new Tuple<SpriteRose, string>(
                            pair.Value.Item1 != null ? ValidateAndMapSpriteRose(
                                sourceGrid, pair.Value.Item1.Up, pair.Value.Item1.Down, pair.Value.Item1.Left,
                                pair.Value.Item1.Right
                            ) : null,
                            pair.Value.Item2
                        );
                    }

                    return new MultiSettings<SpriteRose>(idle, mapping);
                }

                // Maps and creates a sprite rose from input
                private SpriteRose ValidateAndMapSpriteRose(
                    SpriteGrid sourceGrid, Vector2Int up, Vector2Int down, Vector2Int left, Vector2Int right
                )
                {
                    SpriteRose spriteRose = ScriptableObject.CreateInstance<SpriteRose>();
                    Behaviours.SetObjectFieldValues(spriteRose, new Dictionary<string, object>() {
                        { "up", ValidateAndMapSprite(sourceGrid, up) },
                        { "down", ValidateAndMapSprite(sourceGrid, down) },
                        { "left", ValidateAndMapSprite(sourceGrid, left) },
                        { "right", ValidateAndMapSprite(sourceGrid, right) }
                    });
                    return spriteRose;
                }

                ~MultiRoseSpritedSelector()
                {
                    if (result != null)
                    {
                        Object.Destroy(result.Item1);
                        foreach (Tuple<SpriteRose, string> state in result.Item2.Values)
                        {
                            if (state.Item1) Object.Destroy(state.Item1);
                        }
                    }
                }
            }
        }
    }
}