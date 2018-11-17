using UnityEngine;
using System.Collections.Generic;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace UsageStrategies
                {
                    using Types.Inventory.Stacks.UsageStrategies;

                    public abstract class ItemUsageStrategy : ItemStrategy<StackUsageStrategy>
                    {
                        /**
                         * Usage strategies are data bundles being accessed
                         *   from the stacks and will provide data related to
                         *   their usage (however, the usage will be managed
                         *   entirely by the related inventory strategy).
                         * 
                         * Since they are data bundles, they have no particular
                         *   fields. They will have just one method to create
                         *   a compatible stack usage strategy instance, which
                         *   must be implemented. Logic should (will) be present
                         *   in the stack usage counterpart strategy, or more
                         *   likely the inventory usage strategy.
                         * 
                         * The same item may have more than one usage strategy.
                         *   The stacks will have related stack strategies (one for
                         *   each strategy in the item) and in both cases the added
                         *   strategies will depend among themselves.
                         */
                    }
                }
            }
        }
    }
}
