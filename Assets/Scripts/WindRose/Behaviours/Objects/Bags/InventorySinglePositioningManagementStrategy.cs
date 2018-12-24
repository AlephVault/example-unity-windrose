using System.Collections.Generic;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Bags
            {
                using Inventory.ManagementStrategies.PositioningStrategies;

                public class InventorySinglePositioningManagementStrategy : InventoryPositioningManagementStrategy
                {
                    /**
                     * This class only yields and validates the null value as position.
                     */

                    public override bool IsValid(object position)
                    {
                        return position == null;
                    }

                    public override IEnumerable<object> Positions()
                    {
                        yield return null;
                    }
                }
            }
        }
    }
}
