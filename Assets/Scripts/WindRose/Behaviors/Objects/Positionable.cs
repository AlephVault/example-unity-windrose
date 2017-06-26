using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        using Types;
        using Types.Tilemaps;

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
                parentMap = Utils.Layout.RequireComponentInParent<Map>(this);
                tilemapObject = new Tilemap.TilemapObject(this, initialX, initialY, width, height, initialSolidness);
                tilemapObject.Attach(parentMap.InternalTilemap);
            }

            void OnDestroy()
            {
                tilemapObject.Detach();
            }

            public void Teleport(uint? x, uint? y)
            {
                tilemapObject.Teleport(x, y);
            }

            public void SetSolidness(SolidnessStatus newSolidness)
            {
                tilemapObject.SetSolidness(newSolidness);
            }

            public bool StartMovement(Direction movementDirection)
            {
                return tilemapObject.StartMovement(movementDirection);
            }

            public bool FinishMovement()
            {
                return tilemapObject.FinishMovement();
            }

            public bool CancelMovement()
            {
                return tilemapObject.CancelMovement();
            }
        }
    }
}