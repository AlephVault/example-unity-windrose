using System;
using System.Collections.Generic;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Multi-State Rose-Sprited selector appliers are added on top of <see cref="MultiRoseSprited"/>
            ///   visuals so they are able to replace the multiple sprite roses they use.
            /// </summary>
            public class MultiRoseSpritedSelectorApplier : SpriteGridSelectionApplier<MultiSettings<SpriteRose>>
            {
                private MultiRoseSprited multiRoseSprited;

                private void Awake()
                {
                    multiRoseSprited = GetComponent<MultiRoseSprited>();
                }

                /// <summary>
                ///   Tests whether the required states are present or not.
                /// </summary>
                /// <param name="selection">The selection to test</param>
                /// <returns>Whether all the states in the selection exist in the multi-state</returns>
                protected override bool IsCompatible(SpriteGridSelection<MultiSettings<SpriteRose>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiRoseSprited.HasState(key)) return false;
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
                protected override void BeforeUse(SpriteGridSelection<MultiSettings<SpriteRose>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiRoseSprited.HasState(key)) throw new IncompatibleSelectionException(
                            $"The given selection requires the state with name '{key}' to " +
                            $"be present in the current visual object"
                        );
                    }
                }

                /// <summary>
                ///   Replaces all the states with the idle one and the added ones.
                /// </summary>
                /// <param name="selection">The selection to apply</param>
                protected override void AfterUse(SpriteGridSelection<MultiSettings<SpriteRose>> selection)
                {
                    multiRoseSprited.ReplaceState(MultiRoseSprited.IDLE, selection.GetSelection().Item1);
                    foreach (KeyValuePair<string, Tuple<SpriteRose, string>> item in selection.GetSelection().Item2)
                    {
                        multiRoseSprited.ReplaceState(item.Key, item.Value.Item1);
                    }
                }

                /// <summary>
                ///   Clears all the states as specified in the selection.
                /// </summary>
                /// <param name="selection">The selection being removed</param>
                protected override void AfterRelease(SpriteGridSelection<MultiSettings<SpriteRose>> selection)
                {
                    multiRoseSprited.ReplaceState(MultiRoseSprited.IDLE, null);
                    foreach (KeyValuePair<string, Tuple<SpriteRose, string>> item in selection.GetSelection().Item2)
                    {
                        multiRoseSprited.ReplaceState(item.Key, item.Value.Item1);
                    }
                }
            }
        }
    }
}