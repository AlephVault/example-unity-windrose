using System;
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
                    namespace Moving
                    {
                        public class MovingAnimationBundle : AnimationBundle
                        {
                            protected override string GetStateKey()
                            {
                                return "moving";
                            }
                        }
                    }
                }
            }
        }
    }
}
