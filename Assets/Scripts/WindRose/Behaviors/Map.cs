using UnityEngine;
using UnityEngine.Tilemaps;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Types.States;

        public class Map : MonoBehaviour
        {
            /**
             * A map manages its inner tilemaps and objects. It has few utilites beyond
             *   being a shortcut of Grid/Tilemaps.
             */

            public const uint GAME_UNITS_PER_TILE_UNITS = 1;

            [SerializeField]
            private uint width;

            [SerializeField]
            private uint height;

            private MapState internalMapState;

            public MapState InternalMapState { get { return internalMapState; } }
            public uint Height { get { return height; } }
            public uint Width { get { return width; } }

            // Use this for initialization
            private void Awake()
            {
                width = Values.Clamp<uint>(1, width, 100);
                height = Values.Clamp<uint>(1, height, 100);
                internalMapState = new MapState(this, Width, Height);
            }

            private void Start()
            {
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
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile is Types.Tilemaps.IBlockingAwareTile)
                        {
                            internalMapState.SetBlocking((uint)x, (uint)y, ((Types.Tilemaps.IBlockingAwareTile)tile).Blocks());
                        }
                    }
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