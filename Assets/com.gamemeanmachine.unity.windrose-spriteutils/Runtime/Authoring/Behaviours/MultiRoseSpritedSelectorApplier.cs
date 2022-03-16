using System;
using System.Collections.Generic;
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
            public class MultiRoseSpritedSelectorApplier : SpriteGridSelectionApplier<Dictionary<Type, RoseTuple<Sprite>>>
            {
                protected override bool IsCompatible(SpriteGridSelection<Dictionary<Type, RoseTuple<Sprite>>> selection)
                {
                    // Test compatibility (having those state behaviours).
                    return base.IsCompatible(selection);
                }

                protected override void BeforeUse(SpriteGridSelection<Dictionary<Type, RoseTuple<Sprite>>> selection)
                {
                    // Ensure compatibility, or fail (having those state behaviours).
                    base.BeforeUse(selection);
                }

                protected override void AfterUse(SpriteGridSelection<Dictionary<Type, RoseTuple<Sprite>>> selection)
                {
                    // Apply the value.
                    throw new NotImplementedException();
                }

                protected override void AfterRelease(SpriteGridSelection<Dictionary<Type, RoseTuple<Sprite>>> selection)
                {
                    // Clear the value.
                    throw new NotImplementedException();
                }
            }
        }
    }
}