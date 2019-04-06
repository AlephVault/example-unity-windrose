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
                namespace Ceiling
                {
                    /// <summary>
                    ///   The topmost layer (if present, and no custom-type
                    ///     layers exist). It will hold <see cref="Ceilings.Ceiling"/>
                    ///     objects inside.
                    /// </summary>
                    [RequireComponent(typeof(Grid))]
                    public class CeilingLayer : MapLayer
                    {
                        protected override int GetSortingOrder()
                        {
                            return 40;
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                            // We sort the layers accordingly - please use different sorting orders explicitly.
                            Grid grid = GetComponent<Grid>();
                            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
                        }
                    }
                }
            }
        }
    }
}
