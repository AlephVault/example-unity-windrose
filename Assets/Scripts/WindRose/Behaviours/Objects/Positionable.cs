﻿using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            using Types;
            using World;
            using World.Layers;

            [ExecuteInEditMode]
            [RequireComponent(typeof(Pausable))]
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
                public Strategies.ObjectStrategyHolder StrategyHolder { get; private set; }

                private void Awake()
                {
                    StrategyHolder = GetComponent<Strategies.ObjectStrategyHolder>();
                }

                void Start()
                {
                    Initialize();
                }

                void OnDestroy()
                {
                    Detach();
                }

                void OnAttached(object[] args)
                {
                    /*
                     * Attaching to a map involves:
                     * 1. Getting the Map in arguments.
                     * 2. The actual "parent" of the object will be a child of the RelatedMap being an ObjectsLayer.
                     * 3. We set the parent transform of the object to such ObjectsLayer's transform.
                     * 4. Finally we must ensure the transform.localPosition be updated accordingly (i.e. forcing a snap).
                     */
                    parentMap = (Map)args[0];
                    ObjectsLayer objectsLayer = parentMap.GetComponentInChildren<ObjectsLayer>();
                    transform.parent = objectsLayer.transform;
                    transform.localPosition = new Vector3(
                        X * objectsLayer.GetCellWidth(),
                        Y * objectsLayer.GetCellHeight(),
                        0
                    );
                }

                void OnDetached()
                {
                    parentMap = null;
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
                        throw new Exception("A map strategy holder is required when the map initializes.");
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