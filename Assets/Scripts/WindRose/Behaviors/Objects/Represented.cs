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

            public Types.AnimationSpec CurrentAnimation
            {
                get { return currentAnimation.Clone(); }
                set { currentAnimation = value.Clone(); }
            }

            public void SetDefaultAnimation()
            {
                CurrentAnimation = defaultAnimation;
            }

            void Start()
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                SetDefaultAnimation();
            }

            void Update()
            {
                // We set the current animation.
                // We are sure this animation will be non-empty.
                currentAnimation.Thick();
                spriteRenderer.sprite = currentAnimation.CurrentSprite;
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