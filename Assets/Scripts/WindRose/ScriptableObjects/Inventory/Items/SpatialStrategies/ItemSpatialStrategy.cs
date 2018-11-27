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
                namespace SpatialStrategies
                {
                    using Types.Inventory.Stacks.SpatialStrategies;

                    public abstract class ItemSpatialStrategy : ItemStrategy<StackSpatialStrategy>
                    {
                        /**
                         * Spatial strategies are data bundles telling to which
                         *   extent is the item allowed to be added to an inventory.
                         *   Quite frequently, stacks occupy one inventory slot, which
                         *   is indexed. However, in games like Neverwinter Nights,
                         *   stacks may have irregular dimensions, with the inventory
                         *   being a bidimensional matrix. The player is compelled to
                         *   optimize the way they organize their bag.
                         *   
                         * Since they are data bundles, they have no particular fields.
                         *   They will have just one method to create a standard stack
                         *   spatial strategy instance, which will hold an arbitrary
                         *   position that depends on the inventory that holds this stack
                         *   or will become null while not contained in an inventory. Logic
                         *   should (will) be present in the inventory spatial counterpart
                         *   strategy.
                         * 
                         * Only one spatial strategy is allowed on an item. Spatial
                         *   strategies will have no dependencies.
                         * 
                         * This class remains abstract, since data has to be added.
                         */

                        public override StackSpatialStrategy CreateStackStrategy(object argument)
                        {
                            return new StackSpatialStrategy(this, null);
                        }
                    }
                }
            }
        }
    }
}
