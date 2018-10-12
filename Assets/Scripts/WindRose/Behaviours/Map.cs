using UnityEngine;
using UnityEngine.Tilemaps;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Objects;

        [RequireComponent(typeof(Grid))]
        public class Map : MonoBehaviour
        {
            /**
             * A map manages its inner tilemaps and objects. It has few utilites beyond
             *   being a shortcut of Grid/Tilemaps.
             */

            [SerializeField]
            private uint width;

            [SerializeField]
            private uint height;

            private Grid grid;
            private bool initialized = false;

            public uint Height { get { return height; } }
            public uint Width { get { return width; } }
            public bool Initialized { get { return initialized; } }
            public Strategies.StrategyHolder StrategyHolder { get; private set; }

            // Use this for initialization
            private void Awake()
            {
                grid = GetComponent<Grid>();
                width = Values.Clamp(1, width, 100);
                height = Values.Clamp(1, height, 100);
                // Fetching strategy - needed
                StrategyHolder = GetComponent<Strategies.StrategyHolder>();
            }

            private void Start()
            {
                // Initializing tilemap positions
                foreach (Tilemap tilemap in GetComponentsInChildren<Tilemap>())
                {
                    tilemap.transform.localPosition = Vector3.zero;
                }
                // Initializing strategy
                if (StrategyHolder == null)
                {
                    throw new Types.Exception("A map strategy holder is required when the map initializes.");
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

            public float GetCellWidth()
            {
                return grid.cellSize.x;
            }

            public float GetCellHeight()
            {
                return grid.cellSize.y;
            }

            public Vector3Int WorldToCell(Vector3 position)
            {
                return grid.WorldToCell(position);
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