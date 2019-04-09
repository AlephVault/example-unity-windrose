﻿using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Visuals
            {
                /// <summary>
                ///   Handles the object's ability to animate, given a sequence of sprites.
                /// </summary>
                public class Animated : VisualBehaviour
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
                                if (animation) Reset();
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

                    protected override void Awake()
                    {
                        base.Awake();
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
                    ///   Triggered when the underlying visual is started.
                    ///   Ensures the default animation to be selected.
                    /// </summary>
                    public override void DoStart()
                    {
                        SetDefaultAnimation();
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

                    /// <summary>
                    ///   Triggered when the underlying visual is updated. Updating this behaviour involves
                    ///     ensuring appropriate frame in the animation.
                    /// </summary>
                    public override void DoUpdate()
                    {
                        Frame();
                    }
                }
            }
        }
    }
}