using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GabTab.Behaviours;

namespace WindRose
{
    namespace Behaviours
    {
        namespace UI
        {
            using World;

            /// <summary>
            ///   <para>
            ///     Wraps all the map in the scene and also the <see cref="InteractiveInterface"/>
            ///       that is available inside. It is expected that only one be available.
            ///   </para>
            ///   <para>
            ///     The wrapped interactive interface will also have two listener to,
            ///       perhaps, pause/release the maps. This interface will be available for
            ///       any child that would need it (e.g. to start an interaction).
            ///   </para>
            /// </summary>
            public class InteractionProvider : MonoBehaviour
            {
                /// <summary>
                ///   Criteria to pause the map while the interaction is running: don't pause,
                ///     pause everything but animations, or completely freeze.
                /// </summary>
                public enum PauseType { NO, HOLD, FREEZE }

                /// <summary>
                ///   The <see cref="PauseType"/> to use while interacting.
                /// </summary>
                [SerializeField]
                private PauseType pauseType = PauseType.FREEZE;

                private InteractiveInterface interactionTab;

                /// <summary>
                ///   The interactive interface. It must be present among children.
                /// </summary>
                /// <remarks>
                ///   Perhaps this behaviour should be changed to require the user explicitly select
                ///     an interactive interface among the components.
                /// </remarks>
                public InteractiveInterface InteractionTab { get { return interactionTab;  } }

                // Use this for initialization
                private void Start()
                {
                    interactionTab = Support.Utils.Layout.RequireComponentInChildren<InteractiveInterface>(gameObject);
                    interactionTab.beforeRunningInteraction.AddListener(OnAcquire);
                    interactionTab.afterRunningInteraction.AddListener(OnRelease);
                }

                private void OnDestroy()
                {
                    interactionTab.beforeRunningInteraction.RemoveListener(OnAcquire);
                    interactionTab.afterRunningInteraction.RemoveListener(OnRelease);
                }

                private void OnAcquire()
                {
                    if (pauseType != PauseType.NO)
                    {
                        bool fullFreeze = pauseType == PauseType.FREEZE;
                        foreach (Map map in GetComponentsInChildren<Map>())
                        {
                            map.Pause(fullFreeze);
                        }
                    }
                }

                private void OnRelease()
                {
                    if (pauseType != PauseType.NO)
                    {
                        foreach (Map map in GetComponentsInChildren<Map>())
                        {
                            map.Resume();
                        }
                    }
                }
            }
        }
    }
}
