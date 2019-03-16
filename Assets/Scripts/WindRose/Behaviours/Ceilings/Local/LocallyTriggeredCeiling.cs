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
                using World;
                using World.Layers.Ceiling;
                using Entities.Objects;

                /// <summary>
                ///   This subtype of ceiling layer is shown when no "allowed
                ///     objects" are under it, and will be hidden/translucent
                ///     (such behaviour may be chosen) when at least one "allowed
                ///     object" is under it.
                /// </summary>
                [RequireComponent(typeof(BoxCollider2D))]
                [RequireComponent(typeof(Ceiling))]
                public class LocallyTriggeredCeiling : MonoBehaviour
                {
                    /// <summary>
                    ///   The allowed objects. They may be changed in runtime to
                    ///     allow more objects to be triggers of show/hide objects.
                    /// </summary>
                    [SerializeField]
                    private List<GameObject> triggeringObjects;
                    private HashSet<GameObject> triggeringObjectsSet;

                    /**
                     * Currently triggering objects.
                     */
                    private HashSet<GameObject> currentStayingTriggers;

                    /// <summary>
                    ///   The display mode to be used when an allowed object
                    ///     is under the ceiling. <see cref="Ceiling.DisplayMode.VISIBLE"/>
                    ///     cannot be chosen: it will be replaced by
                    ///     <see cref="Ceiling.DisplayMode.VISIBLE"/> instead.
                    /// </summary>
                    [SerializeField]
                    private Ceiling.DisplayMode displayModeWhenTriggering;

                    /// <summary>
                    ///   Width of this object. It should match what is painted on
                    ///     its tilemap.
                    /// </summary>
                    [SerializeField]
                    private uint width;

                    /// <summary>
                    ///   Height of this object. It should match what is painted on
                    ///     its tilemap.
                    /// </summary>
                    [SerializeField]
                    private uint height;

                    private Ceiling ceiling;
                    private Map map;

                    private void Awake()
                    {
                        CeilingLayer ceilingLayer = Support.Utils.Layout.RequireComponentInParent<CeilingLayer>(this);
                        map = Support.Utils.Layout.RequireComponentInParent<Map>(ceilingLayer);
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
                        GameObject gameObject = collider.gameObject;
                        Object mapObject = gameObject.GetComponent<Object>();
                        if (mapObject.ParentMap == map && triggeringObjectsSet.Contains(gameObject))
                        {
                            currentStayingTriggers.Add(gameObject);
                        }
                    }

                    private void OnTriggerExit2D(Collider2D collider)
                    {
                        GameObject gameObject = collider.gameObject;
                        currentStayingTriggers.Remove(gameObject);
                    }

                    /// <summary>
                    ///   Adds an allowed object.
                    /// </summary>
                    /// <param name="trigger">The object to add</param>
                    public void AddTrigger(GameObject trigger)
                    {
                        triggeringObjectsSet.Add(trigger);
                    }

                    /// <summary>
                    ///   Removes an allowed object.
                    /// </summary>
                    /// <param name="trigger">The object to remove</param>
                    public void RemoveTrigger(GameObject trigger)
                    {
                        triggeringObjectsSet.Remove(trigger);
                    }

                    /// <summary>
                    ///   Checks whether the object is allowed.
                    /// </summary>
                    /// <param name="trigger">The object to check</param>
                    /// <returns>Whether it is allowed</returns>
                    public bool HasTrigger(GameObject trigger)
                    {
                        return triggeringObjectsSet.Contains(trigger);
                    }

                    /// <summary>
                    ///   Removes all the triggered objects.
                    /// </summary>
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
