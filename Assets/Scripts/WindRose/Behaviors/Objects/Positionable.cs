using UnityEngine;
using Support.Utils;

namespace WindRose
{
    namespace Behaviours
    {
        using Types;
        using Types.Tilemaps;

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
            public uint Width { get { return tilemapObject.Width; } }
            public uint Height { get { return tilemapObject.Height; } }
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

            void OnDestroy()
            {
                tilemapObject.Detach();
            }

            public void Initialize()
            {
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
                return tilemapObject != null  && !paused && tilemapObject.CancelMovement();
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