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
                // TODO implement this class
                public class DropLayer : MapLayer
                {
                    private Drops.DropStack[,] dropStacks;

                    protected override int GetSortingOrder()
                    {
                        return 10;
                    }

                    protected override void Awake()
                    {
                        base.Awake();
                        dropStacks = new Drops.DropStack[Map.Width, Map.Height];
                    }

                    public void Push(uint x, uint y, Drops.Drop drop)
                    {

                    }
                }
            }
        }
    }
}
