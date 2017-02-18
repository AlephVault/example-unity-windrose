using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        [RequireComponent(typeof(Positionable))]
        public class Snapped : MonoBehaviour
        {
            /**
             * A snapped object provides behavior to react to its position
             *   (since it will also be a positionable object), like:
             *   1. Checking whether the object should adjust its snap in X, Y, or both.
             *      This involves taking the X and Y position from the `positionable` properties
             *        and multiplying them by the tile dimensions to put them on screen.
             *   2. Checking whether the object should clamp its position in X, Y, or both.
             */

            private Positionable positionable;

            public bool SnapInX = true;
            public bool SnapInY = true;

            public bool ClampInX = false;
            public bool ClampInY = false;
            public float MinX = 0;
            public float MaxX = 0;
            public float MinY = 0;
            public float MaxY = 0;

            // Use this for initialization
            void Start()
            {
                positionable = GetComponent<Positionable>();
            }

            // Update is called once per frame
            void Update()
            {
                if (SnapInX || SnapInY)
                {
                    transform.position = new Vector3(
                        SnapInX ? positionable.X * positionable.ObjectLayer.TileWidth : transform.position.x,
                        SnapInY ? positionable.Y * positionable.ObjectLayer.TileHeight : transform.position.y,
                        transform.position.z
                    );
                }

                if (ClampInX || ClampInY)
                {
                    transform.position = new Vector3(
                        ClampInX ? Utils.Values.Clamp<float>(MinX, transform.position.x, MinY) : transform.position.x,
                        ClampInY ? Utils.Values.Clamp<float>(MinY, transform.position.y, MinY) : transform.position.y,
                        transform.position.z
                    );
                }
            }
        }
    }
}