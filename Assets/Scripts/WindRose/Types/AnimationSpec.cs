using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Types
    {
        [System.Serializable]
        public struct AnimationSpec
        {
            public class Exception : Types.Exception
            {
                public Exception() { }
                public Exception(string message) : base(message) { }
                public Exception(string message, System.Exception inner) : base(message, inner) { }
            }

            [SerializeField]
            private Sprite[] sprites;

            [SerializeField]
            private uint fps;

            private uint index;
            private float _secFraction;
            private float _currentTime;
            private bool _initialized;

            public AnimationSpec(Sprite[] sprites, uint fps = 0)
            {
                this.sprites = CopyArray(sprites);
                this.fps = fps;
                // Dummy assignments
                _secFraction = 0;
                _currentTime = 0;
                _initialized = false;
                index = 0;
                // Initialization
                this.Initialize();
            }

            public AnimationSpec Clone()
            {
                return new AnimationSpec(sprites, fps);
            }

            public void Initialize()
            {
                if (_initialized) {
                    return;
                }
                if (sprites == null || sprites.Length == 0)
                {
                    throw new Exception("Cannot have a null or empty animation in an AnimationSpec instance");
                }
                this.fps = (fps == 0) ? ((uint)sprites.Length) : fps;
                this._secFraction = (float)(1.0 / this.fps);
                this.Reset();
                this._initialized = true;
            }

            public void Reset()
            {
                this.index = 0;
                this._currentTime = 0;
            }

            public void Thick()
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > _secFraction)
                {
                    _currentTime -= _secFraction;
                    index = (uint) ((index + 1) % sprites.Length);
                }
            }

            private static Sprite[] CopyArray(Sprite[] source)
            {
                return (source != null) ? ((Sprite[])source.Clone()) : null;
            }

            public uint FPS { get { return fps; } }
            public uint CurrentIndex { get { return index; } }
            public Sprite CurrentSprite { get { return sprites[index]; } }
            public Sprite[] Sprites { get { return CopyArray(sprites); } }
        }
    }
}