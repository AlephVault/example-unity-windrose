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

            // This member hold the last movement being commanded to this object
            private Types.Direction? CommandedMovement = null;

            // A runtime check to determine whether the object is being moved
            public bool IsMoving { get { return positionable.Movement != null; } }
            public Types.Direction? Movement { get { return positionable.Movement; } }

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

            public bool StartMovement(Types.Direction movement, bool queueIfMoving = true)
            {
                if (IsMoving)
                {
                    // The movement will not be performed now since there
                    //   is a movement in progess
                    if (queueIfMoving)
                    {
                        CommandedMovement = movement;
                    }
                    return false;
                }
                else
                {
                    return positionable.StartMovement(movement);
                }
            }

            public void CancelMovement()
            {
                positionable.CancelMovement();
            }

            private Vector2 VersorForCurrentDirection()
            {
                switch(Movement)
                {
                    case Types.Direction.UP:
                        return Vector2.up;
                    case Types.Direction.DOWN:
                        return Vector2.down;
                    case Types.Direction.LEFT:
                        return Vector2.left;
                    case Types.Direction.RIGHT:
                        return Vector2.right;
                }
                // This one is never reached!
                return Vector2.zero;
            }

            void Awake()
            {
                // I DON'T KNOW WHY HIDDEN PROPERTIES FROM INSPECTOR ALSO AVOID NULL VALUES.
                // So I'm adding this code to ensure this particular field starts as null in Start().
                overriddenKeyForMovingAnimation = null;
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
                    Vector2 versor = VersorForCurrentDirection();
                    Vector2 targetOffset = versor * Map.GAME_UNITS_PER_TILE_UNITS;

                    // The object has to perform movement.
                    // Initially, we must set the appropriate target.
                    if (!wasMoving)
                    {
                        origin = transform.localPosition;
                        target = origin + targetOffset;
                        SetMovingAnimation();
                    }
                    
                    // We calculate the movement offset
                    float movementNorm = speed * Time.deltaTime;
                    
                    /**
                     * Now the logic must go like this:
                     * - If no next movement is commanded, or a different-than-current movement is commanded,
                     *   we must move 'till target, as now we do, and at the given speed.
                     * - Otherwise (same movement being commanded) our logic will be extended:
                     *   *** see the inners in the ELSE branch for details ***
                     */                    
                    if (CommandedMovement != Movement)
                    {
                        Vector2 movement = Vector2.MoveTowards(transform.localPosition, target, movementNorm);
                        if ((Vector2)transform.localPosition == movement)
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
                        /**
                         * Inners will be more elaborated here:
                         * 1. We calculate our movement with a target position of (target) + (a versor with norm of [movement offset by current speed and timedelta] + 1).
                         *    This is intended to avoid movement clamping against the target due to high speeds.
                         * 2a. If the distance between this target position and the origin is less than the distance between the target and the origin,
                         *     we just increment the position.
                         * 2c. Otherwise we ALSO mark the movement as completed, and start a new one (adapting origin and target)
                         */
                        Vector2 movementDestination = target + versor * (1 + movementNorm);
                        Vector2 movement = Vector2.MoveTowards(transform.localPosition, movementDestination, movementNorm);
                        float traversedDistanceSinceOrigin = (movement - origin).magnitude;
                        // Adjusting the position as usual
                        transform.localPosition = new Vector3(movement.x, movement.y, transform.localPosition.z);
                        if (traversedDistanceSinceOrigin >= Map.GAME_UNITS_PER_TILE_UNITS)
                        {
                            // Continue with a new movement
                            Types.Direction currentMovement = Movement.Value;
                            positionable.FinishMovement();
                            if (traversedDistanceSinceOrigin > Map.GAME_UNITS_PER_TILE_UNITS)
                            {
                                origin = target;
                                target = target + targetOffset;
                                positionable.StartMovement(currentMovement);
                            }
                        }
                    }

                }
                else if (CommandedMovement != null)
                {
                    positionable.StartMovement(CommandedMovement.Value);
                }
                else
                {
                    oriented.SetIdleAnimation();
                }

                wasMoving = IsMoving;
                // We clean up the last commanded movement, so future frames
                //   do not interpret this command as a must, since it expired.
                CommandedMovement = null;
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