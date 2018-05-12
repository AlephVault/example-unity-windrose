using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        // Requiring Snapped, instead of Positionable, allows us to
        //   have the features of position automatically updated.
        //
        // We make no use of Snapped at all, but the behavior will
        //   automatically be called, and Positionable will be
        //   present anyway.
        [RequireComponent(typeof(Snapped))]
        [RequireComponent(typeof(Sorted))]
        public class Represented : MonoBehaviour
        {
            /**
             * A represented object is a positionable object which can also display
             *   a sprite (it is also a SpriteRenderer object). It will provide an
             *   animation which will change on each frame.
             */

            private SpriteRenderer spriteRenderer;

            [SerializeField]
            private Types.AnimationSpec defaultAnimation;

            private Types.AnimationSpec currentAnimation;
            private float currentTime;
            private float frameInterval;
            private int currentAnimationIndex;

            public Types.AnimationSpec CurrentAnimation
            {
                get { return currentAnimation; }
                set {
                    if (currentAnimation != value) {
                        currentAnimation = value;
                        Reset();
                    }
                }
            }

            public void SetDefaultAnimation()
            {
                CurrentAnimation = defaultAnimation;
            }

            void Awake()
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            void Start()
            {
                SetDefaultAnimation();
            }

            private void Reset()
            {
                currentTime = 0;
                currentAnimationIndex = 0;
                frameInterval = 1.0f / currentAnimation.FPS;                
            }

            private Sprite Thick() {
                currentTime += Time.deltaTime;
                if (currentTime > frameInterval)
                {
                    currentTime -= frameInterval;
                    currentAnimationIndex = ((currentAnimationIndex + 1) % CurrentAnimation.Sprites.Length);
                }
                return CurrentAnimation.Sprites[currentAnimationIndex];
            }

            void Update()
            {
                spriteRenderer.sprite = Thick();
            }

            void Pause(bool fullFreeze)
            {
                enabled = !fullFreeze;
            }

            void Resume()
            {
                enabled = true;
            }
        }
    }
}