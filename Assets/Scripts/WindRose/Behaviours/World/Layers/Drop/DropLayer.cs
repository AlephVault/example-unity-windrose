using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Layers
            {
                namespace Drop
                {
                    // TODO implement this class
                    public class DropLayer : MapLayer
                    {
                        protected override int GetSortingOrder()
                        {
                            return 10;
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                        }
                    }
                }
            }
        }
    }
}
