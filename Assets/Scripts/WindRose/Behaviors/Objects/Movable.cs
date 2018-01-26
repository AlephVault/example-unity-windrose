using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Oriented))]
        public class Movable : MonoBehaviour
        {
            public const string MOVE_ANIMATION = "move";

            // Dependencies
            private Oriented oriented;
            private Positionable positionable;

            // Origin and target of movement. This has to do with the min/max values
            //   of Snapped, but specified for the intended movement.
            private Vector2 origin = Vector2.zero, target = Vector2.zero;

            // These fields are the configurable features of this behavior
            [SerializeField]
            private Types.AnimationSet movingAnimationSet;
            public uint speed = 2; // The speed is expressed in terms of units per second

            // A runtime check to determine whether the object was moving in the previous frame
            private bool wasMoving = false;

            // A runtime check to determine whether the object is being moved
            public bool IsMoving { get { return positionable.Movement != null; } }

            // Perhaps we want to override the animation being used as moving,
            //   with a new one. It is intended to serve as a "temporary" moving
            //   animation for any reason.
            [HideInInspector]
            public string overriddenKeyForMovingAnimation = null;

            public void SetMovingAnimation()
            {
                string newKey = (overriddenKeyForMovingAnimation == null) ? MOVE_ANIMATION : overriddenKeyForMovingAnimation;
                oriented.animationKey = newKey;
            }

            private Vector2 OffsetForCurrentDirection()
            {
                switch(positionable.Movement)
                {
                    case Types.Direction.UP:
                        return Vector2.up * Snapped.GAME_UNITS_PER_TILE_UNITS;
                    case Types.Direction.DOWN:
                        return Vector2.down * Snapped.GAME_UNITS_PER_TILE_UNITS;
                    case Types.Direction.LEFT:
                        return Vector2.left * Snapped.GAME_UNITS_PER_TILE_UNITS;
                    case Types.Direction.RIGHT:
                        return Vector2.right * Snapped.GAME_UNITS_PER_TILE_UNITS;
                }
                // This one is never reached!
                return Vector2.zero;
            }

            void Awake()
            {
                // I DON'T KNOW WHY HIDDEN PROPERTIES FROM INSPECTOR ALSO AVOID NULL VALUES.
                // So I'm adding this code to ensure this particular field starts as null in Start().
                overriddenKeyForMovingAnimation = null;
            }

            // Use this for initialization
            void Start()
            {
                oriented = GetComponent<Oriented>();
                positionable = GetComponent<Positionable>();
                oriented.AddAnimationSet(MOVE_ANIMATION, movingAnimationSet);
            }

            // Update is called once per frame
            void Update()
            {
                if (positionable.ParentMap == null) return;

                if (IsMoving)
                {
                    // The object has to perform movement.
                    // Initially, we must set the appropriate target.
                    if (!wasMoving)
                    {
                        origin = transform.localPosition;
                        target = origin + OffsetForCurrentDirection();
                        SetMovingAnimation();
                    }
                    // Now we move towards the target at a speed of (speed) units per second
                    Vector2 movement = Vector2.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
                    if ((Vector2) transform.localPosition == movement)
                    {
                        // If the movement and the localPosition (converted to 2D vector) are the same,
                        //   we mark the movement as finished.
                        positionable.FinishMovement();
                    }
                    else
                    {
                        // Otherwise we adjust the localPosition to the intermediate step.
                        transform.localPosition = new Vector3(movement.x, movement.y, transform.localPosition.z);
                    }
                }
                else
                {
                    oriented.SetIdleAnimation();
                }
                wasMoving = IsMoving;
            }

            void Pause(bool fullFreeze)
            {
                enabled = false;
            }

            void Resume()
            {
                enabled = true;
            }
        }
    }
}