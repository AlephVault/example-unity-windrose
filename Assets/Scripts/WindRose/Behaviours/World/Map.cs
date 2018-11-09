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

            /**
             * A map contains its dimensions and internal layers.
             * The dimensions are used on its strategy.
             */
            [RequireComponent(typeof(SortingGroup))]
            public class Map : MonoBehaviour
            {
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

                [SerializeField]
                private uint width;

                [SerializeField]
                private uint height;

                [SerializeField]
                private Vector3 cellSize = Vector3.one;

                public Layers.FloorLayer FloorLayer { get; private set; }
                public Layers.DropLayer DropLayer { get; private set; }
                public Layers.ObjectsLayer ObjectsLayer { get; private set; }
                public Layers.CeilingLayer CeilingLayer { get; private set; }

                private bool initialized = false;

                public uint Height { get { return height; } }
                public uint Width { get { return width; } }
                public bool Initialized { get { return initialized; } }
                public ObjectsManagementStrategyHolder StrategyHolder { get; private set; }

                // Use this for initialization
                private void Awake()
                {
                    // Starting the dimensions
                    width = Values.Clamp(1, width, 100);
                    height = Values.Clamp(1, height, 100);
                    // Requiring the layers - at most one of each them may exist per map
                    FloorLayer = ExpectOneLayerComponent<Layers.FloorLayer>(true);
                    DropLayer = ExpectOneLayerComponent<Layers.DropLayer>();
                    ObjectsLayer = ExpectOneLayerComponent<Layers.ObjectsLayer>(true);
                    CeilingLayer = ExpectOneLayerComponent<Layers.CeilingLayer>();
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

                public void Pause(bool fullFreeze)
                {
                    foreach (Pausable p in GetComponentsInChildren<Pausable>(true))
                    {
                        p.Pause(fullFreeze);
                    }
                }

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