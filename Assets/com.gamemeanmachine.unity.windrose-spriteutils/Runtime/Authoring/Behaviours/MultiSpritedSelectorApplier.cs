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
            public class MultiSpritedSelectorApplier : SpriteGridSelectionApplier<MultiSettings<Sprite>>
            {
                private MultiSprited multiSprited;

                private void Awake()
                {
                    multiSprited = GetComponent<MultiSprited>();
                }

                /// <summary>
                ///   Tests whether the required states are present or not.
                /// </summary>
                /// <param name="selection">The selection to test</param>
                /// <returns>Whether all the states in the selection exist in the multi-state</returns>
                protected override bool IsCompatible(SpriteGridSelection<MultiSettings<Sprite>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiSprited.HasState(key)) return false;
                    }

                    return true;
                }

                /// <summary>
                ///   Checks that the required states are present, or fails.
                /// </summary>
                /// <param name="selection">The selection to apply</param>
                /// <exception cref="IncompatibleSelectionException">
                ///   At least a state in the selection does not exist in the multi-state
                /// </exception>
                protected override void BeforeUse(SpriteGridSelection<MultiSettings<Sprite>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiSprited.HasState(key)) throw new IncompatibleSelectionException(
                            $"The given selection requires the state with name '{key}' to " +
                            $"be present in the current visual object"
                        );
                    }
                }

                /// <summary>
                ///   Replaces all the states with the idle one and the added ones.
                /// </summary>
                /// <param name="selection">The selection to apply</param>
                protected override void AfterUse(SpriteGridSelection<MultiSettings<Sprite>> selection)
                {
                    multiSprited.ReplaceState(MultiSprited.IDLE, selection.GetSelection().Item1);
                    foreach (KeyValuePair<string, Tuple<Sprite, string>> item in selection.GetSelection().Item2)
                    {
                        multiSprited.ReplaceState(item.Key, item.Value.Item1);
                    }
                }

                /// <summary>
                ///   Clears all the states as specified in the selection.
                /// </summary>
                /// <param name="selection">The selection being removed</param>
                protected override void AfterRelease(SpriteGridSelection<MultiSettings<Sprite>> selection)
                {
                    multiSprited.ReplaceState(MultiSprited.IDLE, null);
                    foreach (KeyValuePair<string, Tuple<Sprite, string>> item in selection.GetSelection().Item2)
                    {
                        multiSprited.ReplaceState(item.Key, item.Value.Item1);
                    }
                }
            }
        }
    }
}