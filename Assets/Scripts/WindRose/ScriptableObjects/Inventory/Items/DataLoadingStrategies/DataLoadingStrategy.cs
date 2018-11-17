using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                namespace DataLoadingStrategies
                {
                    using Types.Inventory.Stacks.DataDumpingStrategies;

                    public abstract class DataLoadingStrategy : ItemStrategy<DataDumpingStrategy>
                    {
                        /**
                         * A data loading strategy is not a normal strategy
                         *   like the others. This is the one you have to
                         *   manually create for your objects. This has an
                         *   underlying reason: you can combine many
                         *   different strategies, and you could have a
                         *   particular way of storing the data used to
                         *   populate your stack instances.
                         * 
                         * So, while each (other) strategy knows what data
                         *   they should expect (by casting the argument they
                         *   receive), this strategy must consider the item
                         *   strategy particular type and produce the
                         *   appropriate data result to be passed to its
                         *   .Create method. This resulting argument object
                         *   will be extracted from a main object given as
                         *   argument for these methods.
                         * 
                         * All these methods must be implemented and must
                         *   consider the appropriate types.
                         * 
                         * It also defines a .Create method which will return
                         *   the DataDumpingStrategy to add to the stack (which
                         *   will be the one that will export the data in an
                         *   appropriate format).
                         */

                        public abstract object LoadDataFor(QuantifyingStrategies.ItemQuantifyingStrategy strategy, object source);
                        public abstract object LoadDataFor(SpatialStrategies.ItemSpatialStrategy strategy, object source);
                        public abstract object LoadDataFor(UsageStrategies.ItemUsageStrategy strategy, object source);
                        public abstract object LoadDataFor(RenderingStrategies.ItemRenderingStrategy strategy, object source);
                    }
                }
            }
        }
    }
}
