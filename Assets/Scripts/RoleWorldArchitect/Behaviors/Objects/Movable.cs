using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Behaviors
    {
        [RequireComponent(typeof(Oriented))]
        [RequireComponent(typeof(Snapped))]
        [RequireComponent(typeof(Rigidbody2D))]
        public class Movable : MonoBehaviour
        {
            public const string MOVE_ANIMATION = "move";

            /**
             * Based on formerly defined behaviors, this one moves an object.
             *   It provides a walking animation set, which adds under the
             *   MOVE key, which will be added to the dictionary in the
             *   Oriented component.
             * 
             * Only a property will determine whether there is movement:
             *   
             *   bool active
             * 
             * While a property will determine its speed:
             * 
             *   uint speed
             * 
             * And an animation set will be provided for the movement:
             * 
             *   private AnimationSet movingAnimationSet
             *   
             * And will be forced to the Oriented component, when it is moving and
             *   this variable is false:
             *   
             *   bool forceMovingAnimation
             *   
             * However a readonly property will determine whether it is actually
             *   performing movement or not:
             *   
             *   bool IsMoving
             * 
             * It will rely on RigidBody2D to perform the movement with the given
             *   speed, and it will rely on Snapped to control how it will move.
             */

            private Oriented oriented;
            private Rigidbody2D rigidBody2d;
            private Snapped snapped;
            private Positionable positionable;

            private bool isMoving = false;
            private Vector2 origin = Vector2.zero, target = Vector2.zero;
            private Types.Direction currentDirection;

            [SerializeField]
            private Types.AnimationSet movingAnimationSet;

            public bool active = false;
            [HideInInspector]
            public bool forceMovingAnimation = true;
            public uint speed = 64;
            public bool IsMoving { get { return isMoving; } }
            // Perhaps we want to override the animation being used as moving,
            //   with a new one. It is intended to serve as a "temporary" moving
            //   animation for any reason.
            [HideInInspector]
            public string overriddenKeyForMovingAnimation = null;

            public void SetMovingAnimation()
            {
                oriented.animationKey = (overriddenKeyForMovingAnimation == null) ? MOVE_ANIMATION : overriddenKeyForMovingAnimation;
            }

            private Vector2 OffsetForCurrentDirection()
            {
                switch(currentDirection)
                {
                    case Types.Direction.UP:
                        return Vector2.up * positionable.ObjectLayer.TileHeight;
                    case Types.Direction.DOWN:
                        return Vector2.down * positionable.ObjectLayer.TileHeight;
                    case Types.Direction.LEFT:
                        return Vector2.left * positionable.ObjectLayer.TileWidth;
                    case Types.Direction.RIGHT:
                        return Vector2.right * positionable.ObjectLayer.TileWidth;
                }
                // This one is never reached!
                return Vector2.zero;
            }

            private Vector2 VelocityForCurrentDirection()
            {
                switch (currentDirection)
                {
                    case Types.Direction.UP:
                        return Vector2.up * speed;
                    case Types.Direction.DOWN:
                        return Vector2.down * speed;
                    case Types.Direction.LEFT:
                        return Vector2.left * speed;
                    case Types.Direction.RIGHT:
                        return Vector2.right * speed;
                }
                // This one is never reached!
                return Vector2.zero;
            }

            private bool IsBeyondTarget()
            {
                /**
                 * This method only makes sense if isMoving==true 
                 */
                switch(currentDirection)
                {
                    case Types.Direction.UP:
                        return transform.position.y >= target.y;
                    case Types.Direction.DOWN:
                        return transform.position.y <= target.y;
                    case Types.Direction.LEFT:
                        return transform.position.x <= target.x;
                    case Types.Direction.RIGHT:
                        return transform.position.x >= target.x;
                }
                return false;
            }

            // Use this for initialization
            void Start()
            {
                oriented = GetComponent<Oriented>();
                rigidBody2d = GetComponent<Rigidbody2D>();
                snapped = GetComponent<Snapped>();
                positionable = GetComponent<Positionable>();

                oriented.AddAnimationSet(MOVE_ANIMATION, movingAnimationSet);
            }

            // Update is called once per frame
            void Update()
            {
                if (!isMoving)
                {
                    if (!active)
                    {
                        snapped.SnapInX = true;
                        snapped.SnapInY = true;
                        snapped.ClampInX = false;
                        snapped.ClampInY = false;
                        oriented.SetIdleAnimation();
                        rigidBody2d.velocity = Vector2.zero;
                    }
                    else
                    {
                        /**
                         * 
                         * TODO: Abort if, for the next position, the object cannot occupy it!!!
                         *   Such a check considered false, the following 5 lines should not execute.
                         * 
                         * The check must involve querying the related ObjectLayer.
                         *
                         */
                        isMoving = true;
                        origin = new Vector2(transform.position.x, transform.position.y);
                        Types.Direction direction = oriented.direction;
                        currentDirection = direction;
                        target = origin + OffsetForCurrentDirection();
                    }
                }
                else
                {
                    if (IsBeyondTarget())
                    {
                        isMoving = active;
                    }
                    else
                    {
                        snapped.SnapInX = currentDirection == Types.Direction.UP || currentDirection == Types.Direction.DOWN;
                        snapped.SnapInY = currentDirection == Types.Direction.LEFT || currentDirection == Types.Direction.RIGHT;
                        snapped.ClampInX = snapped.SnapInY;
                        snapped.MinX = Utils.Values.Min<float>(origin.x, target.x);
                        snapped.MaxX = Utils.Values.Max<float>(origin.x, target.x);
                        snapped.ClampInY = snapped.SnapInX;
                        snapped.MinY = Utils.Values.Min<float>(origin.y, target.y);
                        snapped.MaxY = Utils.Values.Max<float>(origin.y, target.y);
                        oriented.direction = currentDirection;
                        rigidBody2d.velocity = VelocityForCurrentDirection();
                        SetMovingAnimation();
                    }
                }
            }
        }
    }
}