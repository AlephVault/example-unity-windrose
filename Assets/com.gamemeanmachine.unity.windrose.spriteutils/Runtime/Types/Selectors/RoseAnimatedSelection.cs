using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
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
            ///   An oriented & animated selector involves a list of sprites per direction.
            /// </summary>
            public class RoseAnimatedSelection : MappedSpriteGridSelection<RoseTuple<ReadOnlyCollection<Vector2Int>>, AnimationRose>
            {
                // The FPS to use for the selection.
                private uint fps;

                public RoseAnimatedSelection(SpriteGrid sourceGrid, RoseTuple<ReadOnlyCollection<Vector2Int>> selection, uint framesPerSecond) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                    fps = Values.Max(1u, framesPerSecond);
                }

                /// <summary>
                ///   The validation and mapping process involves a list of sprites
                ///   from a list of vector for each direction.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions rose tuple to select</param>
                /// <returns>The mapped WindRose animation rose</returns>
                protected override AnimationRose ValidateAndMap(SpriteGrid sourceGrid, RoseTuple<ReadOnlyCollection<Vector2Int>> selection)
                {
                    if (selection.Up == null || selection.Left == null || selection.Right == null ||
                        selection.Down == null)
                    {
                        throw new ArgumentException(
                            $"A null value was given to the sprite list rose tuple in one of the fields"
                        );
                    }
                    
                    AnimationRose animationRose = ScriptableObject.CreateInstance<AnimationRose>();
                    Behaviours.SetObjectFieldValues(animationRose, new Dictionary<string, object>() {
                        { "up", MakeAnimation(from position in selection.Up select ValidateAndMapSprite(sourceGrid, position)) },
                        { "down", MakeAnimation(from position in selection.Down select ValidateAndMapSprite(sourceGrid, position)) },
                        { "left", MakeAnimation(from position in selection.Left select ValidateAndMapSprite(sourceGrid, position)) },
                        { "right", MakeAnimation(from position in selection.Right select ValidateAndMapSprite(sourceGrid, position)) },
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
                
                ~RoseAnimatedSelection()
                {
                    if (result != null)
                    {
                        Object.Destroy(result.GetForDirection(Direction.UP));
                        Object.Destroy(result.GetForDirection(Direction.DOWN));
                        Object.Destroy(result.GetForDirection(Direction.LEFT));
                        Object.Destroy(result.GetForDirection(Direction.RIGHT));
                        Object.Destroy(result);
                    }
                }
            }
        }
    }
}