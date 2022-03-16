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
            ///   Rose-Sprited selector appliers are added on top of <see cref="RoseSprited"/>
            ///   visuals so they are able to replace the animation rose they use.
            /// </summary>
            [RequireComponent(typeof(RoseSprited))]
            public class RoseSpritedSelectorApplier : SpriteGridSelectionApplier<SpriteRose>
            {
                private RoseSprited roseSprited;

                private void Awake()
                {
                    roseSprited = GetComponent<RoseSprited>();
                }

                /// <summary>
                ///   Sets the sprite rose directly into the RoseSprited behaviour.
                /// </summary>
                /// <param name="selection">The new selection</param>
                protected override void AfterUse(SpriteGridSelection<SpriteRose> selection)
                {
                    roseSprited.SpriteRose = selection.GetSelection();
                }

                /// <summary>
                ///   Clears the sprite rose from the RoseSprited behaviour.
                /// </summary>
                /// <param name="selection">The previous, just released, selection</param>
                protected override void AfterRelease(SpriteGridSelection<SpriteRose> selection)
                {
                    roseSprited.SpriteRose = null;
                }
            }
        }
    }
}