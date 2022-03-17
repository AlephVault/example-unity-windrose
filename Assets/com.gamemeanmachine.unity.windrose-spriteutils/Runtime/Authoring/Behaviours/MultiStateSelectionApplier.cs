using System;
using System.Collections.Generic;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using AlephVault.Unity.SpriteUtils.Types;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using GameMeanMachine.Unity.WindRose.Authoring.ScriptableObjects.VisualResources;
using GameMeanMachine.Unity.WindRose.SpriteUtils.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.SpriteUtils
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Multi-State selector appliers are added on top of a <see cref="MultiState{T}"/>
            ///   visual so they are able to replace the multiple states they use.
            /// </summary>
            public abstract class MultiStateSelectionApplier<StateType, MultiStateType> : SpriteGridSelectionApplier<MultiSettings<StateType>>
                where MultiStateType : MultiState<StateType>
            {
                private MultiStateType multiState;

                private void Awake()
                {
                    multiState = GetComponent<MultiStateType>();
                }

                /// <summary>
                ///   Tests whether the required states are present or not.
                /// </summary>
                /// <param name="selection">The selection to test</param>
                /// <returns>Whether all the states in the selection exist in the multi-state</returns>
                protected override bool IsCompatible(SpriteGridSelection<MultiSettings<StateType>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiState.HasState(key)) return false;
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
                protected override void BeforeUse(SpriteGridSelection<MultiSettings<StateType>> selection)
                {
                    foreach (string key in selection.GetSelection().Item2.Keys)
                    {
                        if (!multiState.HasState(key)) throw new IncompatibleSelectionException(
                            $"The given selection requires the state with name '{key}' to " +
                            $"be present in the current visual object"
                        );
                    }
                }

                /// <summary>
                ///   Replaces all the states with the idle one and the added ones.
                /// </summary>
                /// <param name="selection">The selection to apply</param>
                protected override void AfterUse(SpriteGridSelection<MultiSettings<StateType>> selection)
                {
                    multiState.ReplaceState(MultiRoseSprited.IDLE, selection.GetSelection().Item1);
                    foreach (KeyValuePair<string, Tuple<StateType, string>> item in selection.GetSelection().Item2)
                    {
                        multiState.ReplaceState(item.Key, item.Value.Item1);
                    }
                }

                /// <summary>
                ///   Clears all the states as specified in the selection.
                /// </summary>
                /// <param name="selection">The selection being removed</param>
                protected override void AfterRelease(SpriteGridSelection<MultiSettings<StateType>> selection)
                {
                    multiState.ReplaceState(MultiRoseSprited.IDLE, default);
                    foreach (KeyValuePair<string, Tuple<StateType, string>> item in selection.GetSelection().Item2)
                    {
                        multiState.ReplaceState(item.Key, item.Value.Item1);
                    }
                }
            }
        }
    }
}