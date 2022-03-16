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
            public class MultiRoseAnimatedSelector : MappedSpriteGridSelection<
                Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>>,
                Dictionary<Type, AnimationRose>
            >
            {
                // The FPS to use for the selection.
                private uint fps;

                public MultiRoseAnimatedSelector(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>> selection, uint framesPerSecond) : base(sourceGrid, selection)
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
                /// <returns>The mapped WindRose animation rose (mapped from type)</returns>
                protected override Dictionary<Type, AnimationRose> ValidateAndMap(SpriteGrid sourceGrid, Dictionary<Type, RoseTuple<ReadOnlyCollection<Vector2Int>>> selection)
                {
                    Dictionary<Type, AnimationRose> result = new Dictionary<Type, AnimationRose>();
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

                        AnimationRose animationRose = ScriptableObject.CreateInstance<AnimationRose>();
                        Behaviours.SetObjectFieldValues(animationRose, new Dictionary<string, object>() {
                            { "up", MakeAnimation(from position in pair.Value.Up select ValidateAndMapSprite(sourceGrid, position)) },
                            { "down", MakeAnimation(from position in pair.Value.Down select ValidateAndMapSprite(sourceGrid, position)) },
                            { "left", MakeAnimation(from position in pair.Value.Left select ValidateAndMapSprite(sourceGrid, position)) },
                            { "right", MakeAnimation(from position in pair.Value.Right select ValidateAndMapSprite(sourceGrid, position)) },
                        });
                        result[pair.Key] = animationRose;
                    }

                    return result;
                }

                // Creates an animation object.
                private Animation MakeAnimation(IEnumerable<Sprite> sprites)
                {
                    Animation result = ScriptableObject.CreateInstance<Animation>();
                    Behaviours.SetObjectFieldValues(result, new Dictionary<string, object> {
                        { "sprites", sprites }, { "fps", fps }
                    });
                    return result;
                }

                ~MultiRoseAnimatedSelector()
                {
                    if (result != null)
                    {
                        foreach (AnimationRose animation in result.Values)
                        {
                            Object.Destroy(animation.GetForDirection(Direction.UP));
                            Object.Destroy(animation.GetForDirection(Direction.DOWN));
                            Object.Destroy(animation.GetForDirection(Direction.LEFT));
                            Object.Destroy(animation.GetForDirection(Direction.RIGHT));
                            Object.Destroy(animation);
                        }
                    }
                }
            }
        }
    }
}