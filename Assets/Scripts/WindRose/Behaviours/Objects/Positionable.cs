using System;
using UnityEngine;
using UnityEngine.Events;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            using Types;
            using World;
            using World.Layers.Objects;

            [ExecuteInEditMode]
            [RequireComponent(typeof(Pausable))]
            [RequireComponent(typeof(ObjectStrategyHolder))]
            public class Positionable : MonoBehaviour
            {
                /**
                 * A positionable object updates its position and solidness status
                 *   to its holding layer.
                 * 
                 * It will have behaviors like walking and teleporting.
                 */

                /* *********************** Initial data *********************** */

                [SerializeField]
                private uint width = 1;

                [SerializeField]
                private uint height = 1;

                /* *********************** Additional data *********************** */

                private Map parentMap = null;
                private bool paused = false;
                private bool initialized = false;

                /* *********************** Public properties *********************** */

                public Map ParentMap { get { return parentMap; } }
                public uint Width { get { return width; } } // Referencing directly allows us to query the width without a map assigned yet.
                public uint Height { get { return height; } } // Referencing directly allows us to query the height without a map assigned yet.
                public uint X { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).X; } }
                public uint Y { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Y; } }
                public uint Xf { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).X + Width - 1; } }
                public uint Yf { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Y + Height - 1; } }
                public Direction? Movement { get { return parentMap.StrategyHolder.StatusFor(StrategyHolder).Movement; } }
                public ObjectStrategyHolder StrategyHolder { get; private set; }

                /* *********************** Events *********************** */
                [Serializable]
                public class UnityAttachedEvent : UnityEvent<Map> { }
                public readonly UnityAttachedEvent onAttached = new UnityAttachedEvent();
                public readonly UnityEvent onDetached = new UnityEvent();
                [Serializable]
                public class UnityMovementEvent : UnityEvent<Types.Direction> { }
                [Serializable]
                public class UnityOptionalMovementEvent : UnityEvent<Types.Direction?> { }
                public readonly UnityMovementEvent onMovementStarted = new UnityMovementEvent();
                public readonly UnityOptionalMovementEvent onMovementCancelled = new UnityOptionalMovementEvent();
                public readonly UnityMovementEvent onMovementFinished = new UnityMovementEvent();
                [Serializable]
                public class UnityPropertyUpdateEvent : UnityEvent<string, object, object> { }
                public readonly UnityPropertyUpdateEvent onPropertyUpdated = new UnityPropertyUpdateEvent();
                [Serializable]
                public class UnityTeleportedEvent : UnityEvent<uint, uint> { }
                public readonly UnityTeleportedEvent onTeleported = new UnityTeleportedEvent();

                private void Awake()
                {
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
                        ObjectsLayer objectsLayer = parentMap.GetComponentInChildren<ObjectsLayer>();
                        transform.parent = objectsLayer.transform;
                        transform.localPosition = new Vector3(
                            X * objectsLayer.GetCellWidth(),
                            Y * objectsLayer.GetCellHeight(),
                            0
                        );
                    });
                    onDetached.AddListener(delegate ()
                    {
                        parentMap = null;
                    });
                }

                void Start()
                {
                    Initialize();
                }

                void OnDestroy()
                {
                    Detach();
                    onAttached.RemoveAllListeners();
                    onDetached.RemoveAllListeners();
                    onMovementStarted.RemoveAllListeners();
                    onMovementCancelled.RemoveAllListeners();
                    onMovementFinished.RemoveAllListeners();
                    onPropertyUpdated.RemoveAllListeners();
                    onTeleported.RemoveAllListeners();
                }

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
                        throw new Exception("An object strategy holder is required when the positionable initializes.");
                    }
                    else
                    {
                        StrategyHolder.Initialize();
                    }

                    try
                    {
                        // We find the parent map like this: (current) -> ObjectsLayer -> map
                        parentMap = Layout.RequireComponentInParent<Map>(transform.parent.gameObject);
                        if (!parentMap.Initialized) return;
                        // And we also keep its objects layer
                        Layout.RequireComponentInParent<ObjectsLayer>(gameObject);
                        // Then we calculate the cell position from the grid in the layer.
                        Grid grid = Layout.RequireComponentInParent<Grid>(gameObject);
                        Vector3Int cellPosition = grid.WorldToCell(transform.position);
                        // Then we initialize, and perhaps it may explode due to exception.
                        ParentMap.StrategyHolder.Attach(StrategyHolder, (uint)cellPosition.x, (uint)cellPosition.y);
                        initialized = true;
                    }
                    catch (Layout.MissingComponentInParentException)
                    {
                        // nothing - diaper
                    }
                }

                public void Detach()
                {
                    // There are some times at startup when the MapState object may be null.
                    // That's why we run the conditional.
                    //
                    // For the general cases, Detach will find a mapObjectState attached.
                    if (parentMap != null) parentMap.StrategyHolder.Detach(StrategyHolder);
                }

                public void Attach(Map map, uint x, uint y, bool force = false)
                {
                    if (force) Detach();
                    // TODO: Clamp x, y? or raise exception?
                    map.StrategyHolder.Attach(StrategyHolder, x, y);
                }

                public void Teleport(uint x, uint y)
                {
                    if (parentMap != null && !paused) parentMap.StrategyHolder.Teleport(StrategyHolder, x, y);
                }

                public bool StartMovement(Direction movementDirection, bool continuated = false)
                {
                    return parentMap != null && !paused && parentMap.StrategyHolder.MovementStart(StrategyHolder, movementDirection, continuated);
                }

                public bool FinishMovement()
                {
                    return parentMap != null && !paused && parentMap.StrategyHolder.MovementFinish(StrategyHolder);
                }

                public bool CancelMovement()
                {
                    return parentMap != null && !paused && parentMap.StrategyHolder.MovementCancel(StrategyHolder);
                }

                public float GetCellWidth()
                {
                    return GetComponentInParent<ObjectsLayer>().GetCellWidth();
                }

                public float GetCellHeight()
                {
                    return GetComponentInParent<ObjectsLayer>().GetCellHeight();
                }

                void Pause(bool fullFreeze)
                {
                    paused = true;
                }

                void Resume()
                {
                    paused = false;
                }
            }
        }
    }
}