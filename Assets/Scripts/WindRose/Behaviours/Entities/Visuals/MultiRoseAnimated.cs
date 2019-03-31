using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindRose.ScriptableObjects.Animations;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Visuals
            {
                /// <summary>
                ///   MultiRoseAnimated state managers involve a rose animated behavior and
                ///     will give them the state in form of an <see cref="AnimationRose"/>.
                /// </summary>
                public class MultiRoseAnimated : MultiState<AnimationRose>
                {
                    private RoseAnimated roseAnimated;

                    protected override void UseState(AnimationRose state)
                    {
                        roseAnimated.AnimationRose = state;
                    }

                    protected override void Awake()
                    {
                        base.Awake();
                        roseAnimated = GetComponent<RoseAnimated>();
                    }
                }
            }
        }
    }
}

