using System.Collections.ObjectModel;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;
using UnityEngine;
using Animation = GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources.Animation;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Animated selector appliers are added on top of <see cref="Animated"/>
            ///   visuals so they are able to replace the animation they use.
            /// </summary>
            [RequireComponent(typeof(Animated))]
            public class AnimatedSelectorApplier : SpriteGridSelectionApplier<Animation>
            {
                private Animated animated;
                
                private void Awake()
                {
                    animated = GetComponent<Animated>();
                }
                
                /// <summary>
                ///   Sets the animation directly into the animated behaviour.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected override void AfterUse(SpriteGridSelection<Animation> selection)
                {
                    animated.Animation = selection.GetSelection();
                }

                /// <summary>
                ///   Clears the animation from the animated behaviour.
                /// </summary>
                /// <param name="selection">The previous, just released, selection</param>
                protected override void AfterRelease(SpriteGridSelection<Animation> selection)
                {
                    animated.Animation = null;
                }
            }
        }
    }
}