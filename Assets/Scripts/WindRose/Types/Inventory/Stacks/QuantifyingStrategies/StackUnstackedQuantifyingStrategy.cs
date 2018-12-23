using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindRose.ScriptableObjects.Inventory.Items.QuantifyingStrategies;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace QuantifyingStrategies
                {
                    class StackUnstackedQuantifyingStrategy : StackQuantifyingStrategy
                    {
                        public StackUnstackedQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy) : base(itemStrategy, null)
                        {
                        }

                        public override bool WillSaturate(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft)
                        {
                            // This call always saturates!
                            finalQuantity = Quantity;
                            quantityAdded = null;
                            quantityLeft = quantity;
                            return true;
                        }

                        protected override Type GetAllowedQuantityType()
                        {
                            // Type is not used here
                            return null;
                        }

                        protected override void CheckQuantityType(object quantity)
                        {
                            if (quantity != null)
                            {
                                throw new InvalidQuantityType("Given quantity must be null");
                            }
                        }

                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            return quantity == null;
                        }

                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            // This will only be allowed for null quantity
                            return false;
                        }

                        protected override bool IsFullQuantity(object quantity)
                        {
                            // This will only be allowed for null quantity
                            return true;
                        }

                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            // This will only account for null quantity.
                            // Quantity will be null by default, and delta will be ignored here.
                            return null;
                        }

                        protected override object QuantitySub(object quantity, object delta)
                        {
                            // This will only account for null quantity.
                            // Quantity will be null by default, and delta will be ignored here.
                            return null;
                        }
                    }
                }
            }
        }
    }
}
