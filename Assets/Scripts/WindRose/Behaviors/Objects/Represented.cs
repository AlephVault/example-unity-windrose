using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        // Requiring Snapped, instead of Positionable, allows us to
        //   have the features of position automatically updated.
        //
        // We make no use of Snapped at all, but the behavior will
        //   automatically be called, and Positionable will be
        //   present anyway.
        [RequireComponent(typeof(Snapped))]
        [RequireComponent(typeof(SpriteRenderer))]
        public class Represented : MonoBehaviour
        {
            /**
             * A represented object is a positionable object which can also display
             *   a sprite (it is also a SpriteRenderer object). It will provide an
             *   animation which will change on each frame.
             */

            private SpriteRenderer spriteRenderer;
            private Positionable positionable;

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
                positionable = GetComponent<Positionable>();
                SetDefaultAnimation();
            }

            void Update()
            {
                // We order the sprite
                spriteRenderer.sortingOrder = (int)(positionable.Yf * positionable.ParentMap.Width + positionable.Xf) * 3 + 1;
                // And also set the current animation.
                //   We are sure this animation will be non-empty.
                currentAnimation.Thick();
                spriteRenderer.sprite = currentAnimation.CurrentSprite;
            }
        }
    }
}