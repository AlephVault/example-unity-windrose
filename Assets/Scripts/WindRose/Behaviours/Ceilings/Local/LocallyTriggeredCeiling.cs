using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Ceilings
        {
            namespace Local
            {
                [RequireComponent(typeof(BoxCollider2D))]
                [RequireComponent(typeof(Ceiling))]
                public class LocallyTriggeredCeiling : MonoBehaviour
                {
                    /**
                     * Shows/Hides the ceiling depending on the triggers on it.
                     */

                    /**
                     * These triggers are the allowed ones - when they collide to this
                     *   ceiling, the ceiling will change its state.
                     */
                    [SerializeField]
                    private List<GameObject> triggeringObjects;
                    private HashSet<GameObject> triggeringObjectsSet;

                    /**
                     * Currently triggering objects.
                     */
                    private HashSet<GameObject> currentStayingTriggers;

                    /**
                     * Display state to change when allowed triggers are present.
                     */
                    [SerializeField]
                    private Ceiling.DisplayMode displayModeWhenTriggering;

                    /**
                     * Width and Height to account for on this local triggering.
                     */
                    [SerializeField]
                    private uint width;
                    [SerializeField]
                    private uint height;

                    private Ceiling ceiling;

                    private void Awake()
                    {
                        // VISIBLE is not allowed, since there would be no
                        //   change: the ceiling would never take off.
                        if (displayModeWhenTriggering == Ceiling.DisplayMode.VISIBLE)
                        {
                            displayModeWhenTriggering = Ceiling.DisplayMode.HIDDEN;
                        }
                        currentStayingTriggers = new HashSet<GameObject>();
                        triggeringObjectsSet = new HashSet<GameObject>(triggeringObjects);
                        ceiling = GetComponent<Ceiling>();
                    }

                    private void Start()
                    {
                        BoxCollider2D collider = GetComponent<BoxCollider2D>();
                        Tilemap tilemap = GetComponent<Tilemap>();
                        collider.isTrigger = true;
                        collider.size = new Vector2(tilemap.cellSize.x * width, tilemap.cellSize.y * height);
                        collider.offset = collider.size / 2;
                        // Just decrease the triger width slightly, to avoid colliding on the edges.
                        collider.size = collider.size - Vector2.one * 0.1f;
                    }

                    private void OnTriggerEnter2D(Collider2D collider)
                    {
                        if (triggeringObjectsSet.Contains(collider.gameObject))
                        {
                            currentStayingTriggers.Add(collider.gameObject);
                        }
                    }

                    private void OnTriggerExit2D(Collider2D collider)
                    {
                        currentStayingTriggers.Remove(collider.gameObject);
                    }

                    public void AddTrigger(GameObject trigger)
                    {
                        triggeringObjectsSet.Add(trigger);
                    }

                    public void RemoveTrigger(GameObject trigger)
                    {
                        triggeringObjectsSet.Remove(trigger);
                    }

                    public bool HasTrigger(GameObject trigger)
                    {
                        return triggeringObjectsSet.Contains(trigger);
                    }

                    public void ClearTriggers()
                    {
                        triggeringObjectsSet.Clear();
                    }

                    private void Update()
                    {
                        ceiling.displayMode = (triggeringObjectsSet.Count != 0 && triggeringObjectsSet.Overlaps(currentStayingTriggers)) ? displayModeWhenTriggering : Ceiling.DisplayMode.VISIBLE;
                    }
                }
            }
        }
    }
}
