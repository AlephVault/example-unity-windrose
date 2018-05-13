using UnityEngine;
using UnityEngine.Tilemaps;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Types.States;

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

            private MapState internalMapState;
            private Grid grid;
            private bool initialized = false;

            public MapState InternalMapState { get { return internalMapState; } }
            public uint Height { get { return height; } }
            public uint Width { get { return width; } }
            public bool Initialized { get { return initialized; } }

            // Use this for initialization
            private void Awake()
            {
                width = Values.Clamp(1, width, 100);
                height = Values.Clamp(1, height, 100);
                internalMapState = new MapState(this, Width, Height);
            }

            private void Start()
            {
                grid = GetComponent<Grid>();
                InitBlockedPositions();
                initialized = true;
                foreach (Positionable positionable in GetComponentsInChildren<Positionable>())
                {
                    positionable.Initialize();
                }
            }

            private void InitBlockedPositions()
            {
                int childCount = transform.childCount;
                for(int index = 0; index < childCount; index++)
                {
                    GameObject go = transform.GetChild(index).gameObject;
                    Tilemap tilemap = go.GetComponent<Tilemap>();
                    if (tilemap != null) InitBlockedPositionsFromTilemap(tilemap);
                }
            }

            private void InitBlockedPositionsFromTilemap(Tilemap tilemap)
            {
                for(int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, -y-1, 0));
                        if (tile is Types.Tilemaps.IBlockingAwareTile)
                        {
                            // Why y-1? Because tiles "grow" up-right. So if I intend to pick the corner expanding from a point (x, -y)
                            //   considering its pivot in top-left, I will actually be picking (x, -(y+1)), hence the correction factor
                            //   of -1 to reference the proper index.
                            internalMapState.SetBlocking((uint)x, (uint)y, ((Types.Tilemaps.IBlockingAwareTile)tile).Blocks());
                        }
                    }
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