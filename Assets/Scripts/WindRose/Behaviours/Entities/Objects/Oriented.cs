using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            /// <summary>
            ///   <para>
            ///     This class holds a set of animations ready to be used by depending
            ///       behaviours (e.g. walking. running). By default it uses the IDLE_ANIMATION
            ///       animation key with a default animation set.
            ///   </para>
            ///   <para>
            ///     This class takes into account the current direction it is looking to, and
            ///       also the current animation being played (animations are provided by
            ///       different animation sets, which provide an animation for each possible
            ///       direction).
            ///   </para>
            /// </summary>
            [RequireComponent(typeof(Animated))]
            public class Oriented : MonoBehaviour
            {
                /// <summary>
                ///   The default animation key provided by Orientable components.
                /// </summary>
                public const string IDLE_ANIMATION = "";

                /// <summary>
                ///   Tells when an error is raised inside Orientable component methods.
                /// </summary>
                public class Exception : Types.Exception
                {
                    public Exception() { }
                    public Exception(string message) : base(message) { }
                    public Exception(string message, System.Exception inner) : base(message, inner) { }
                }

                // All the registered animations
                private Dictionary<string, ScriptableObjects.Animations.AnimationRose> animations = new Dictionary<string, ScriptableObjects.Animations.AnimationRose>();

                // All the temporary replacements for the registered animations
                private Dictionary<string, ScriptableObjects.Animations.AnimationRose> replacements = new Dictionary<string, ScriptableObjects.Animations.AnimationRose>();

                private string previousAnimationKey = "";
                private Types.Direction previousOrientation = Types.Direction.DOWN;

                private Animated represented;
                private Positionable positionable;

                /// <summary>
                ///   The default animation set (used by default for Orientable elements,
                ///     and usually meaning the object is standing).
                /// </summary>
                [SerializeField]
                private ScriptableObjects.Animations.AnimationRose idleAnimationSet;

                /// <summary>
                ///   The object's orientation. Changing this value will change the sprite
                ///     sequence to be rendered inside the currently set <see cref="AnimationSet"/>.
                /// </summary>
                public Types.Direction orientation = Types.Direction.DOWN;

                /// <summary>
                ///   The key of the animation being rendered.
                /// </summary>
                [HideInInspector]
                public string animationKey = IDLE_ANIMATION;

                /// <summary>
                ///   <para>
                ///     By setting this value to a non-null string, this value will be
                ///       used instead of <see cref="IDLE_ANIMATION"/> to choose the
                ///       animation to be used when going idle.
                ///   </para>
                ///   <para>
                ///     This is not a matter of animation replacement, but a matter of
                ///       different logic in handing (e.g. changing this behaviour
                ///       to use the "tired" animation key (state) instead of the
                ///       "idle" animation key (state) without screwing either animation).
                ///   </para>
                /// </summary>
                [HideInInspector]
                public string overriddenKeyForIdleAnimation = null;

                private void SetCurrentAnimation()
                {
                    try
                    {
                        ScriptableObjects.Animations.AnimationRose replacement;
                        if (replacements.TryGetValue(animationKey, out replacement))
                        {
                            represented.Animation = replacement.GetForDirection(orientation);
                        }
                        else
                        {
                            represented.Animation = animations[animationKey].GetForDirection(orientation);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        // Key IDLE_ANIMATION will always be available
                        animationKey = IDLE_ANIMATION;
                    }
                }

                /// <summary>
                ///   Sets the current animation to idle. If <see cref="overriddenKeyForIdleAnimation"/>
                ///     is set, that value will be picked. Otherwise, <see cref="IDLE_ANIMATION"/> will
                ///     be used.
                /// </summary>
                public void SetIdleAnimation()
                {
                    animationKey = (overriddenKeyForIdleAnimation == null) ? IDLE_ANIMATION : overriddenKeyForIdleAnimation;
                }

                /// <summary>
                ///   Registers an animation set under a key. This is usually done when initializing
                ///     other components (e.g. moving components).
                /// </summary>
                /// <param name="key">The key to use</param>
                /// <param name="animationSet">The animation set to register</param>
                public void AddAnimationSet(string key, ScriptableObjects.Animations.AnimationRose animationSet)
                {
                    if (animations.ContainsKey(key))
                    {
                        throw new Types.Exception("AnimationSet key already in use: " + key);
                    }
                    else
                    {
                        animations.Add(key, animationSet);
                    }
                }

                /// <summary>
                ///   Replaces an existing animation set with a new one. This is run at run-time
                ///     and will require the animation being replaced to exist, or fail otherwise.
                ///     Set animation to <c>null</c> to clear the replacement on a given key.
                /// </summary>
                /// <param name="key">The key of the animation being replaced</param>
                /// <param name="animation">The new animation to use, or null to undo the replacement</param>
                public void ReplaceAnimationSet(string key, ScriptableObjects.Animations.AnimationRose animation)
                {
                    if (animations.ContainsKey(key))
                    {
                        throw new Types.Exception("AnimationSet key does not exist: " + key);
                    }
                    else
                    {
                        if (animation != null)
                        {
                            replacements[key] = animation;
                        }
                        else
                        {
                            replacements.Remove(key);
                        }
                        if (key == animationKey) SetCurrentAnimation();
                    }
                }

                // Use this for initialization
                void Awake()
                {
                    // I DON'T KNOW WHY HIDDEN PROPERTIES FROM INSPECTOR ALSO AVOID NULL VALUES.
                    // So I'm adding this code to ensure this particular field starts as null in Awake().
                    overriddenKeyForIdleAnimation = null;
                    AddAnimationSet(IDLE_ANIMATION, idleAnimationSet);
                    positionable = GetComponent<Positionable>();
                    represented = GetComponent<Animated>();
                    positionable.onAttached.AddListener(delegate (World.Map map)
                    {
                        enabled = true;
                    });
                    positionable.onDetached.AddListener(delegate ()
                    {
                        enabled = false;
                    });
                }

                /// <summary>
                ///   <para>
                ///     This is a callback for the Start of the positionable. It is
                ///       not intended to be called directly.
                ///   </para>
                ///   <para>
                ///     Initializes the current animation set to the animation key.
                ///   </para>
                /// </summary>
                public void DoStart()
                {
                    SetCurrentAnimation();
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
                    // If the object is being moved, we assign the movement direction as the current orientation
                    if (positionable.Movement != null && positionable.Movement != orientation)
                    {
                        orientation = positionable.Movement.Value;
                    }

                    // Given an animation change or an orientation change, we change the animation
                    if (animationKey != previousAnimationKey || orientation != previousOrientation)
                    {
                        SetCurrentAnimation();
                    }

                    previousOrientation = orientation;
                    previousAnimationKey = animationKey;
                }
            }
        }
    }
}