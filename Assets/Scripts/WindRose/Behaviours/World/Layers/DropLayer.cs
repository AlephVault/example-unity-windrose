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
                    protected override int GetSortingOrder()
                    {
                        return 10;
                    }
                }
            }
        }
    }
}
