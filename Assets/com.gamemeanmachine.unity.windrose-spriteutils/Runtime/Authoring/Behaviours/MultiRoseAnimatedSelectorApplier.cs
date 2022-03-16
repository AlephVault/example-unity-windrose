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
            public class MultiRoseAnimatedSelectorApplier : SpriteGridSelectionApplier<Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>>
            {
                protected override bool IsCompatible(SpriteGridSelection<Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>> selection)
                {
                    // Test compatibility (having those state behaviours, no empty animation).
                    return base.IsCompatible(selection);
                }

                protected override void BeforeUse(SpriteGridSelection<Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>> selection)
                {
                    // Ensure compatibility, or fail (having those state behaviours, no empty animation).
                    base.BeforeUse(selection);
                }

                protected override void AfterUse(SpriteGridSelection<Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>> selection)
                {
                    // Apply the value.
                    throw new NotImplementedException();
                }

                protected override void AfterRelease(SpriteGridSelection<Dictionary<Type, RoseTuple<ReadOnlyCollection<Sprite>>>> selection)
                {
                    // Clear the value.
                    throw new NotImplementedException();
                }
            }
        }
    }
}