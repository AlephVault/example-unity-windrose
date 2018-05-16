using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(BoxCollider2D))]
        public class TriggerVisionRange : TriggerZone
        {
            /**
             * A TriggerVisionRange ties to a strict WindRise component, and so will also correctly
             *   compute its position. Also, it will have a Box-type collision mask.
             * 
             * However, despite being a box collider as well, its purpose will be complementary to
             *   TriggerActivator.
             */

            // This inner margin is not mutable and will work to avoid bleeding
            const float BLEEDING_BUFFER = 0.1f;

            // This related component will be used to tie the events of attach/detach to it, and to
            //   calculate the offsets. It is different to the Platform, in the way that the Platform
            //   has the component by itself.
            [SerializeField]
            private EventDispatcher relatedEventDispatcher;

            // Perhaps the related object has an Oriented component. We will make use of it.
            private Oriented oriented;

            // We will make use of this field as a fixed value if the related Positionable object
            //   does not have an Oriented component to take the actual and current direction from.
            [SerializeField]
            private Types.Direction direction = Types.Direction.DOWN;

            // Size corresponds to half-width, rounded down.
            // e.g. 0 corresponds to 1-cell width, 1 corresponds to 3-cell width, 3 to 5, ...
            [SerializeField]
            private uint visionSize = 0;

            // Length corresponds to how far does the vision reach. The actual length will be this
            //   value, plus 1 (the immediately next step is not counted as part of the vision
            //   range).
            [SerializeField]
            private uint visionLength = 0;

            private uint halfWidth;
            private uint halfHeight;

            protected override void Awake()
            {
                base.Awake();
                oriented = positionable.GetComponent<Oriented>();
            }

            protected override void Start()
            {
                base.Start();
                if (positionable.Width % 2 == 0 || positionable.Height % 2 == 0)
                {
                    throw new Types.Exception("For a vision range to work appropriately, the related positionable must have an odd width and height");
                }
                halfHeight = positionable.Height / 2;
                halfWidth = positionable.Width / 2;
                // Forcing accurate position the first time
                Update();
            }

            protected override void Update()
            {
                if (oriented != null) direction = oriented.orientation;
                RefreshDimensions();
                base.Update();
            }

            protected override int GetDeltaX()
            {
                int extra;
                switch(direction)
                {
                    case Types.Direction.RIGHT:
                        extra = (int)positionable.Width;
                        break;
                    case Types.Direction.UP:
                    case Types.Direction.DOWN:
                        extra = (int)halfWidth;
                        break;
                    default:
                        extra = 0;
                        break;
                }
                return extra + (int)positionable.X;
            }

            protected override int GetDeltaY()
            {
                int extra;
                switch (direction)
                {
                    case Types.Direction.DOWN:
                        extra = (int)positionable.Height;
                        break;
                    case Types.Direction.LEFT:
                    case Types.Direction.RIGHT:
                        extra = (int)halfHeight;
                        break;
                    default:
                        extra = 0;
                        break;
                }
                return extra + (int)positionable.Y;
            }

            protected override EventDispatcher GetRelatedEventDispatcher()
            {
                return relatedEventDispatcher;
            }

            protected override Collider2D GetCollider2D()
            {
                return GetComponent<BoxCollider2D>();
            }

            protected override void SetupCollider(Collider2D collider2D)
            {
                BoxCollider2D boxCollider2D = (BoxCollider2D)collider2D;
                float cellWidth = positionable.GetCellWidth();
                float cellHeight = positionable.GetCellHeight();
                // we set the size based on the direction we are looking, and also the offset back to the right-top corner
                switch (direction)
                {
                    case Types.Direction.UP:
                    case Types.Direction.DOWN:
                        boxCollider2D.size = new Vector2((visionSize * 2 + 1) * cellWidth, (visionLength + 1) * cellHeight);
                        break;
                    default:
                        boxCollider2D.size = new Vector2((visionLength + 1) * cellWidth, (visionSize * 2 + 1) * cellHeight);
                        break;
                }
                boxCollider2D.offset = new Vector2(0.5f * boxCollider2D.size.x, 0.5f * boxCollider2D.size.y);
                // also we set the transform of this vision range, using global coordinates:
                Vector3 basePosition = positionable.transform.position;
                Vector3 newPosition = Vector3.zero;
                switch(direction)
                {
                    case Types.Direction.UP:
                        newPosition = new Vector3(basePosition.x - visionSize * cellWidth, basePosition.y + boxCollider2D.size.y, basePosition.z);
                        break;
                    case Types.Direction.DOWN:
                        newPosition = new Vector3(basePosition.x - visionSize * cellWidth, basePosition.y - positionable.Height * cellHeight, basePosition.z);
                        break;
                    case Types.Direction.LEFT:
                        newPosition = new Vector3(basePosition.x - boxCollider2D.size.x, basePosition.y + visionSize * cellHeight, basePosition.z);
                        break;
                    case Types.Direction.RIGHT:
                        newPosition = new Vector3(basePosition.x + positionable.Width * cellWidth, basePosition.y + visionSize * cellHeight, basePosition.z);
                        break;
                    default:
                        break;
                }
                transform.position = newPosition;
                // We apply the bleeding buffer right here
                boxCollider2D.size = boxCollider2D.size - 2 * new Vector2(BLEEDING_BUFFER * cellWidth, BLEEDING_BUFFER * cellHeight);
            }
        }
    }
}