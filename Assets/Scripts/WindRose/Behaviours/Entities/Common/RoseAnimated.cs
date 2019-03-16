using UnityEngine;

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
                ///   The orientation to consider. Will be tracked for changes as well.
                /// </summary>
                /// <seealso cref="Orientation"/>
                [SerializeField]
                private Types.Direction orientation = Types.Direction.FRONT;

                // Track the current state to not update unnecessarily the animation later.
                private ScriptableObjects.Animations.AnimationRose animationRose;

                // Refreshes the underlying animation.
                private void RefreshAnimation()
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
                ///   Gets or sets the current orientation, and updates the animation (on set).
                /// </summary>
                public Types.Direction Orientation
                {
                    get { return orientation; }
                    set
                    {
                        if (orientation != value)
                        {
                            orientation = value;
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

                private void DoStart()
                {
                    SetDefaultAnimationRose();
                }
            }
        }
    }
}
