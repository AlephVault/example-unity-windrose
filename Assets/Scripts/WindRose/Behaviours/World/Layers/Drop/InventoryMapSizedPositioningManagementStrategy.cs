using System;
using System.Collections.Generic;
using UnityEngine;

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
                    using Support.Utils;
                    using Inventory.ManagementStrategies.PositioningStrategies;
                    public class InventoryMapSizedPositioningManagementStrategy : InventoryPositioningManagementStrategy
                    {
                        /**
                         * This clas validates and iterates position based on the map's dimensions.
                         */

                        private uint width;
                        private uint height;

                        protected override void Awake()
                        {
                            base.Awake();
                            Map map = Layout.RequireComponentInParent<Map>(this);
                            width = map.Width;
                            height = map.Height;
                        }

                        public override bool IsValid(object position)
                        {
                            if (position is Vector2Int)
                            {
                                Vector2Int vector = (Vector2Int)position;
                                return (Values.In(0, vector.x, (int?)(width - 1)) && Values.In(0, vector.y, (int?)(height - 1)));
                            }
                            return false;
                        }

                        public override IEnumerable<object> Positions()
                        {
                            for (var ix = 0; ix < width; ix++)
                                for (var iy = 0; iy < height; iy++)
                                    yield return new Vector2Int(ix, iy);
                        }
                    }
                }
            }
        }
    }
}
