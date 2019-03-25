using System;
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
            using World.Layers.Entities;

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
            [RequireComponent(typeof(ObjectStrategyHolder))]
            public class Object : Common.Entity
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
                ///   Initial add-ons for this object, if needed. These add-ons will
                ///     be added above this object.
                /// </summary>
                [SerializeField]
                private List<AddOns.AddOn> overlays;

                /// <summary>
                ///   Initial add-ons for this object, if needed. These add-ons will
                ///     be added below this object.
                /// </summary>
                [SerializeField]
                private List<AddOns.AddOn> underlays;

                /* *********************** Additional data and state *********************** */

                /// <summary>
                ///   The map this object is currently attached to.
                /// </summary>
                private Map parentMap = null;

                private bool initialized = false;

                /* *********************** Public properties *********************** */

                /// <summary>
                ///   Gets the parent map this object is attached to. See <see cref="parentMap"/>.
                /// </summary>
                public override Map ParentMap { get { return parentMap; } }

                /// <summary>
                ///   The object's overlays group.
                /// </summary>
                public AddOns.AddOnGroup OverlaysGroup { get; private set; }

                /// <summary>
                ///   The object's underlays group.
                /// </summary>
                public AddOns.AddOnGroup UnderlaysGroup { get; private set; }

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
                public override uint X { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).X; } }

                /// <summary>
                ///   The current Y position of the object inside the attached map.
                /// </summary>
                public override uint Y { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Y; } }

                /// <summary>
                ///   Gets the appropriate sub-layer for this entity. For this one, the middle sub-layer is the
                ///     appropriate.
                /// </summary>
                /// <param name="layer">The entities layer to take the sub-layer from</param>
                /// <returns>The middle sub-layer</returns>
                protected override SortingSubLayer GetSubLayerFrom(EntitiesLayer layer)
                {
                    return layer.ObjectsSubLayer;
                }

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
                public override Direction? Movement { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Movement; } }

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

                private void InstantiateAddOnGroups()
                {
                    GameObject underlaysGroupObject = new GameObject("Underlays");
                    GameObject overlaysGroupObject = new GameObject("Overlays");

                    UnderlaysGroup = Layout.AddComponent<AddOns.AddOnGroup>(underlaysGroupObject, new Dictionary<string, object>()
                    {
                        { "addOnGroupType", AddOns.AddOnGroup.AddOnGroupType.Underlay },
                        { "relatedObject", this }
                    });
                    OverlaysGroup = Layout.AddComponent<AddOns.AddOnGroup>(overlaysGroupObject, new Dictionary<string, object>()
                    {
                        { "addOnGroupType", AddOns.AddOnGroup.AddOnGroupType.Overlay },
                        { "relatedObject", this }
                    });

                    foreach (AddOns.AddOn addOn in underlays)
                    {
                        UnderlaysGroup.Add(Instantiate(addOn.gameObject).GetComponent<AddOns.AddOn>());
                    }
                    foreach (AddOns.AddOn addOn in overlays)
                    {
                        OverlaysGroup.Add(Instantiate(addOn.gameObject).GetComponent<AddOns.AddOn>());
                    }
                }

                private void Awake()
                {
                    StrategyHolder = GetComponent<ObjectStrategyHolder>();
                    onAttached.AddListener(delegate (Map newParentMap)
                    {
                        /*
                         * Attaching to a map involves:
                         * 1. The actual "parent" of the object will be a child of the RelatedMap being an EntitiesLayer.
                         * 2. We set the parent transform of the object to such EntitiesLayer's transform.
                         * 3. Finally we must ensure the transform.localPosition be updated accordingly (i.e. forcing a snap).
                         */
                        parentMap = newParentMap; 
                        EntitiesLayer entitiesLayer = parentMap.GetComponentInChildren<EntitiesLayer>();
                        EnsureAppropriateVerticalSorting();
                        transform.localPosition = new Vector3(
                            X * entitiesLayer.GetCellWidth(),
                            Y * entitiesLayer.GetCellHeight(),
                            0
                        );
                    });
                    onDetached.AddListener(delegate ()
                    {
                        parentMap = null;
                    });

                    // Get related components that need to run in a particular order
                    Oriented oriented = GetComponent<Oriented>();
                    Movable movable = GetComponent<Movable>();
                    Snapped snapped = GetComponent<Snapped>();
                    Sorted sorted = GetComponent<Sorted>();
                    Animated animated = GetComponent<Animated>();

                    // Add them to start, update, and animationUpdate callbacks
                    if (oriented != null)
                    {
                        startCallbacks += oriented.DoStart;
                        updateCallbacks += oriented.DoUpdate;
                    }
                    if (movable != null)
                    {
                        updateCallbacks += movable.DoUpdate;
                    }
                    if (snapped != null)
                    {
                        updateCallbacks += snapped.DoUpdate;
                    }
                    if (sorted != null)
                    {
                        updateCallbacks += sorted.DoUpdate;
                    }
                    if (animated != null)
                    {
                        startCallbacks += animated.DoStart;
                        updateAnimationCallbacks += animated.DoUpdate;
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
                        InstantiateAddOnGroups();
                    }
                }

                protected override void UpdatePipeline()
                {
                    // Updates the local callbacks.
                    if (!Paused) updateCallbacks();
                    if (!AnimationsPaused) updateAnimationCallbacks();
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
                    if (Application.isPlaying)
                    {
                        Destroy(OverlaysGroup);
                        Destroy(UnderlaysGroup);
                    }
                }

                /// <summary>
                ///   <para>
                ///     This method is called when the map is initialized (first) and when this
                ///       object starts its execution in the scene. Both conditions have to be
                ///       fulfilled for the logic to initialize.
                ///   </para>
                ///   <para>
                ///     For this method to succeed, this object must be a child object of one
                ///       holding a <see cref="EntitiesLayer"/> which in turn must be inside a
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
                        // We find the parent map like this: (current) -> EntitiesLayer -> map
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
                            Layout.RequireComponentInParent<EntitiesLayer>(gameObject);
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
                ///   See <see cref="EntitiesLayer.GetCellWidth"/>.
                /// </summary>
                /// <returns>The width of the cells of its parent Objects Layer</returns>
                public float GetCellWidth()
                {
                    return GetComponentInParent<EntitiesLayer>().GetCellWidth();
                }

                /// <summary>
                ///   See <see cref="EntitiesLayer.GetCellHeight"/>.
                /// </summary>
                /// <returns>The height of the cells of its parent Objects Layers</returns>
                public float GetCellHeight()
                {
                    return GetComponentInParent<EntitiesLayer>().GetCellHeight();
                }

                /// <summary>
                ///   Flags the object as paused. This also invokes <see cref="AddOns.AddOnGroup.Pause(bool)"/> on
                ///     this object's <see cref="OverlaysGroup"/> and <see cref="UnderlaysGroup"/>.
                /// </summary>
                /// <param name="fullFreeze">If <c>true</c>, also flags the object animations as paused</param>
                public override void Pause(bool fullFreeze)
                {
                    Paused = true;
                    AnimationsPaused = fullFreeze;
                    if (OverlaysGroup) OverlaysGroup.Pause(fullFreeze);
                    if (UnderlaysGroup) UnderlaysGroup.Pause(fullFreeze);
                }

                /// <summary>
                ///   Flags the object, and its animations, as unpaused. This also invokes <see cref="AddOns.AddOnGroup.Resume"/> on
                ///     this object's <see cref="OverlaysGroup"/> and <see cref="UnderlaysGroup"/>.
                /// </summary>
                public override void Resume()
                {
                    Paused = false;
                    AnimationsPaused = false;
                    if (OverlaysGroup) OverlaysGroup.Resume();
                    if (UnderlaysGroup) UnderlaysGroup.Resume();
                }
            }
        }
    }
}