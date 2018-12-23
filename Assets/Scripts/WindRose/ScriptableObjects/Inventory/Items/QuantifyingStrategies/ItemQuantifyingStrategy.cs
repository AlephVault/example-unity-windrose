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
                namespace QuantifyingStrategies
                {
                    using Types.Inventory.Stacks.QuantifyingStrategies;

                    public abstract class ItemQuantifyingStrategy : ItemStrategy<StackQuantifyingStrategy>
                    {
                        /**
                         * Quantifying strategies tell us to what extent a stack of
                         *   this item may be created. Usual strategies will involve
                         *   non-stackability, integer-sized stacks, or float-sized
                         *   stacks. They may have a max limit (and they often will).
                         * 
                         * Since they are data bundles, they have no particular fields.
                         *  They will have just one method to create a compatible stack
                         *   rendering strategy instance, which must be implemented.
                         *   Logic should (will) be present in the stack quantifying
                         *   counterpart strategy.
                         * 
                         * Only one quantifying strategy is allowed on an item.
                         *   Quantifying strategies will have no dependencies.
                         */

                        public abstract StackQuantifyingStrategy CreateStackStrategy(object quantity);
                    }
                }
            }
        }
    }
}
