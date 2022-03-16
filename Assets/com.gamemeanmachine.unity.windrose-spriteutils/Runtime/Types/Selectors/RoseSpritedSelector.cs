using System;
using System.Collections.Generic;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.SpriteUtils.Types;
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
            ///   An oriented & sprited selector involves just one sprite per direction.
            /// </summary>
            public class RoseSpritedSelector : MappedSpriteGridSelection<RoseTuple<Vector2Int>, SpriteRose>
            {
                public RoseSpritedSelector(SpriteGrid sourceGrid, RoseTuple<Vector2Int> selection) : base(sourceGrid, selection)
                {
                    if (selection == null) throw new ArgumentNullException(nameof(selection));
                }

                /// <summary>
                ///   The validation and mapping involves a single sprite from a single
                ///   vector for each direction.
                /// </summary>
                /// <param name="sourceGrid">The grid to validate against</param>
                /// <param name="selection">The positions rose tuple to select</param>
                /// <returns>The mapped WindRose sprites rose</returns>
                protected override SpriteRose ValidateAndMap(SpriteGrid sourceGrid, RoseTuple<Vector2Int> selection)
                {
                    SpriteRose spriteRose = ScriptableObject.CreateInstance<SpriteRose>();
                    Behaviours.SetObjectFieldValues(spriteRose, new Dictionary<string, object>() {
                        { "up", ValidateAndMapSprite(sourceGrid, selection.Up) },
                        { "down", ValidateAndMapSprite(sourceGrid, selection.Down) },
                        { "left", ValidateAndMapSprite(sourceGrid, selection.Left) },
                        { "right", ValidateAndMapSprite(sourceGrid, selection.Right) }
                    });
                    return spriteRose;
                }

                ~RoseSpritedSelector()
                {
                    if (result != null)
                    {
                        Object.Destroy(result);
                    }
                }
            }
        }
    }
}
