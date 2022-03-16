using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
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
            ///   A multi-state & animated selector involves a list of sprites per state.
            /// </summary>
            public class MultiAnimatedSelector
                : MappedSpriteGridSelection<MultiSettings<ReadOnlyCollection<Vector2Int>>, MultiSettings<Animation>>
            {
                // The FPS to use for the selection.
                private uint fps;
                
                public MultiAnimatedSelector(SpriteGrid sourceGrid, MultiSettings<ReadOnlyCollection<Vector2Int>> selection, uint framesPerSecond) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                    fps = Values.Max(1u, framesPerSecond);
                }

                /// <summary>
                ///   Validates and maps a dictionary of <see cref="Type"/> => <see cref="ReadOnlyCollection{Vector2Int}"/>.
                ///   Each value must be valid and each type must be a subclass of <see cref="SpriteBundle"/>.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions lists to select (mapped from type)</param>
                /// <returns>The mapped WindRose animation (mapped from type, and idle state)</returns>
                protected override MultiSettings<Animation> ValidateAndMap(SpriteGrid sourceGrid, MultiSettings<ReadOnlyCollection<Vector2Int>> selection)
                {
                    if (selection.Item1 == null) throw new ArgumentException(
                        "A null value was given to the sprite list in idle state"
                    );
                    Animation idle = ValidateAndMapAnimation(sourceGrid, selection.Item1);
                    Dictionary<Type, Tuple<Animation, string>> mapping = new Dictionary<Type, Tuple<Animation, string>>();
                    foreach (KeyValuePair<Type, Tuple<ReadOnlyCollection<Vector2Int>, string>> pair in selection.Item2)
                    {
                        if (!Classes.IsSameOrSubclassOf(pair.Key, typeof(SpriteBundle)))
                        {
                            throw new ArgumentException(
                                $"Cannot use type {pair.Key} as key, since it is not a subclass of " +
                                $"{typeof(SpriteBundle).FullName}"
                            );
                        }
                        if (pair.Value == null) throw new ArgumentException(
                            $"A null value was given to the sprite list by key: {pair.Key.FullName}"
                        );

                        mapping[pair.Key] = new Tuple<Animation, string>(
                            pair.Value.Item1 != null ? ValidateAndMapAnimation(sourceGrid, pair.Value.Item1) : null,
                            pair.Value.Item2
                        );
                    }

                    return new MultiSettings<Animation>(idle, mapping);
                }

                // Maps an entire animation from the input positions and the sprite grid.
                private Animation ValidateAndMapAnimation(SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> value)
                {
                    Sprite[] sprites = (from position in value
                                        select ValidateAndMapSprite(sourceGrid, position)).ToArray();
                    Animation animation = ScriptableObject.CreateInstance<Animation>();
                    Behaviours.SetObjectFieldValues(animation, new Dictionary<string, object> {
                        { "sprites", sprites }, { "fps", fps }
                    });
                    return animation;
                }

                ~MultiAnimatedSelector()
                {
                    if (result != null)
                    {
                        Object.Destroy(result.Item1);
                        foreach (Tuple<Animation, string> state in result.Item2.Values)
                        {
                            if (state.Item1) Object.Destroy(state.Item1);
                        }
                    }
                }
            }
        }
    }
}