using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace PositioningStrategies
                {
                    public abstract class InventoryPositioningManagementStrategy : InventoryManagementStrategy
                    {
                        /**
                         * Tells which positions are valid to handle inventories.
                         * 
                         * There will be at least two strategies here:
                         * - Single-inventories allow only `null` position.
                         * - Floor-inventories allow and iterate over WxH positions.
                         */

                        public class InvalidPositionException : Types.Exception
                        {
                            /**
                             * This class tells that a given position is not valid on this spatial
                             *   container. 
                             */

                            public InvalidPositionException(string message) : base(message) {}
                        }

                        public abstract bool IsValid(object position);
                        public abstract IEnumerable<object> Positions();
                        public void CheckPosition(object position)
                        {
                            if (!IsValid(position))
                            {
                                throw new InvalidPositionException(string.Format("Invalid inventory position: {0}", position));
                            }
                        }
                    }
                }
            }
        }
    }
}
