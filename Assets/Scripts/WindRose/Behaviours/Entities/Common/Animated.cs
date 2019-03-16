using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            /// <summary>
            ///   Handles the object's ability to animate, given a sequence of sprites.
            /// </summary>
            [RequireComponent(typeof(SpriteRenderer))]
            public class Animated : MonoBehaviour
            {
                protected SpriteRenderer spriteRenderer;

                /// <summary>
                ///   The default animation, for when no other animation is given.
                /// </summary>
                [SerializeField]
                private ScriptableObjects.Animations.Animation defaultAnimation;

                /**
                 * Stuff to handle and render the current animation.
                 */

                private new ScriptableObjects.Animations.Animation animation;
                private float currentTime;
                private float frameInterval;
                private int currentAnimationIndex;

                /// <summary>
                ///   Gets or sets the current animation, and resets it (on set).
                /// </summary>
                public ScriptableObjects.Animations.Animation Animation
                {
                    get { return animation; }
                    set
                    {
                        if (animation != value)
                        {
                            animation = value;
                            Reset();
                        }
                    }
                }

                /// <summary>
                ///   Sets the current animation to the default one.
                /// </summary>
                public void SetDefaultAnimation()
                {
                    Animation = defaultAnimation;
                }

                protected virtual void Awake()
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    spriteRenderer.enabled = false;
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Start of the positionable. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Initializes the default animation.
                ///   </para>
                /// </summary>
                public void DoStart()
                {
                    SetDefaultAnimation();
                }

                private void Reset()
                {
                    currentTime = 0;
                    currentAnimationIndex = 0;
                    frameInterval = 1.0f / animation.FPS;
                }

                private Sprite Tick()
                {
                    currentTime += Time.deltaTime;
                    if (currentTime > frameInterval)
                    {
                        currentTime -= frameInterval;
                        currentAnimationIndex = ((currentAnimationIndex + 1) % Animation.Sprites.Length);
                    }
                    return Animation.Sprites[currentAnimationIndex];
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Update of the positionable. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Updates the current animation frame on the object.
                ///   </para>
                /// </summary>
                public void DoUpdate()
                {
                    spriteRenderer.sprite = Tick();
                }
            }
        }
    }
}