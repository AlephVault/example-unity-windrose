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
                    public abstract class ItemSpatialStrategy : ItemStrategy<object>
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
                         * This class will have no behaviour, but just data. There is
                         *   no need to hold a counterpart Stack Spatial Strategy
                         *   since all the behaviour will exist on the inventory
                         *   strategy, and the stack will have no need to know
                         *   anything additional.
                         * 
                         * This class remains abstract, since data has to be added.
                         */

                        public object CreateStackStrategy()
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}
