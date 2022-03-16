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
            public class SpritedSelectorApplier : SpriteGridSelectionApplier<Sprite>
            {
                protected override void AfterUse(SpriteGridSelection<Sprite> selection)
                {
                    // Apply the value.
                    throw new NotImplementedException();
                }

                protected override void AfterRelease(SpriteGridSelection<Sprite> selection)
                {
                    // Clear the value.
                    throw new NotImplementedException();
                }
            }
        }
    }
}