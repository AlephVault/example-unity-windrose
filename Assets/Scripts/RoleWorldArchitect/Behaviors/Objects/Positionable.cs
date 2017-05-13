using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        using Types.ObjectLayerHelpers;

        public class Positionable : MonoBehaviour
        {
            /**
             * A positionable object updates its position and solidness status
             *   to its holding layer. It has nothing to do with the actual
             *   sprite position on the screen (there will be other behaviors
             *   to accomplish that), but with the tiling system.
             */

            private ObjectLayer objectLayer;

            private uint previousX;
            private uint previousY;
            private SolidnessStatus previousStatus = SolidnessStatus.Ghost;

            [SerializeField]
            private uint width;

            [SerializeField]
            private uint height;

            public uint X;
            public uint Y;
            public SolidnessStatus Status = SolidnessStatus.Ghost;

            public bool IsGhost { get { return Status == SolidnessStatus.Ghost; } }
            public bool IsSolidForOthers { get { return Status == SolidnessStatus.SolidForOthers; } }
            public bool IsSolid { get { return Status == SolidnessStatus.Solid; } }
            public uint PreviousX { get { return previousX; } }
            public uint PreviousY { get { return previousY; } }
            public SolidnessStatus PreviousStatus { get { return previousStatus; } }
            public ObjectLayer ObjectLayer { get { return objectLayer; } }
            public uint Xf { get { return X + width - 1; } }
            public uint Yf { get { return Y + height - 1; } }

            void Start()
            {
                objectLayer = Utils.Layout.RequireComponentInParent<ObjectLayer>(this);
                objectLayer.Put(this);
            }

            void Update()
            {
                X = Utils.Values.Clamp<uint>(0, X, ObjectLayer.MapWidth - width);
                Y = Utils.Values.Clamp<uint>(0, Y, ObjectLayer.MapHeight - height);

                if (X != previousX || Y != previousY || Status != previousStatus)
                {
                    previousX = X;
                    previousY = Y;
                    previousStatus = Status;
                    objectLayer.Put(this);
                }
            }

            void OnDestroy()
            {
                objectLayer.Remove(this);
            }
        }
    }
}