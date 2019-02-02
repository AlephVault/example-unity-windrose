using System;
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
                namespace Objects
                {
                    /// <summary>
                    ///   <para>
                    ///     The layer that will contain the map objects. This layer is mandatory.
                    ///   </para>
                    ///   <para>
                    ///     This component depends on <see cref="Grid"/> and will provide the
                    ///       values of the cell size. It should match the same values in the
                    ///       <see cref="Floor.FloorLayer"/>'s <see cref="Grid" /> component,
                    ///       although it is not strictly required or enforced.
                    ///   </para>
                    /// </summary>
                    [RequireComponent(typeof(Grid))]
                    public class ObjectsLayer : MapLayer
                    {
                        private Grid grid;

                        protected override void Awake()
                        {
                            base.Awake();
                            grid = GetComponent<Grid>();
                        }

                        protected override int GetSortingOrder()
                        {
                            return 20;
                        }

                        /// <summary>
                        ///   Gets the cell width.
                        /// </summary>
                        /// <returns>The cell width, in game units</returns>
                        public float GetCellWidth()
                        {
                            return grid.cellSize.x;
                        }

                        /// <summary>
                        ///   Gets the cell height.
                        /// </summary>
                        /// <returns>The cell height, in game units</returns>
                        public float GetCellHeight()
                        {
                            return grid.cellSize.y;
                        }
                    }
                }
            }
        }
    }
}
