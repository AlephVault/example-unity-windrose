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
                namespace Entities
                {
                    /// <summary>
                    ///   <para>
                    ///     The layer that will contain the map objects. This layer is mandatory.
                    ///   </para>
                    ///   <para>
                    ///     This component depends on <see cref="Grid"/> and will provide the
                    ///       values of the cell size. It will match the same values in the
                    ///       <see cref="Floor.FloorLayer"/>'s <see cref="Grid" /> component,
                    ///       although it is not strictly required or enforced.
                    ///   </para>
                    ///   <para>
                    ///     This layer contains 3 sub-layers intended for sorting. Child objects
                    ///       go in the middle layer, while underlays and overlays (when they
                    ///       are used) go in the background and foreground sub layers.
                    ///   </para>
                    /// </summary>
                    [RequireComponent(typeof(Grid))]
                    public class EntitiesLayer : MapLayer
                    {
                        private Grid grid;

                        /// <summary>
                        ///   This is the underlays sub-layer. Intended for feet-level glows or
                        ///     effects.
                        /// </summary>
                        public SortingSubLayer UnderlaysSubLayer { get; private set; }

                        /// <summary>
                        ///   All the <see cref="Entities.Objects.Object"/> objects will fall inside this
                        ///     sub layer. Their underlays or overlays will fall inside the
                        ///     other two sub layers.
                        /// </summary>
                        /// <seealso cref="UnderlaysSubLayer"/>
                        /// <seealso cref="OverlaysSubLayer"/>
                        public SortingSubLayer ObjectsSubLayer { get; private set; }

                        /// <summary>
                        ///   This is the overlays sub-layer. Intended for head-level glows or
                        ///     effects (e.g. lights from elsewhere, shadows of doors,...).
                        /// </summary>
                        public SortingSubLayer OverlaysSubLayer { get; private set; }

                        private SortingSubLayer AddSortingSubLayer(string name, int sortOrder)
                        {
                            GameObject gameObject = new GameObject(name);
                            gameObject.transform.parent = transform;
                            SortingGroup sortingGroup = gameObject.AddComponent<SortingGroup>();
                            sortingGroup.sortingLayerID = 0;
                            sortingGroup.sortingOrder = sortOrder;
                            return gameObject.AddComponent<SortingSubLayer>();
                        }

                        protected override void Awake()
                        {
                            base.Awake();
                            grid = GetComponent<Grid>();
                            UnderlaysSubLayer = AddSortingSubLayer("UnderlaysSubLayer", 0);
                            ObjectsSubLayer = AddSortingSubLayer("MainSubLayer", 10);
                            OverlaysSubLayer = AddSortingSubLayer("OverlaysSubLayer", 20);
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
