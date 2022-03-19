using System;
using System.Collections.Generic;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
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
            public class MultiRoseSpritedSelection : MappedSpriteGridSelection<MultiSettings<RoseTuple<Vector2Int>>, MultiSettings<SpriteRose>>
            {
                public MultiRoseSpritedSelection(SpriteGrid sourceGrid, MultiSettings<RoseTuple<Vector2Int>> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="RoseTuple{Vector2Int}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The rose tuples of positions to select (mapped from type)</param>
                /// <returns>The mapped WindRose sprite roses (mapped from type, and an idle state)</returns>
                protected override MultiSettings<SpriteRose> ValidateAndMap(SpriteGrid sourceGrid, MultiSettings<RoseTuple<Vector2Int>> selection)
                {
                    MultiSettings<SpriteRose> mapping = new MultiSettings<SpriteRose>();
                    foreach (KeyValuePair<string, RoseTuple<Vector2Int>> pair in selection)
                    {
                        if (pair.Value == null) throw new ArgumentException(
                            $"A null value was given to the sprite rose-tuple dictionary by key: {pair.Key}"
                        );

                        mapping[pair.Key] = ValidateAndMapSpriteRose(
                            sourceGrid, pair.Value.Up, pair.Value.Down, pair.Value.Left, pair.Value.Right
                        );
                    }

                    return mapping;
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

                ~MultiRoseSpritedSelection()
                {
                    if (result != null)
                    {
                        foreach (SpriteRose state in result.Values)
                        {
                            if (state) Object.Destroy(state);
                        }
                    }
                }
            }
        }
    }
}