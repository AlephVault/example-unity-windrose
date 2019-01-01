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

                        public override bool WillOverflow(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft)
                        {
                            // If the current quantity and the new quantity are both true
                            //   then it saturates. If the new quantity is false, no saturation
                            //   occurs.
                            //
                            // Otherwise, we just add the quantity, leave no quantity, and
                            //   never saturate.
                            if ((bool)Quantity)
                            {
                                finalQuantity = Quantity;
                                quantityAdded = false;
                                // The remainder quantity... is the input quantity.
                                quantityLeft = quantity;
                                return (bool)quantity;
                            }
                            else
                            {
                                finalQuantity = quantity;
                                quantityAdded = quantity;
                                quantityLeft = false;
                                return false;
                            }
                        }

                        public override bool Saturate()
                        {
                            Quantity = true;
                            return true;
                        }

                        protected override Type GetAllowedQuantityType()
                        {
                            // Type is bool: we'd use a flag to tell whether
                            //   the stack is empty (false) or full (true).
                            return typeof(bool);
                        }

                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            // Since the quantity is bool (the type was validated
                            //   beforehand), we need no further checks.
                            return true;
                        }

                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            // `false` is the empty quantity.
                            return (bool)quantity == false;
                        }

                        protected override bool IsFullQuantity(object quantity)
                        {
                            // `true` is the full quantity.
                            return (bool)quantity == true;
                        }

                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            // Adding is done with the OR operator.
                            return (bool)quantity || (bool)delta;
                        }

                        protected override object QuantitySub(object quantity, object delta)
                        {
                            // Subtracting is done as follows: if delta is true, the result is false.
                            // Otherwise, the result is the given input quantity.
                            return (bool)quantity && !(bool)delta;
                        }
                    }
                }
            }
        }
    }
}
