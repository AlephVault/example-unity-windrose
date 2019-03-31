using System.Collections;
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
                    ///   State bundle for animation roses.
                    /// </summary>
                    [RequireComponent(typeof(MultiRoseAnimated))]
                    public abstract class AnimationRoseBundle : StateBundle<ScriptableObjects.Animations.AnimationRose>
                    {
                    }
                }
            }
        }
    }
}