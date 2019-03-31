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
                    ///   State bundle for sprites.
                    /// </summary>
                    [RequireComponent(typeof(MultiSprite))]
                    public abstract class SpriteBundle : StateBundle<Sprite>
                    {
                    }
                }
            }
        }
    }
}