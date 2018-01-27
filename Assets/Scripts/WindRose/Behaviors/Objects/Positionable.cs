using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Types;
        using Types.Tilemaps;

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

            [SerializeField]
            private uint initialX = 0;

            [SerializeField]
            private uint initialY = 0;

            [SerializeField]
            private SolidnessStatus initialSolidness = SolidnessStatus.Solid;

            /* *********************** Additional data *********************** */

            private Map parentMap = null;
            private Tilemap.TilemapObject tilemapObject = null;
            private bool paused = false;

            /* *********************** Public properties *********************** */

            public Map ParentMap { get { return parentMap; } }
            public uint Width { get { return width; } } // Referencing directly allows us to query the width without a map assigned yet.
            public uint Height { get { return width; } } // Referencing directly allows us to query the height without a map assigned yet.
            public uint X { get { return tilemapObject.X; } }
            public uint Y { get { return tilemapObject.Y; } }
            public uint Xf { get { return tilemapObject.Xf; } }
            public uint Yf { get { return tilemapObject.Yf; } }
            public Direction? Movement { get { return tilemapObject.Movement; } }
            public SolidnessStatus Solidness { get { return tilemapObject.Solidness; } }
            
            void Start()
            {
                Initialize();
            }

            #if UNITY_EDITOR
            private void Update()
            {
                if (!Application.isPlaying)
                {
                    transform.localPosition = new Vector3(initialX * Map.GAME_UNITS_PER_TILE_UNITS, - (int)initialY * Map.GAME_UNITS_PER_TILE_UNITS, transform.localPosition.z);
                }
            }
            #endif

            void OnDestroy()
            {
                Detach();
            }

            void OnAttached(object[] args)
            {
                parentMap = ((Tilemap)(args[0])).RelatedMap;
            }

            void OnDetached()
            {
                parentMap = null;
            }

            public void Initialize()
            {
                if (!Application.isPlaying) return;

                if (tilemapObject != null)
                {
                    return;
                }

                try
                {
                    // perhaps it will not be added now because the Map component is not yet initialized! (e.g. this method being called from Start())
                    // however, when the Map becomes ready, this method will be called, again, by the map itself, which will exist.
                    parentMap = Layout.RequireComponentInParent<Map>(this);
                    tilemapObject = new Tilemap.TilemapObject(this, initialX, initialY, width, height, initialSolidness);
                    tilemapObject.Attach(parentMap.InternalTilemap);
                }
                catch (Layout.MissingComponentInParentException)
                {
                    // nothing - diaper
                }
            }

            public void Detach()
            {
                // There are some times at startup when the Tilemap object may be null.
                // That's why we run the conditional.
                //
                // For the general cases, Detach will find a tilemapObject attached.
                if (tilemapObject != null) tilemapObject.Detach();
            }

            public void Attach(Map map, uint? x = null, uint? y = null, bool force = false)
            {
                if (force) Detach();
                tilemapObject.Attach(map != null ? map.InternalTilemap : null, x, y);
            }

            public void Teleport(uint? x, uint? y)
            {
                if (tilemapObject != null && !paused) tilemapObject.Teleport(x, y);
            }

            public void SetSolidness(SolidnessStatus newSolidness)
            {
                if (tilemapObject != null && !paused) tilemapObject.SetSolidness(newSolidness);
            }

            public bool StartMovement(Direction movementDirection)
            {
                return tilemapObject != null && !paused && tilemapObject.StartMovement(movementDirection);
            }

            public bool FinishMovement()
            {
                return tilemapObject != null && !paused && tilemapObject.FinishMovement();
            }

            public bool CancelMovement()
            {
                return tilemapObject != null && !paused && tilemapObject.CancelMovement();
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