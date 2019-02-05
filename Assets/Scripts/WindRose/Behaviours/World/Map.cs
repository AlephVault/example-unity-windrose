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
            using Objects;

            /// <summary>
            ///   Everything happens here. A map is essentially the place where movement and
            ///     interaction can occur.
            /// </summary>
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
                ///     this property, but <see cref="Positionable"/> objects will.
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
                    width = Values.Clamp(1, width, 100);
                    height = Values.Clamp(1, height, 100);
                    // Requiring the layers - at most one of each them may exist per map
                    FloorLayer = ExpectOneLayerComponent<Layers.Floor.FloorLayer>(true);
                    DropLayer = ExpectOneLayerComponent<Layers.Drop.DropLayer>();
                    ObjectsLayer = ExpectOneLayerComponent<Layers.Objects.ObjectsLayer>(true);
                    CeilingLayer = ExpectOneLayerComponent<Layers.Ceiling.CeilingLayer>();
                    // Fetching strategy - needed
                    StrategyHolder = GetComponent<ObjectsManagementStrategyHolder>();
                }

                private void Start()
                {
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
                    // Now, it is turn of the already-in-place positionables to initialize.
                    foreach (Positionable positionable in GetComponentsInChildren<Positionable>())
                    {
                        positionable.Initialize();
                    }
                }

                /// <summary>
                ///   Attaches an object to this map.
                /// </summary>
                /// <param name="positionable">The object to attach</param>
                /// <param name="x">The new X position</param>
                /// <param name="y">The new Y position</param>
                public void Attach(Positionable positionable, uint x, uint y)
                {
                    if (initialized) StrategyHolder.Attach(positionable.StrategyHolder, x, y);
                }

                /// <summary>
                ///   Pauses the map. Actually, pauses all the objects inside the map.
                /// </summary>
                /// <param name="fullFreeze">If true, it also pauses objects' animations</param>
                public void Pause(bool fullFreeze)
                {
                    foreach (Pausable p in GetComponentsInChildren<Pausable>(true))
                    {
                        p.Pause(fullFreeze);
                    }
                }

                /// <summary>
                ///   Resumes the map. Actually, resumes all the objects inside the map.
                /// </summary>
                public void Resume()
                {
                    foreach (Pausable p in GetComponentsInChildren<Pausable>(true))
                    {
                        p.Resume();
                    }
                }
            }
        }
    }
}