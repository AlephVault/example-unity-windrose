using System.Collections;
using System.Collections.Generic;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals.StateBundles;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Samples
    {
        public class SpritedAltState : SpriteBundle
        {
            protected override string GetStateKey()
            {
                return "alt";
            }
        }
    }
}
