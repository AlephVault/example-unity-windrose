﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace DataDumpingStrategies
                {
                    using ScriptableObjects.Inventory.Items.DataLoadingStrategies;

                    public abstract class DataDumpingStrategy : StackStrategy<DataLoadingStrategy>
                    {
                        /**
                         * This strategy is the counterpart of DataLoadingStrategy
                         *   for the items and, in the same way, it is not a normal
                         *   strategy, but you must instead create one for each
                         *   combination of other strategies.
                         * 
                         * This strategy will take the exported data of the strategies
                         *   and, depending on the type of the strategies, populate
                         *   the target object accordingly. The stack object will
                         *   make use of those methods to populate the final object
                         *   accordingly on export.
                         * 
                         * All the methods must be implemented, with appropriate type
                         *   checks and casts.
                         */

                        // This constructor will do nothing by itself, aside for calling former constructors.
                        // Data will seldom be accessed/parsed from the `arguments` param.
                        public DataDumpingStrategy(DataLoadingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        public abstract object DumpDataFor(QuantifyingStrategies.StackQuantifyingStrategy strategy, object exported, object target);
                        public abstract object DumpDataFor(SpatialStrategies.StackSpatialStrategy strategy, object exported, object target);
                        public abstract object DumpDataFor(UsageStrategies.StackUsageStrategy strategy, object exported, object target);
                        public abstract object DumpDataFor(RenderingStrategies.StackRenderingStrategy strategy, object exported, object target);
                    }
                }
            }
        }
    }
}
