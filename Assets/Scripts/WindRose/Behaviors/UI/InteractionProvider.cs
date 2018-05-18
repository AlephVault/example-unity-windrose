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
            public class InteractionProvider : MonoBehaviour
            {
                /**
                 * Wraps all the maps in the scene and also the InteractiveInterface that is
                 *   available inside. It is expected that only one be available.
                 * 
                 * The wrapped interactive interface will also have two listeners to, perhaps,
                 *   pause/release the maps. This interface will be available for any child
                 *   that would need it (e.g. to start an interaction).
                 */
                public enum PauseType { NO, HOLD, FREEZE }

                [SerializeField]
                private PauseType pauseType = PauseType.FREEZE;

                private InteractiveInterface interactionTab;

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
