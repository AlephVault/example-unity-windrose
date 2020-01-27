using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackPack
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace PositioningStrategies
                {
                    /// <summary>
                    ///   Tells which positions are valid to handle the containers in the related
                    ///     inventory. There will be at least two strategies here: single-inventories
                    ///     or floor-inventories (containers in a Width x Height matrix).
                    /// </summary>
                    public abstract class InventoryPositioningManagementStrategy : InventoryManagementStrategy
                    {
                        /// <summary>
                        ///   Tells when a container ID is invalid for this strategy.
                        /// </summary>
                        public class InvalidPositionException : GMM.Types.Exception
                        {
                            public InvalidPositionException(string message) : base(message) {}
                        }
                        
                        /// <summary>
                        ///   Tells whether a container ID is invalid for this strategy.
                        ///     This will imply the raise of <see cref="InvalidPositionException"/>.
                        /// </summary>
                        /// <param name="position">The position to check</param>
                        /// <returns>Whether it is valid</returns>
                        protected abstract bool IsValid(object position);

                        /// <summary>
                        ///   An enumerable yielding all the valid positions for this strategy.
                        /// </summary>
                        /// <returns></returns>
                        public abstract IEnumerable<object> Positions();

                        /// <summary>
                        ///   Checks that the given position is valid. If invalid, raises
                        ///     an exception.
                        /// </summary>
                        /// <param name="position">The position to check</param>
                        /// <seealso cref="IsValid(object)"/>
                        /// <seealso cref="InvalidPositionException"/>
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
