﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Animations
        {
            /// <summary>
            ///   This class defines a WindRose animation spec. An animation spec contains the frames to render
            ///     and the amount of intended frames per second. Usually, this class is used inside an animation
            ///     set (for certain direction in such set) or as default animation for objects that are not
            ///     orientable (i.e. they have no direction to look/move to).
            /// </summary>
            [CreateAssetMenu(fileName = "NewAnimationSpec", menuName = "Wind Rose/Objects/Animation Spec", order = 202)]
            public class AnimationSpec : ScriptableObject
            {
                public class Exception : Types.Exception
                {
                    public Exception() { }
                    public Exception(string message) : base(message) { }
                    public Exception(string message, System.Exception inner) : base(message, inner) { }
                }

                /// <summary>
                ///   The frames to render.
                /// </summary>
                [SerializeField]
                private Sprite[] sprites;

                /// <summary>
                ///   The frames per second.
                /// </summary>
                [SerializeField]
                private uint fps;

                /// <summary>
                ///   See <see cref="fps"/>.
                /// </summary>
                public uint FPS { get { return fps; } }

                /// <summary>
                ///   See <see cref="sprites"/> .
                /// </summary>
                public Sprite[] Sprites { get { return sprites; } }
            }
        }
    }
}