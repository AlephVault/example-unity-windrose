using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Rose-Animated selector appliers are added on top of <see cref="RoseAnimated"/>
            ///   visuals so they are able to replace the animation rose they use.
            /// </summary>
            [RequireComponent(typeof(RoseAnimated))]
            public class RoseAnimatedSelectorApplier : SpriteGridSelectionApplier<AnimationRose>
            {
                private RoseAnimated roseAnimated;

                private void Awake()
                {
                    roseAnimated = GetComponent<RoseAnimated>();
                }

                /// <summary>
                ///   Sets the animation rose directly into the RoseAnimated behaviour.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected override void AfterUse(SpriteGridSelection<AnimationRose> selection)
                {
                    roseAnimated.AnimationRose = selection.GetSelection();
                }

                /// <summary>
                ///   Clears the animation rose from the RoseAnimated behaviour.
                /// </summary>
                /// <param name="selection">The previous, just released, selection</param>
                protected override void AfterRelease(SpriteGridSelection<AnimationRose> selection)
                {
                    roseAnimated.AnimationRose = null;
                }
            }
        }
    }
}