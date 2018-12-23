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
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks.RenderingStrategies;

                    public abstract class ItemRenderingStrategy : ItemStrategy<StackRenderingStrategy>
                    {
                        /**
                         * Rendering strategies are data bundles being read from the
                         *   stacks when they need to be refreshed in an inventory
                         *   renderer. Actually, they will seldom hold own data.
                         *   They will no need any data for initialization since
                         *   they rely on other data.
                         * 
                         * Since they are data bundles, they have no particular fields.
                         *   They will have just one method to create a compatible stack
                         *   rendering strategy instance, which must be implemented.
                         *   Logic should (will) be present in the stack rendering
                         *   counterpart strategy.
                         * 
                         * The same item may have more than one rendering strategy.
                         *   The stacks will have related stack strategies (one for
                         *   each strategy in the item) and in both cases the added
                         *   strategies will depend among themselves and will gather
                         *   the data through flattened / ordered iteration when the
                         *   need to be rendered is present.
                         * 
                         * Rendering strategies may depend on:
                         * - Quantifying strategies.
                         * - State strategies.
                         * - Other rendering strategies.
                         */

                        public abstract StackRenderingStrategy CreateStackStrategy();
                    }
                }
            }
        }
    }
}
