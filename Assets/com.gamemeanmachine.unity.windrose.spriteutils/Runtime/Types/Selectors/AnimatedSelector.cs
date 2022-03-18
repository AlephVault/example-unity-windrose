using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
using AlephVault.Unity.Support.Utils;
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
            ///   A simple & animated selector involves a list of sprites.
            /// </summary>
            public class AnimatedSelector : MappedSpriteGridSelection<ReadOnlyCollection<Vector2Int>, Animation>
            {
                // The FPS to use for the selection.
                private uint fps;
                
                public AnimatedSelector(SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> selection, uint framesPerSecond) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                    fps = Values.Max(1u, framesPerSecond);
                }

                /// <summary>
                ///   The validation and mapping process involves a list of sprites
                ///   from a list of vector.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions to select</param>
                /// <returns>The mapped WindRose animation</returns>
                protected override Animation ValidateAndMap(SpriteGrid sourceGrid, ReadOnlyCollection<Vector2Int> selection)
                {
                    Sprite[] sprites = (from position in selection
                                        select ValidateAndMapSprite(sourceGrid, position)).ToArray();
                    Animation result = ScriptableObject.CreateInstance<Animation>();
                    Behaviours.SetObjectFieldValues(result, new Dictionary<string, object> {
                        { "sprites", sprites }, { "fps", fps }
                    });
                    return result;
                }

                ~AnimatedSelector()
                {
                    if (result != null) Object.Destroy(result);
                }
            }
        }
    }
}