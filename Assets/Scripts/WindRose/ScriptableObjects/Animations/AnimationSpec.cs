using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Animations
        {
            [CreateAssetMenu(fileName = "NewAnimationSpec", menuName = "Wind Rose/Objects/Animation Spec", order = 202)]
            public class AnimationSpec : ScriptableObject
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

                public uint FPS { get { return fps; } }
                public Sprite[] Sprites { get { return sprites; } }
            }
        }
    }
}