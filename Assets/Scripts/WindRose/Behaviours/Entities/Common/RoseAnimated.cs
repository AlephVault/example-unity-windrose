﻿using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Common
        {
            /// <summary>
            ///   Handles the object's ability to animate, given four sequences of sprites.
            ///   From those four sequences, it will pick the appropriate for the current
            ///     direction, which is a behaviour to be implemented. The fetched animation
            ///     will be given to its related <see cref="Animated"/> component.
            /// </summary>
            [RequireComponent(typeof(Animated))]
            public abstract class RoseAnimated : MonoBehaviour
            {
                private Animated animated;

                /// <summary>
                ///   The default animation rose, for when no other animation is given.
                /// </summary>
                [SerializeField]
                private ScriptableObjects.Animations.AnimationRose defaultAnimationRose;

                /// <summary>
                ///   The current orientation. Different behaviours will set this value
                ///     in different moments, likely related to the <see cref="Objects.Oriented"/>
                ///     behaviour subscription.
                /// </summary>
                protected Types.Direction orientation = Types.Direction.FRONT;

                // Track the current state to not update unnecessarily the animation later.
                private ScriptableObjects.Animations.AnimationRose animationRose;

                // Refreshes the underlying animation.
                protected void RefreshAnimation()
                {
                    animated.Animation = animationRose.GetForDirection(orientation);
                }

                /// <summary>
                ///   Gets or sets the current animation rose, and updates the animation (on set).
                /// </summary>
                public ScriptableObjects.Animations.AnimationRose AnimationRose
                {
                    get { return animationRose; }
                    set
                    {
                        if (animationRose != value)
                        {
                            animationRose = value;
                            RefreshAnimation();
                        }
                    }
                }

                /// <summary>
                ///   Sets the current animation rose to the default one.
                /// </summary>
                public void SetDefaultAnimationRose()
                {
                    AnimationRose = defaultAnimationRose;
                }

                private void Awake()
                {
                    animated = GetComponent<Animated>();
                }
            }
        }
    }
}
