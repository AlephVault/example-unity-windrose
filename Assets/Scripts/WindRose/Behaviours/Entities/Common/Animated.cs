using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Common
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

                private void Reset()
                {
                    currentTime = 0;
                    currentAnimationIndex = 0;
                    frameInterval = 1.0f / animation.FPS;
                }

                /// <summary>
                ///   Updates the current image. To be invoked, in different moments, by
                ///     the different subclasses.
                /// </summary>
                protected void Frame()
                {
                    currentTime += Time.deltaTime;
                    if (currentTime > frameInterval)
                    {
                        currentTime -= frameInterval;
                        currentAnimationIndex = ((currentAnimationIndex + 1) % Animation.Sprites.Length);
                    }
                    spriteRenderer.sprite = Animation.Sprites[currentAnimationIndex];
                }
            }
        }
    }
}