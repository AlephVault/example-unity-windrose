using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using UnityEngine;
using Animation = GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources.Animation;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Multi-State Animated selector appliers are added on top of <see cref="MultiAnimated"/>
            ///   visuals so they are able to replace the multiple animations they use.
            /// </summary>
            [RequireComponent(typeof(MultiAnimated))]
            public class MultiAnimatedSelectionApplier : MultiStateSelectionApplier<Animation, MultiAnimated> {}
        }
    }
}