using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GabTab.Behaviours;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            using Entities.Objects;

            /// <summary>
            ///   <para>
            ///     Wraps all the map in the scene and links to a <see cref="InteractiveInterface"/>
            ///       that is available somewhere.
            ///   </para>
            ///   <para>
            ///     The wrapped interactive interface will also have two listeners to,
            ///       perhaps, pause/release the maps. This interface will be available for
            ///       any child that would need it (e.g. to start an interaction).
            ///   </para>
            ///   <para>
            ///     This behaviour will also may make use od a camera.
            ///   </para>
            /// </summary>
            public class PlaySpace : MonoBehaviour
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

                /// <summary>
                ///   The related <see cref="InteractiveInterface"/> to provide/trigger.
                ///   It may not need to be right inside this object's hierarchy.
                /// </summary>
                [SerializeField]
                private InteractiveInterface interactionTab;

                /// <summary>
                ///   The interactive interface. It must be present among children.
                /// </summary>
                /// <remarks>
                ///   Perhaps this behaviour should be changed to require the user explicitly select
                ///     an interactive interface among the components.
                /// </remarks>
                public InteractiveInterface InteractionTab { get { return interactionTab; } }

                // All the instances and the cameras they are bound to.
                private static Dictionary<Camera, PlaySpace> camerasMapping = new Dictionary<Camera, PlaySpace>();

                /// <summary>
                ///   The camera this PlaySpace is tied to.
                /// </summary>
                [SerializeField]
                public Camera camera;

                /// <summary>
                ///   Gets or sets the current camera. On set, if camera is in use by another component, it will fail.
                /// </summary>
                public Camera Camera
                {
                    get
                    {
                        return camera;
                    }
                    set
                    {
                        // If there new camera is in use by another playspace, it is an error.
                        // Otherwise, lets process the property change.
                        PlaySpace otherPS;
                        bool newCameraInUse = camerasMapping.TryGetValue(value, out otherPS);
                        if (newCameraInUse)
                        {
                            if (otherPS != null) throw new Types.Exception("The camera being assigned to this playspace is already being used by another playspace");
                        }
                        else
                        {
                            if (camera != null)
                            {
                                camerasMapping.Remove(camera);
                            }
                            if (value != null)
                            {
                                camerasMapping.Add(value, this);
                                Canvas interactorCanvas = interactionTab.GetComponentInParent<Canvas>();
                                interactorCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                                interactorCanvas.worldCamera = value;
                            }
                            camera = value;
                        }
                    }
                }

                /// <summary>
                ///   The object being followed.
                /// </summary>
                public MapObject Target { get; private set; }

                // The remaining time of the transition.
                private float remainingTransitioningTime = 0f;

                /// <summary>
                ///   Current focus status (using the specified camera):
                ///   <list type="bullet">
                ///     <item>
                ///       <term>NotFocusing</term>
                ///       <description>There is no current target being followed.</description>
                ///     </item>
                ///     <item>
                ///       <term>Transitioning</term>
                ///       <description>The camera is moving towards the target object.</description>
                ///     </item>
                ///     <item>
                ///       <term>Focusing</term>
                ///       <description>The object is following the target object.</description>
                ///     </item>
                ///   </list>
                /// </summary>
                public enum FocusStatus { NotFocusing, Transitioning, Focusing }

                /// <summary>
                ///   The current focus status.
                /// </summary>
                /// <seealso cref="FocusStatus"/>
                public FocusStatus Status { get; private set; }

                private void Awake()
                {
                    // Redundant init of camera.
                    try
                    {
                        Camera = camera;
                    }
                    catch
                    {
                        Destroy(gameObject);
                        throw;
                    }
                }

                // Use this for initialization
                private void Start()
                {
                    interactionTab.beforeRunningInteraction.AddListener(OnAcquire);
                    interactionTab.afterRunningInteraction.AddListener(OnRelease);
                }

                private void OnDestroy()
                {
                    if (camera) camerasMapping.Remove(camera);
                    interactionTab.beforeRunningInteraction.RemoveListener(OnAcquire);
                    interactionTab.afterRunningInteraction.RemoveListener(OnRelease);
                }

                /// <summary>
                ///   <para>
                ///     Chooses a new object to follow, specifying an optional delay and, in
                ///       that case, the possibility to avoid waiting any current transition.
                ///   </para>
                ///   <para>
                ///     If no new <paramref name="newTarget"/> is specified, <paramref name="delay"/>
                ///       will not be considered. If <paramref name="noWait"/> is true, the current
                ///       transition will be aborted instantly and the target will be set to null.
                ///       If it is false, the target will be set to null after waiting the current
                ///       transition.
                ///   </para>
                ///   <para>
                ///     If a new <paramref name="newTarget"/> is specified, <paramref name="delay"/>
                ///       will be considered to start a transition (if > 0). If <paramref name="noWait"/>
                ///       is true, and a transition is being run, no coroutine will start. Otherwise,
                ///       it will start a waiting & transitioning coroutine as normal.
                ///   </para>
                /// </summary>
                /// <param name="newTarget">The new object to follow</param>
                /// <param name="delay">The delay to take transitioning to the new object</param>
                /// <param name="noWait">Tells whether waiting the current transition or not</param>
                /// <returns>The new coroutine</returns>
                public Coroutine Focus(MapObject newTarget, float delay = 0f, bool noWait = false)
                {
                    if (!camera)
                    {
                        // An empty coroutine.
                        return StartCoroutine(new MapObject[] { }.GetEnumerator());
                    }
                    else
                    {
                        return StartCoroutine(DoFocus(newTarget, delay, noWait));
                    }
                }

                private IEnumerator DoFocus(MapObject newTarget, float delay = 0f, bool noWait = false)
                {
                    if (noWait)
                    {
                        if (newTarget && Status == FocusStatus.Transitioning)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        // Wait until the current object is being focused.
                        yield return new WaitUntil(delegate () { return Status != FocusStatus.Transitioning; });
                    }

                    // Set the object and move to its position or start a new transition to it.
                    Target = newTarget;
                    if (Target == null)
                    {
                        Status = FocusStatus.NotFocusing;
                    }
                    else if (delay >= 0)
                    {
                        Status = FocusStatus.Transitioning;
                        remainingTransitioningTime = delay;
                    }
                    else
                    {
                        Status = FocusStatus.Focusing;
                    }
                }

                private void Update()
                {
                    /**
                     * Focuses the camera on the target object position, or transitions by considering the deltaTime/remaining
                     *   fraction and the object's distance to the camera. When the transition ends, the <see cref="Status"/>
                     *   will be changed to <see cref="FocusStatus.Focusing"/>.
                     */
                    if (Target && camera)
                    {
                        Vector3 targetPosition = new Vector3(Target.transform.position.x, Target.transform.position.y, camera.transform.position.z);
                        if (Status == FocusStatus.Focusing)
                        {
                            camera.transform.position = targetPosition;
                        }
                        else // FocusStatus.Transitioning
                        {
                            float timeDelta = Time.deltaTime;
                            float timeFraction = 0f;
                            if (timeDelta >= remainingTransitioningTime)
                            {
                                timeFraction = 1f;
                                remainingTransitioningTime = 0;
                                Status = FocusStatus.Focusing;
                            }
                            else
                            {
                                timeFraction = timeDelta / remainingTransitioningTime;
                                remainingTransitioningTime -= timeDelta;
                            }
                            camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPosition, (targetPosition - camera.transform.position).magnitude * timeFraction);
                        }
                    }
                    else
                    {
                        Target = null;
                        Status = FocusStatus.NotFocusing;
                    }
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
