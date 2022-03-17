using System;
using System.Collections.Generic;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Multi-State Sprited selector appliers are added on top of <see cref="MultiSprited"/>
            ///   visuals so they are able to replace the multiple sprites they use.
            /// </summary>
            [RequireComponent(typeof(MultiSprited))]
            public class MultiSpritedSelectorApplier : MultiStateSelectorApplier<Sprite, MultiSprited> {}
        }
    }
}