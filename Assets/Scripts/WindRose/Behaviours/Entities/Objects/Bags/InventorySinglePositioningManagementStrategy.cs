using System.Collections.Generic;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace Bags
            {
                using Inventory.ManagementStrategies.PositioningStrategies;

                /// <summary>
                ///   Handles and yield one single position. One container
                ///     will be allowed.
                /// </summary>
                public class InventorySinglePositioningManagementStrategy : InventoryPositioningManagementStrategy
                {
                    /// <summary>
                    ///   The only valid position will be <see cref="Position.Instance"/>.
                    /// </summary>
                    /// <param name="position">The position to check</param>
                    /// <returns>Whether the position value is <see cref="Position.Instance"/></returns>
                    protected override bool IsValid(object position)
                    {
                        return position == Position.Instance;
                    }

                    /// <summary>
                    ///   The only yielded position is <see cref="Position.Instance"/>.
                    /// </summary>
                    /// <returns>The iterator yielding that single position</returns>
                    public override IEnumerable<object> Positions()
                    {
                        yield return Position.Instance;
                    }
                }
            }
        }
    }
}
