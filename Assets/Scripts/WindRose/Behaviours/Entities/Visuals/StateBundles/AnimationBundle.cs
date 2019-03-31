﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Visuals
            {
                namespace StateBundles
                {
                    /// <summary>
                    ///   State bundle for animations.
                    /// </summary>
                    [RequireComponent(typeof(MultiAnimated))]
                    public abstract class AnimationBundle : StateBundle<ScriptableObjects.Animations.Animation>
                    {
                    }
                }
            }
        }
    }
}
