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
                [RequireComponent(typeof(Grid))]
                public class CeilingLayer : MapLayer
                {
                    protected override int GetSortingOrder()
                    {
                        return 30;
                    }
                }
            }
        }
    }
}
