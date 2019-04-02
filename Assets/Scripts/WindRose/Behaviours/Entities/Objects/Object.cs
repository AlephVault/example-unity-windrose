using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            using Types;
            using World;
            using World.Layers.Objects;

            /// <summary>
            ///   <para>
            ///     Aside of the map itself, map objects are the spirit of the party.
            ///   </para>
            ///   <para>
            ///     Map objects are the middle step between the user interface (or
            ///       artificial intelligence) and the underlying map and object
            ///       strategies: They will provide the behaviour to move, teleport,
            ///       attach to -and detach from- maps, and look in different directions.
            ///   </para>
            ///   <para>
            ///     They will also provide events to help other (dependent) behaviours
            ///       to refresh appropriately (e.g. animation change, movement start,
            ///       ...).
            ///   </para>
            /// </summary>
            [ExecuteInEditMode]
            [RequireComponent(typeof(Pausable))]
            [RequireComponent(typeof(Snapped))]
            [RequireComponent(typeof(ObjectStrategyHolder))]
            public class Object : MonoBehaviour, Common.Pausable.IPausable
            {
                /* *********************** Initial data *********************** */

                /// <summary>
                ///   The width of this object, in map cells.
                /// </summary>
                [SerializeField]
                private uint width = 1;

                /// <summary>
                ///   The height of this object, in map cells.
                /// </summary>
                [SerializeField]
                private uint height = 1;

                /// <summary>
                ///   Map objects MAY have a visual considered the MAIN one. This
                ///     is not mandatory but, if done, it will ensure the main visual
                ///     is forevert tied to this object.
                /// </summary>
                [SerializeField]
                private Visuals.Visual mainVisual;

                /* *********************** Additional data and state *********************** */

                /// <summary>
                ///   The map this object is currently attached to.
                /// </summary>
                private Map parentMap = null;

                // The visual objects that are attached to this object.
                private HashSet<Visuals.Visual> visuals = new HashSet<Visuals.Visual>();

                private bool initialized = false;

                /* *********************** Public properties *********************** */

                /// <summary>
                ///   Gets the parent map this object is attached to. See <see cref="parentMap"/>.
                /// </summary>
                public Map ParentMap { get { return parentMap; } }

                /// <summary>
                ///   See <see cref="mainVisual"/>.
                /// </summary>
                public Visuals.Visual MainVisual { get { return mainVisual; } }

                /// <summary>
                ///   Returns the visual objects currently attached to this object.
                /// </summary>
                public IEnumerator<Visuals.Visual> Visuals
                {
                    get
                    {
                        return visuals.GetEnumerator();
                    }
                }

                /// <summary>
                ///   See <see cref="width"/>.
                /// </summary>
                public uint Width { get { return width; } } // Referencing directly allows us to query the width without a map assigned yet.

                /// <summary>
                ///   See <see cref="height"/>.
                /// </summary>
                public uint Height { get { return height; } } // Referencing directly allows us to query the height without a map assigned yet.

                /// <summary>
                ///   The current X position of the object inside the attached map.
                /// </summary>
                public uint X { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).X; } }

                /// <summary>
                ///   The current Y position of the object inside the attached map.
                /// </summary>
                public uint Y { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Y; } }

                /// <summary>
                ///   The opposite X position of this object inside the attached map, with
                ///     respect of its <see cref="width"/> value.
                /// </summary>
                /// <remarks>(Xf, Yf) point is the opposite corner of (X, Y).</remarks>
                public uint Xf { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).X + Width - 1; } }

                /// <summary>
                ///   The opposite Y position of this object inside the attached map, with
                ///     respect of its <see cref="height"/> value.
                /// </summary>
                /// <remarks>(Xf, Yf) point is the opposite corner of (X, Y).</remarks>
                public uint Yf { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Y + Height - 1; } }

                /// <summary>
                ///   The current movement of the object inside the attached map.
                ///   It will be <c>null</c> if the object is not moving.
                /// </summary>
                public Direction? Movement { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Movement; } }

                /// <summary>
                ///   The strategy holder of this object.
                /// </summary>
                public ObjectStrategyHolder StrategyHolder { get; private set; }

                /// <summary>
                ///   Tells whether this object is paused.
                /// </summary>
                public bool Paused { get; private set; }

                /// <summary>
                ///   Tells whether the animations of this object are paused.
                ///   For certain game configuration, you may have this in <c>false</c>
                ///     even while having <see cref="Paused"/> in true.
                /// </summary>
                public bool AnimationsPaused { get; private set; }

                /* *********************** Events *********************** */

                [Serializable]
                public class UnityAttachedEvent : UnityEvent<Map> { }

                /// <summary>
                ///   Event that triggers when this object is attached to a map.
                /// </summary>
                public readonly UnityAttachedEvent onAttached = new UnityAttachedEvent();

                /// <summary>
                ///   Event that triggers when this object is detached from its map.
                /// </summary>
                public readonly UnityEvent onDetached = new UnityEvent();

                [Serializable]
                public class UnityMovementEvent : UnityEvent<Direction> { }
                [Serializable]
                public class UnityOptionalMovementEvent : UnityEvent<Direction?> { }

                /// <summary>
                ///   Event that triggers when the object starts moving.
                /// </summary>
                public readonly UnityMovementEvent onMovementStarted = new UnityMovementEvent();

                /// <summary>
                ///   Event that triggers when the object cancels its movement.
                /// </summary>
                public readonly UnityOptionalMovementEvent onMovementCancelled = new UnityOptionalMovementEvent();

                /// <summary>
                ///   Event that triggers when the object completes its movement into a cell.
                /// </summary>
                public readonly UnityMovementEvent onMovementFinished = new UnityMovementEvent();

                [Serializable]
                public class UnityPropertyUpdateEvent : UnityEvent<string, object, object> { }

                /// <summary>
                ///   Event that triggers when the object changes one of its properties.
                ///   This event is triggered explicitly via capabilities inside <see cref="Strategies.ObjectStrategy.PropertyWasUpdated(string, object, object)"/>.
                /// </summary>
                public readonly UnityPropertyUpdateEvent onPropertyUpdated = new UnityPropertyUpdateEvent();

                [Serializable]
                public class UnityTeleportedEvent : UnityEvent<uint, uint> { }

                /// <summary>
                ///   Event that triggers after the object is teleported to a certain position inside the map.
                /// </summary>
                public readonly UnityTeleportedEvent onTeleported = new UnityTeleportedEvent();

                // These callbacks are run when this map object starts.
                private Action startCallbacks = delegate() {};
                // These callbacks are run when this map object updates and is not paused.
                private Action updateCallbacks = delegate () { };
                // These callbacks are run when this map object updates and animations are not paused.
                private Action updateAnimationCallbacks = delegate () { };

                // Gets all the children visual objects.
                private IEnumerable<Visuals.Visual> GetChildVisuals()
                {
                    return from component in (
                      from index in Enumerable.Range(0, transform.childCount)
                      select transform.GetChild(index).GetComponent<Visuals.Visual>()
                    )
                    where component != null
                    select component;
                }

                private void Awake()
                {
                    // Cleans the initial value of mainVisual
                    if (!new HashSet<Visuals.Visual>(GetChildVisuals()).Contains(mainVisual))
                    {
                        mainVisual = null;
                    }

                    StrategyHolder = GetComponent<ObjectStrategyHolder>();
                    onAttached.AddListener(delegate (Map newParentMap)
                    {
                        /*
                         * Attaching to a map involves:
                         * 1. The actual "parent" of the object will be a child of the RelatedMap being an ObjectsLayer.
                         * 2. We set the parent transform of the object to such ObjectsLayer's transform.
                         * 3. Finally we must ensure the transform.localPosition be updated accordingly (i.e. forcing a snap).
                         */
                        parentMap = newParentMap;
                        ObjectsLayer ObjectsLayer = parentMap.GetComponentInChildren<ObjectsLayer>();
                        transform.parent = newParentMap.ObjectsLayer.transform;
                        transform.localPosition = new Vector3(
                            X * ObjectsLayer.GetCellWidth(),
                            Y * ObjectsLayer.GetCellHeight(),
                            0
                        );
                    });
                    onDetached.AddListener(delegate ()
                    {
                        parentMap = null;
                    });

                    // Get related components that need to run in a particular order
                    Movable movable = GetComponent<Movable>();
                    Snapped snapped = GetComponent<Snapped>();

                    // Add them to start, update, and animationUpdate callbacks
                    if (Application.isPlaying)
                    {
                        if (movable != null)
                        {
                            updateCallbacks += movable.DoUpdate;
                        }
                        if (snapped != null)
                        {
                            updateCallbacks += snapped.DoUpdate;
                        }
                    }
                }

                // Attaches all the visuals that are direct children.
                private void InitVisuals()
                {
                    foreach (Visuals.Visual visual in GetChildVisuals())
                    {
                        AddVisual(visual);
                        visual.DoStart();
                    }
                }

                void Start()
                {
                    Initialize();
                    // Run the start on other components.
                    startCallbacks();
                    // THEN instantiate all the overlays.
                    if (Application.isPlaying)
                    {
                        InitVisuals();
                    }
                }

                private void Update()
                {
                    // Updates the local callbacks.
                    if (!Paused) updateCallbacks();
                    foreach (Visuals.Visual visual in visuals) visual.DoUpdate(); 
                }

                void OnDestroy()
                {
                    Detach();
                    startCallbacks = delegate () {};
                    updateCallbacks = delegate () {};
                    updateAnimationCallbacks = delegate () {};
                    onAttached.RemoveAllListeners();
                    onDetached.RemoveAllListeners();
                    onMovementStarted.RemoveAllListeners();
                    onMovementCancelled.RemoveAllListeners();
                    onMovementFinished.RemoveAllListeners();
                    onPropertyUpdated.RemoveAllListeners();
                    onTeleported.RemoveAllListeners();
                }

                /// <summary>
                ///   <para>
                ///     This method is called when the map is initialized (first) and when this
                ///       object starts its execution in the scene. Both conditions have to be
                ///       fulfilled for the logic to initialize.
                ///   </para>
                ///   <para>
                ///     For this method to succeed, this object must be a child object of one
                ///       holding a <see cref="ObjectsLayer"/> which in turn must be inside a
                ///       <see cref="Map"/>, and the map must have dimensions that allow this
                ///       object considering its size and initial position.
                ///   </para>
                /// </summary>
                public void Initialize()
                {
                    if (!Application.isPlaying) return;

                    if (initialized)
                    {
                        return;
                    }

                    // We will make use of strategy
                    if (StrategyHolder == null)
                    {
                        throw new Exception("An object strategy holder is required when the map object initializes.");
                    }
                    else
                    {
                        StrategyHolder.Initialize();
                    }

                    try
                    {
                        Map parentMap;
                        // We find the parent map like this: (current) -> ObjectsLayer -> map
                        if (transform.parent != null && transform.parent.parent != null)
                        {
                            parentMap = transform.parent.parent.GetComponent<Map>();
                        }
                        else
                        {
                            parentMap = null;
                        }
                        // It is OK to have no map! However, the object will be detached and
                        //   almost nothing useful will be able to be done to the object until
                        //   it is attached.
                        if (parentMap != null)
                        {
                            // Here we are with an object that was instantiated inside a map's
                            //   hierarchy. We will not proceed and mark as initialized if
                            //   the underlying map is not initialized beforehand: otherwise
                            //   we would not necessarily know the appropriate dimensions.
                            if (!parentMap.Initialized) return;
                            // And we also keep its objects layer
                            Layout.RequireComponentInParent<ObjectsLayer>(gameObject);
                            // Then we calculate the cell position from the grid in the layer.
                            Grid grid = Layout.RequireComponentInParent<Grid>(gameObject);
                            Vector3Int cellPosition = grid.WorldToCell(transform.position);
                            // Then we initialize, and perhaps it may explode due to exception.
                            Attach(parentMap, (uint)cellPosition.x, (uint)cellPosition.y);
                        }
                        // After success of a standalone map object being initialized, either
                        //   by itself or by the parent map invoking the initialization.
                        initialized = true;
                    }
                    catch (Layout.MissingComponentInParentException)
                    {
                        // nothing - diaper
                    }
                }

                /// <summary>
                ///   Detaches the object from its map. See <see cref="ObjectsManagementStrategyHolder.Detach(ObjectStrategyHolder)"/>
                ///     for more details.
                /// </summary>
                /// <remarks>It does nothing if the object is not attached to a map.</remarks>
                public void Detach()
                {
                    // There are some times at startup when the MapState object may be null.
                    // That's why we run the conditional.
                    //
                    // For the general cases, Detach will find a mapObjectState attached.
                    if (parentMap != null) parentMap.StrategyHolder.Detach(StrategyHolder);
                }

                /// <summary>
                ///   Attaches the object to a map.
                /// </summary>
                /// <param name="map">The map to attach the object to</param>
                /// <param name="x">The new x position of the object</param>
                /// <param name="y">The new y position of the object</param>
                /// <param name="force">
                ///   If true, the object will be detached from its previous map, and attached to this one.
                ///   If false and the object is already attached to a map, an error will raise.
                /// </param>
                public void Attach(Map map, uint x, uint y, bool force = false)
                {
                    if (force) Detach();
                    // TODO: Clamp x, y? or raise exception?
                    map.Attach(this, x, y);
                }

                /// <summary>
                ///   Teleports the object to another position in the same map.
                /// </summary>
                /// <param name="x">The new x position of the object</param>
                /// <param name="y">The new y position of the object</param>
                /// <remarks>Does nothing if the object is paused.</remarks>
                public void Teleport(uint x, uint y)
                {
                    if (parentMap != null && !Paused) parentMap.StrategyHolder.Teleport(StrategyHolder, x, y);
                }

                /// <summary>
                ///   Starts (allocates) a new movement. This method is intended to be invoked from
                ///     <see cref="Movable"/> and it is not intended for the developer to invoke it.
                /// </summary>
                /// <param name="movementDirection">The direction of the movement to start</param>
                /// <param name="continuated">Whether the movement should be considered a continuation of the previous one</param>
                /// <remarks>Does nothing if the object is paused.</remarks>
                public bool StartMovement(Direction movementDirection, bool continuated = false)
                {
                    return parentMap != null && !Paused && parentMap.StrategyHolder.MovementStart(StrategyHolder, movementDirection, continuated);
                }

                /// <summary>
                ///   Finishes an already started movement. This method is intended to be invoked
                ///     from <see cref="Movable"/> and it is not intended for the developer to invoke it.
                /// </summary>
                /// <returns>Does nothing if the object is paused.</returns>
                public bool FinishMovement()
                {
                    return parentMap != null && !Paused && parentMap.StrategyHolder.MovementFinish(StrategyHolder);
                }

                /// <summary>
                ///   Cancels an already started movement. This method is intended to be invoked
                ///     from <see cref="Movable"/> and it is not intended for the developer to invoke it.
                /// </summary>
                /// <returns>Does nothing if the object is paused.</returns>
                public bool CancelMovement()
                {
                    return parentMap != null && !Paused && parentMap.StrategyHolder.MovementCancel(StrategyHolder);
                }

                /// <summary>
                ///   See <see cref="ObjectsLayer.GetCellWidth"/>.
                /// </summary>
                /// <returns>The width of the cells of its parent Objects Layer</returns>
                public float GetCellWidth()
                {
                    return GetComponentInParent<ObjectsLayer>().GetCellWidth();
                }

                /// <summary>
                ///   See <see cref="ObjectsLayer.GetCellHeight"/>.
                /// </summary>
                /// <returns>The height of the cells of its parent Objects Layers</returns>
                public float GetCellHeight()
                {
                    return GetComponentInParent<ObjectsLayer>().GetCellHeight();
                }

                /// <summary>
                ///   Flags the object, and its animations, as unpaused. This also invokes <see cref="Common.Pausable.Pause(bool)"/>
                ///     on the pausable components of each attached visual.
                /// </summary>
                /// <param name="fullFreeze">If <c>true</c>, also flags the object animations as paused</param>
                public void Pause(bool fullFreeze)
                {
                    Paused = true;
                    AnimationsPaused = fullFreeze;
                    foreach(Visuals.Visual visual in visuals)
                    {
                        visual.GetComponent<Common.Pausable>().Pause(fullFreeze);
                    }
                }

                /// <summary>
                ///   Flags the object, and its animations, as unpaused. This also invokes <see cref="Common.Pausable.Resume"/>
                ///     on the pausable components of each attached visual.
                /// </summary>
                public void Resume()
                {
                    Paused = false;
                    AnimationsPaused = false;
                    foreach (Visuals.Visual visual in visuals)
                    {
                        visual.GetComponent<Common.Pausable>().Resume();
                    }
                }

                /// <summary>
                ///   Attaches the visual to this object, if it is not
                ///     attached. Raises an exception if the visual is
                ///     the main visual in another object, and fails
                ///     silently if the visual is null.
                /// </summary>
                /// <param name="visual">The visual to add</param>
                /// <returns>Whether the visual was just added</returns>
                public bool AddVisual(Visuals.Visual visual)
                {
                    if (!visual || visuals.Contains(visual)) return false;
                    if (visual.IsMain && visual.RelatedObject != this)
                    {
                        throw new Exception("The visual object trying to add is the main visual in another object");
                    }
                    visual.Detach();
                    visuals.Add(visual);
                    visual.OnAttached(this);
                    return true;
                }

                /// <summary>
                ///   Detaches the visual from this object, if it is
                ///     attached.
                /// </summary>
                /// <param name="visual">The visual to remove</param>
                /// <returns>Whether the visual was just removed</returns>
                public bool PopVisual(Visuals.Visual visual)
                {
                    if (!visuals.Contains(visual)) return false;
                    if (visual.IsMain)
                    {
                        throw new Exception("The visual object trying to remove is the main visual in this object");
                    }
                    visuals.Remove(visual);
                    visual.OnDetached(this);
                    return true;
                }
            }
        }
    }
}