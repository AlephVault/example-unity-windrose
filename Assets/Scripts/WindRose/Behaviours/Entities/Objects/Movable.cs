using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   <para>
            ///     Moving components exist on regular objects also having a
            ///       <see cref="StatePicker"/> component. These components
            ///       add a "moving" state they will interact with. To do so,
            ///       they also have the means to start and cancel a movement
            ///       on any direction.
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(StatePicker))]
            public class Movable : MonoBehaviour
            {
                /// <summary>
                ///   The default state key provided by Movable components.
                /// </summary>
                public const string MOVING_STATE = "moving";

                // Dependencies
                private StatePicker statePicker;
                private Object mapObject;

                // Origin and target of movement. This has to do with the min/max values
                //   of Snapped, but specified for the intended movement.
                private Vector2 origin = Vector2.zero, target = Vector2.zero;

                /// <summary>
                ///   The movement speed, in game units per second.
                /// </summary>
                public uint speed = 2;

                // A runtime check to determine whether the object was moving in the previous frame
                private bool wasMoving = false;

                // This member hold the last movement being commanded to this object
                private Types.Direction? CommandedMovement = null;

                /// <summary>
                ///   Tells whether the object is moving. It knows that by reading the
                ///     current movement in the underlying map object.
                /// </summary>
                public bool IsMoving { get { return mapObject.Movement != null; } }

                /// <summary>
                ///   Gets the current movement in the underlying map object.
                /// </summary>
                public Types.Direction? Movement { get { return mapObject.Movement; } }

                /// <summary>
                ///   Sets the current state to the movement state registered
                ///     in this component.
                /// </summary>
                public void SetMovingState()
                {
                    statePicker.SelectedKey = MOVING_STATE;
                }

                /// <summary>
                ///   Starts a movement in certain direction.
                /// </summary>
                /// <param name="movement">The direction of the new movement</param>
                /// <param name="queueIfMoving">
                ///   If <c>true</c>, this movement is "stored" and will execute automatically
                ///     after the current movement ends.
                /// </param>
                /// <returns>Whether the movement could be started</returns>
                public bool StartMovement(Types.Direction movement, bool queueIfMoving = true)
                {
                    if (mapObject.ParentMap == null) return false;

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
                        return mapObject.StartMovement(movement);
                    }
                }

                /// <summary>
                ///   Cancels the current movement, if any.
                /// </summary>
                public void CancelMovement()
                {
                    if (mapObject.ParentMap == null) return;

                    mapObject.CancelMovement();
                }

                private Vector2 VectorForCurrentDirection()
                {
                    switch (Movement)
                    {
                        case Types.Direction.UP:
                            return Vector2.up * mapObject.GetCellHeight();
                        case Types.Direction.DOWN:
                            return Vector2.down * mapObject.GetCellHeight();
                        case Types.Direction.LEFT:
                            return Vector2.left * mapObject.GetCellWidth();
                        case Types.Direction.RIGHT:
                            return Vector2.right * mapObject.GetCellWidth();
                    }
                    // This one is never reached!
                    return Vector2.zero;
                }

                void Awake()
                {
                    statePicker = GetComponent<StatePicker>();
                    mapObject = GetComponent<Object>();
                    mapObject.onAttached.AddListener(delegate (World.Map parentMap)
                    {
                        // Avoid inheriting former value of origin.
                        // If a movement is being performed, then
                        //   just set a new value to origin.
                        origin = transform.localPosition;
                        wasMoving = false;
                        enabled = true;
                    });
                    mapObject.onDetached.AddListener(delegate ()
                    {
                        enabled = false;
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the map object. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Updates the current in-map position on the object, reflecting
                ///       partial movement on each frame.
                ///   </para>
                /// </summary>
                public void DoUpdate()
                {
                    if (mapObject.ParentMap == null) return;

                    if (IsMoving)
                    {
                        Vector2 vector = VectorForCurrentDirection();
                        Vector2 targetOffset = vector;

                        // The object has to perform movement.
                        // Initially, we must set the appropriate target.
                        if (!wasMoving)
                        {
                            origin = transform.localPosition;
                            target = origin + targetOffset;
                            SetMovingState();
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
                                mapObject.FinishMovement();
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
                             * 1. We calculate our movement with a target position of (target) + (a vector with norm of [movement offset by current speed and timedelta] + 1).
                             *    This is intended to avoid movement clamping against the target due to high speeds.
                             * 2a. If the distance between this target position and the origin is less than the distance between the target and the origin,
                             *     we just increment the position.
                             * 2c. Otherwise (LOOPING) we ALSO mark the movement as completed, and start a new one (adapting origin and target)
                             */
                            Vector2 movementDestination = target + vector * (1 + movementNorm);
                            Vector2 movement = Vector2.MoveTowards(transform.localPosition, movementDestination, movementNorm);
                            // Adjusting the position as usual
                            transform.localPosition = new Vector3(movement.x, movement.y, transform.localPosition.z);
                            while (true)
                            {
                                float traversedDistanceSinceOrigin = (movement - origin).magnitude;

                                // We break this loop if the delta is lower than cell dimension because we
                                //   do not need to mark new movements anymore.
                                if (traversedDistanceSinceOrigin < vector.magnitude) break;

                                // We intend to at least finish this movement and perhaps continue with a new one
                                Types.Direction currentMovement = Movement.Value;
                                mapObject.FinishMovement();
                                if (traversedDistanceSinceOrigin > vector.magnitude)
                                {
                                    origin = target;
                                    target = target + targetOffset;
                                    // If the movement cannot be performed, we break this loop
                                    //   and also clamp the movement to the actual box, so we
                                    //   avoid "bounces".
                                    if (!mapObject.StartMovement(currentMovement))
                                    {
                                        transform.localPosition = new Vector3(origin.x, origin.y, transform.localPosition.z);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (CommandedMovement != null)
                    {
                        mapObject.StartMovement(CommandedMovement.Value);
                    }
                    else
                    {
                        statePicker.SelectedKey = "";
                    }

                    wasMoving = IsMoving;
                    // We clean up the last commanded movement, so future frames
                    //   do not interpret this command as a must, since it expired.
                    CommandedMovement = null;
                }
            }
        }
    }
}