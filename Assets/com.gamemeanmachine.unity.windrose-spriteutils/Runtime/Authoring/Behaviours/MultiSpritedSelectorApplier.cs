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
            public class MultiSpritedSelectorApplier : SpriteGridSelectionApplier<Dictionary<Type, Sprite>>
            {
                protected override bool IsCompatible(SpriteGridSelection<Dictionary<Type, Sprite>> selection)
                {
                    // Test compatibility (having those state behaviours).
                    return base.IsCompatible(selection);
                }

                protected override void BeforeUse(SpriteGridSelection<Dictionary<Type, Sprite>> selection)
                {
                    // Ensure compatibility, or fail (having those state behaviours).
                    base.BeforeUse(selection);
                }

                protected override void AfterUse(SpriteGridSelection<Dictionary<Type, Sprite>> selection)
                {
                    // Apply the value.
                    throw new NotImplementedException();
                }

                protected override void AfterRelease(SpriteGridSelection<Dictionary<Type, Sprite>> selection)
                {
                    // Clear the value.
                    throw new NotImplementedException();
                }
            }
        }
    }
}