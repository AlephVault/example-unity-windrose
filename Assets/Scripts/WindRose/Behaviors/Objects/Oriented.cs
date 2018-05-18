using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(Represented))]
            public class Oriented : MonoBehaviour
            {
                /**
                 * This class holds a set of animations ready to be used by depending
                 *   behaviors (e.g. walking, running). By default it uses the IDLE_ANIMATION
                 *   animation key with a default animation set.
                 * 
                 * This class takes into account the current direction it is looking to, and
                 *   also the current animation being played (animations are provided by
                 *   different animation sets, which provide an animation for each possible
                 *   direction).
                 */

                public const string IDLE_ANIMATION = "";

                public class Exception : Types.Exception
                {
                    public Exception() { }
                    public Exception(string message) : base(message) { }
                    public Exception(string message, System.Exception inner) : base(message, inner) { }
                }

                private Dictionary<string, Types.AnimationSet> animations = new Dictionary<string, Types.AnimationSet>();
                private string previousAnimationKey = "";
                private Types.Direction previousOrientation = Types.Direction.DOWN;

                private Represented represented;
                private Positionable positionable;

                [SerializeField]
                private Types.AnimationSet idleAnimationSet;

                public Types.Direction orientation = Types.Direction.DOWN;

                [HideInInspector]
                public string animationKey = IDLE_ANIMATION;
                // Perhaps we want to override the animation being used as idle,
                //   with a new one. It is intended to serve as a "temporary" idle
                //   animation for any reason.
                [HideInInspector]
                public string overriddenKeyForIdleAnimation = null;

                private void SetCurrentAnimation()
                {
                    try
                    {
                        represented.CurrentAnimation = animations[animationKey].GetForDirection(orientation);
                    }
                    catch (KeyNotFoundException)
                    {
                        // Key IDLE_ANIMATION will always be available
                        animationKey = IDLE_ANIMATION;
                    }
                }

                public void SetIdleAnimation()
                {
                    animationKey = (overriddenKeyForIdleAnimation == null) ? IDLE_ANIMATION : overriddenKeyForIdleAnimation;
                }

                /**
                 * Usually, adding an animation set is done when creating custom behaviours.
                 *   Adding one will check whether the key does not exist, and fail otherwise.
                 */
                public void AddAnimationSet(string key, Types.AnimationSet animation)
                {
                    if (animations.ContainsKey(key))
                    {
                        throw new Types.Exception("AnimationSet key already in use: " + key);
                    }
                    else
                    {
                        animations.Add(key, animation);
                    }
                }

                /**
                 * In contrast to AddAnimationSet, this method replaces an existing animation set
                 *   in the middle of an execution of all the present behaviours. This method will check
                 *   whether the animation exists beforehand, and fail otherwise.
                 *   
                 * This is intentional to prevent any accident creating new animations when it was not intended.
                 */
                public void ReplaceAnimationSet(string key, Types.AnimationSet animation)
                {
                    if (animations.ContainsKey(key))
                    {
                        throw new Types.Exception("AnimationSet key does not exist: " + key);
                    }
                    else
                    {
                        animations[key] = animation;
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
                    represented = GetComponent<Represented>();
                }

                void Start()
                {
                    SetCurrentAnimation();
                }

                // Update is called once per frame
                void Update()
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
}