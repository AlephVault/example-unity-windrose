using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;
using Animation = GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources.Animation;
using Object = UnityEngine.Object;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Types
    {
        namespace Selectors
        {
            /// <summary>
            ///   A multi-state oriented & animated selector involves a list of sprites per direction & state.
            /// </summary>
            public class MultiRoseAnimatedSelection : MappedSpriteGridSelection<
                MultiSettings<RoseTuple<ReadOnlyCollection<Vector2Int>>>,
                MultiSettings<AnimationRose>
            >
            {
                // The FPS to use for the selection.
                private uint fps;

                public MultiRoseAnimatedSelection(SpriteGrid sourceGrid, MultiSettings<RoseTuple<ReadOnlyCollection<Vector2Int>>> selection, uint framesPerSecond) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                    fps = Values.Max(1u, framesPerSecond);
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="RoseTuple{ReadOnlyCollection{Vector2Int}}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions lists to select, for each direction (mapped from type)</param>
                /// <returns>The mapped WindRose animation rose (mapped from type, and idle state)</returns>
                protected override MultiSettings<AnimationRose> ValidateAndMap(SpriteGrid sourceGrid, MultiSettings<RoseTuple<ReadOnlyCollection<Vector2Int>>> selection)
                {
                    MultiSettings<AnimationRose> mapping = new MultiSettings<AnimationRose>();
                    foreach (KeyValuePair<string, RoseTuple<ReadOnlyCollection<Vector2Int>>> pair in selection)
                    {
                        if (pair.Value == null || pair.Value.Up == null || pair.Value.Left == null ||
                            pair.Value.Right == null || pair.Value.Down == null)
                        {
                            throw new ArgumentException(
                                $"A null value was given to the sprite list rose tuple dictionary by key: " +
                                $"{pair.Key} or, given the rose, any of its component"
                            );
                        }

                        mapping[pair.Key] = ValidateAndMapAnimationRose(
                            sourceGrid, pair.Value.Up, pair.Value.Down, pair.Value.Left,
                            pair.Value.Right
                        );
                    }

                    return mapping;
                }

                // Maps and creates an animation rose from input.
                private AnimationRose ValidateAndMapAnimationRose(
                    SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> up,
                    ReadOnlyCollection<Vector2Int> down, ReadOnlyCollection<Vector2Int> left,
                    ReadOnlyCollection<Vector2Int> right
                )
                {
                    AnimationRose animationRose = ScriptableObject.CreateInstance<AnimationRose>();
                    Behaviours.SetObjectFieldValues(animationRose, new Dictionary<string, object>() {
                        { "up", MakeAnimation(from position in up select ValidateAndMapSprite(sourceGrid, position)) },
                        { "down", MakeAnimation(from position in down select ValidateAndMapSprite(sourceGrid, position)) },
                        { "left", MakeAnimation(from position in left select ValidateAndMapSprite(sourceGrid, position)) },
                        { "right", MakeAnimation(from position in right select ValidateAndMapSprite(sourceGrid, position)) },
                    });
                    return animationRose;
                }

                // Creates an animation object.
                private Animation MakeAnimation(IEnumerable<Sprite> sprites)
                {
                    Animation result = ScriptableObject.CreateInstance<Animation>();
                    Behaviours.SetObjectFieldValues(result, new Dictionary<string, object> {
                        { "sprites", sprites.ToArray() }, { "fps", fps }
                    });
                    return result;
                }

                ~MultiRoseAnimatedSelection()
                {
                    if (result != null)
                    {
                        foreach (AnimationRose state in result.Values)
                        {
                            if (state)
                            {
                                Object.Destroy(state.GetForDirection(Direction.UP));
                                Object.Destroy(state.GetForDirection(Direction.DOWN));
                                Object.Destroy(state.GetForDirection(Direction.LEFT));
                                Object.Destroy(state.GetForDirection(Direction.RIGHT));
                                Object.Destroy(state);
                            }
                        }
                    }
                }
            }
        }
    }
}