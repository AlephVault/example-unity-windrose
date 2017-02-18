using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Types
    {
        [System.Serializable]
        public struct AnimationSet
        {
            private bool _initialized;

            [SerializeField]
            private AnimationSpec up;
            [SerializeField]
            private AnimationSpec down;
            [SerializeField]
            private AnimationSpec left;
            [SerializeField]
            private AnimationSpec right;

            public AnimationSet(AnimationSpec up, AnimationSpec down, AnimationSpec left, AnimationSpec right)
            {
                this.up = up;
                this.down = down;
                this.left = left;
                this.right = right;
                this._initialized = false;
                this.Initialize();
            }

            public AnimationSet Clone()
            {
                return new Types.AnimationSet(up.Clone(), down.Clone(), left.Clone(), right.Clone());
            }

            public void Initialize()
            {
                if (_initialized)
                {
                    return;
                }
                up.Initialize();
                down.Initialize();
                left.Initialize();
                right.Initialize();
                this._initialized = true;
            }

            public AnimationSpec GetForDirection(Direction direction)
            {
                switch(direction)
                {
                    case Direction.UP:
                        return up;
                    case Direction.DOWN:
                        return down;
                    case Direction.LEFT:
                        return left;
                    case Direction.RIGHT:
                        return right;
                    default:
                        // No default will run here,
                        //   but just for code completeness
                        return down;
                }
            }
        }
    }
}