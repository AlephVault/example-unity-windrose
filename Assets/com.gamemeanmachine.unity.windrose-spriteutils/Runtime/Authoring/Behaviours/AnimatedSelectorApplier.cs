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
            public class AnimatedSelectorApplier : SpriteGridSelectionApplier<ReadOnlyCollection<Sprite>>
            {
                private SpriteRenderer renderer;
                private Animated animated;
                
                private void Awake()
                {
                    renderer = GetComponent<SpriteRenderer>();
                    animated = GetComponent<Animated>();
                }

                protected override bool IsCompatible(SpriteGridSelection<ReadOnlyCollection<Sprite>> selection)
                {
                    return selection.GetSelection().Count > 0;
                }

                protected override void BeforeUse(SpriteGridSelection<ReadOnlyCollection<Sprite>> selection)
                {
                    if (selection.GetSelection().Count == 0)
                    {
                        throw new IncompatibleSelectionException("Cannot use an empty animation");
                    }
                }

                protected override void AfterUse(SpriteGridSelection<ReadOnlyCollection<Sprite>> selection)
                {
                    // animated.Animation.Sprites = selection.GetSelection();
                    // show the sprite renderer.
                    if (animated.Animation)
                    {
                        Animation animation = animated.Animation;
                        
                        animated.Animation = null;
                    }
                }

                protected override void AfterRelease(SpriteGridSelection<ReadOnlyCollection<Sprite>> selection)
                {
                    // animation.Animation.Sprites = nothing.
                    // hide the sprite renderer.
                }
            }
        }
    }
}