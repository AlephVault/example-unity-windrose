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

            public enum SubLayer { LOW, MIDDLE, HIGH }
            [SerializeField]
            private SubLayer subLayer = SubLayer.MIDDLE;

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
                int sortingOffset = (int)(positionable.ParentMap.Width + positionable.ParentMap.Height) * ((int)(subLayer));
                spriteRenderer.sortingOrder = sortingOffset + (int)(positionable.Yf * positionable.ParentMap.Width + positionable.Xf);
                // And also set the current animation.
                //   We are sure this animation will be non-empty.
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