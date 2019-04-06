using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            using Entities.Objects;

            /// <summary>
            ///   Everything happens here. A map is essentially the place where movement and
            ///     interaction can occur.
            /// </summary>
            [ExecuteInEditMode]
            [RequireComponent(typeof(SortingGroup))]
            public class Map : MonoBehaviour
            {
                /// <summary>
                ///   This exception is deprecated. In the future, we should change this
                ///     exception (and <see cref="ExpectOneLayerComponent{T}(bool)"/>) to
                ///     the use of <see cref="DisallowMultipleComponent"/>.
                /// </summary>
                public class OneComponentIsNeeded : Types.Exception
                {
                    public OneComponentIsNeeded() { }
                    public OneComponentIsNeeded(string message) : base(message) { }
                    public OneComponentIsNeeded(string message, System.Exception inner) : base(message, inner) { }
                }

                /**
                 * Requires a component (being child of MapLayer). It may be optional or mandatory
                 *   but only one of that type will be allowed. It also fixes the size of the grids,
                 *   if any, and always resets the transform.
                 */
                private T ExpectOneLayerComponent<T>(bool require = false) where T : Layers.MapLayer
                {
                    T[] components = GetComponentsInChildren<T>();
                    if (require ? (components.Length != 1) : (components.Length > 1))
                    {
                        Destroy(gameObject);
                        throw new OneComponentIsNeeded(string.Format("One {0} component of type {1} is expected on this object", require ? "mandatory" : "optional", typeof(T).FullName));
                    }
                    else if (components.Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        T component = components[0];
                        Grid componentGrid = component.GetComponent<Grid>();
                        if (componentGrid != null)
                        {
                            componentGrid.cellGap = Vector3.zero;
                            componentGrid.cellSize = cellSize;
                        }
                        return component;
                    }
                }

                /// <summary>
                ///   The width of the map. It will be clamped to be between 1 and 100.
                /// </summary>
                [SerializeField]
                private uint width;

                /// <summary>
                ///   The height of the map. It will be clamped to be between 1 and 100.
                /// </summary>
                [SerializeField]
                private uint height;

                /// <summary>
                ///   The cell size. This value will be set to the underlying grid component.
                ///   By default, it will be (1, 1, 1) game units.
                /// </summary>
                [SerializeField]
                private Vector3 cellSize = Vector3.one;
                public Vector3 CellSize { get { return cellSize; } }

                /// <summary>
                ///   The map's floor layer. It will hold a lot of children of type
                ///     <see cref="Floors.Floor"/>. The user should give each child's
                ///     <see cref="Tilemap"/> component an appropriate value to their
                ///     <see cref="TilemapRenderer.sortOrder"/>.
                /// </summary>
                public Layers.Floor.FloorLayer FloorLayer { get; private set; }

                /// <summary>
                ///   The map's drop layer. This layer is optional, and will hold the
                ///     dropped objects (if implemented - there are several games that
                ///     do not make use of drop features).
                /// </summary>
                public Layers.Drop.DropLayer DropLayer { get; private set; }

                /// <summary>
                ///   The map's objects layer. This is where most of the interesting
                ///     things of your game will happen: movable, oriented, staying
                ///     and other types of objects will live in this layer.
                /// </summary>
                public Layers.Objects.ObjectsLayer ObjectsLayer { get; private set; }

                /// <summary>
                ///   The map's visuals layer. Visuals are the visible part of objects,
                ///     and will render at many level depths. They will have no logic,
                ///     but they will exist on top of objects layer.
                /// </summary>
                public Layers.Visuals.VisualsLayer VisualsLayer { get; private set; }

                /// <summary>
                ///   The ceilings layer will hold overlays floating that hide
                ///     everything else. Being of type <see cref="Ceilings.Ceiling"/>,
                ///     these overlays and also change their opacity to transparent
                ///     or translucent, so the player can see what is inside.
                /// </summary>
                public Layers.Ceiling.CeilingLayer CeilingLayer { get; private set; }

                private bool initialized = false;

                /// <summary>
                ///   See <see cref="height"/>.
                /// </summary>
                public uint Height { get { return height; } }

                /// <summary>
                ///   See <see cref="width"/>.
                /// </summary>
                public uint Width { get { return width; } }

                /// <summary>
                ///   Tells whether the map is initialized. No need to make use of
                ///     this property, but <see cref="Object"/> objects will.
                /// </summary>
                public bool Initialized { get { return initialized; } }

                /// <summary>
                ///   The objects strategy holder. It manages the rules under which the
                ///     objcts inside can perform movements.
                /// </summary>
                public ObjectsManagementStrategyHolder StrategyHolder { get; private set; }

                // Use this for initialization
                private void Awake()
                {
                    // Starting the dimensions
                    width = Values.Clamp(1, width, (uint)short.MaxValue);
                    height = Values.Clamp(1, height, (uint)short.MaxValue);
                    // Requiring the layers - at most one of each them may exist per map
                    FloorLayer = ExpectOneLayerComponent<Layers.Floor.FloorLayer>(true);
                    DropLayer = ExpectOneLayerComponent<Layers.Drop.DropLayer>();
                    ObjectsLayer = ExpectOneLayerComponent<Layers.Objects.ObjectsLayer>(true);
                    VisualsLayer = ExpectOneLayerComponent<Layers.Visuals.VisualsLayer>(true);
                    CeilingLayer = ExpectOneLayerComponent<Layers.Ceiling.CeilingLayer>();
                    Grid floorLayerGrid = FloorLayer.GetComponent<Grid>();
                    CopyGridProperties(ObjectsLayer.GetComponent<Grid>(), floorLayerGrid);
                    if (CeilingLayer != null) CopyGridProperties(CeilingLayer.GetComponent<Grid>(), floorLayerGrid);
                    // Fetching strategy - needed
                    StrategyHolder = GetComponent<ObjectsManagementStrategyHolder>();
                }

                void CopyGridProperties(Grid dst, Grid src)
                {
                    dst.cellSize = src.cellSize;
                    dst.cellLayout = src.cellLayout;
                    dst.cellGap = src.cellGap;
                    dst.cellSwizzle = src.cellSwizzle;
                }

                private void Start()
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying) return;
#endif

                    // Initializing strategy
                    if (StrategyHolder == null)
                    {
                        throw new Types.Exception("An objects management strategy holder is required when the map initializes.");
                    }
                    else
                    {
                        StrategyHolder.Initialize();
                    }
                    // TODO This line should be removed, and its behaviour moved to a particular
                    // TODO   subclass of (map-) Strategy upon initialization.
                    // InitBlockedPositions();
                    // We consider this map as initialized after its strategy started.
                    initialized = true;
                    // Now, it is turn of the already-in-place map objects to initialize.
                    foreach (Object mapObject in GetComponentsInChildren<Object>())
                    {
                        mapObject.Initialize();
                    }
                }

                /// <summary>
                ///   Attaches an object to this map.
                /// </summary>
                /// <param name="mapObject">The object to attach</param>
                /// <param name="x">The new X position</param>
                /// <param name="y">The new Y position</param>
                public void Attach(Object mapObject, uint x, uint y)
                {
                    if (initialized) StrategyHolder.Attach(mapObject.StrategyHolder, x, y);
                }

                /// <summary>
                ///   Pauses the map. Actually, pauses all the objects inside the map.
                /// </summary>
                /// <param name="fullFreeze">If true, it also pauses objects' animations</param>
                public void Pause(bool fullFreeze)
                {
                    foreach (Entities.Common.Pausable p in GetComponentsInChildren<Entities.Common.Pausable>(true))
                    {
                        p.Pause(fullFreeze);
                    }
                }

                /// <summary>
                ///   Resumes the map. Actually, resumes all the objects inside the map.
                /// </summary>
                public void Resume()
                {
                    foreach (Entities.Common.Pausable p in GetComponentsInChildren<Entities.Common.Pausable>(true))
                    {
                        p.Resume();
                    }
                }

#if UNITY_EDITOR
                // Normalizes all the tilemaps, grids, and layers.
                private void NormalizeTilemapsAndGrids()
                {
                    foreach (Transform child in transform)
                    {
                        Grid grid = transform.GetComponent<Grid>();
                        child.localScale = Vector2.one;
                        child.localPosition = Vector2.zero;
                        child.localRotation = Quaternion.identity;

                        if (grid)
                        {
                            foreach (Transform grandchild in child)
                            {
                                grandchild.localScale = Vector2.one;
                                grandchild.localPosition = Vector2.zero;
                                grandchild.localRotation = Quaternion.identity;
                            }
                        }
                    }
                }

                // Normalizes grid properties in ceilings, from floors, which in turn
                //   comes from cellSize.
                private void NormalizeCeilingWithFloor()
                {
                    Layers.Floor.FloorLayer floorLayer = ExpectOneLayerComponent<Layers.Floor.FloorLayer>();
                    Layers.Ceiling.CeilingLayer ceilingLayer = ExpectOneLayerComponent<Layers.Ceiling.CeilingLayer>();
                    if (floorLayer)
                    {
                        Grid floorGrid = floorLayer.GetComponent<Grid>();
                        if (floorGrid)
                        {
                            floorGrid.cellSize = cellSize;
                            if (ceilingLayer)
                            {
                                Grid ceilingGrid = ceilingLayer.GetComponent<Grid>();
                                if (ceilingLayer) CopyGridProperties(ceilingGrid, floorGrid);
                            }
                        }
                    }
                }

                // Ensures the highlight layer being present. The highlight by itself will
                //   know how to update, and to remove on application run.
                private Layers.Highlight.HighlightLayer EnsureBackgroundHighlight()
                {
                    Layers.Highlight.HighlightLayer highlightLayer = null;
                    foreach(Transform child in transform)
                    {
                        highlightLayer = child.GetComponent<Layers.Highlight.HighlightLayer>();
                        if (highlightLayer) {
                            highlightLayer.ForceUpdate();
                            return highlightLayer;
                        };
                    }

                    GameObject highlightObject = new GameObject("Highlight");
                    highlightObject.SetActive(false);
                    highlightLayer = highlightObject.AddComponent<Layers.Highlight.HighlightLayer>();
                    highlightObject.transform.parent = transform;
                    highlightObject.SetActive(true);
                    highlightLayer.ForceUpdate();
                    return highlightLayer;
                }

                private void Update()
                {
                    if (!Application.isPlaying)
                    {
                        NormalizeTilemapsAndGrids();
                        NormalizeCeilingWithFloor();
                        EnsureBackgroundHighlight();
                    }
                }
#endif
            }
        }
    }
}