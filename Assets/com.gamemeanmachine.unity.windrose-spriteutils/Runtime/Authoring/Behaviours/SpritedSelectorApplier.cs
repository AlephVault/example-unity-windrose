using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Sprited selector appliers are added on top of <see cref="SpriteRenderer"/>
            ///   visuals so they are able to replace the animation they use.
            /// </summary>
            [RequireComponent(typeof(SpriteRenderer))]
            public class SpritedSelectorApplier : SpriteGridSelectionApplier<Sprite>
            {
                private SpriteRenderer renderer;

                private void Awake()
                {
                    renderer = GetComponent<SpriteRenderer>();
                }

                /// <summary>
                ///   Sets the sprite directly into the SpriteRenderer behaviour.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected override void AfterUse(SpriteGridSelection<Sprite> selection)
                {
                    renderer.sprite = selection.GetSelection();
                }

                /// <summary>
                ///   Clears the sprite from the SpriteRenderer behaviour.
                /// </summary>
                /// <param name="selection">The previous, just released, selection</param>
                protected override void AfterRelease(SpriteGridSelection<Sprite> selection)
                {
                    renderer.sprite = null;
                }
            }
        }
    }
}