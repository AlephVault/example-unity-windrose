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
                    class StackIntegerQuantifyingStrategy : StackQuantifyingStrategy
                    {
                        public StackIntegerQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        public override bool WillOverflow(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft)
                        {
                            CheckQuantityType(quantity);
                            int quantityToAdd = ((int)quantity);
                            int currentQuantity = ((int)Quantity);
                            int maxQuantity = ((ItemIntegerQuantifyingStrategy)ItemStrategy).Max;

                            if (quantityToAdd <= 0)
                            {
                                // negative amounts will be discarded as 0
                                finalQuantity = currentQuantity;
                                quantityAdded = 0;
                                quantityLeft = 0;
                                return false;
                            }
                            else if (maxQuantity == 0)
                            {
                                finalQuantity = currentQuantity + quantityToAdd;
                                quantityAdded = quantityToAdd;
                                quantityLeft = 0;
                                return false;
                            }
                            else
                            {
                                int potentialQuantity = currentQuantity + quantityToAdd;
                                if (potentialQuantity > maxQuantity)
                                {
                                    finalQuantity = maxQuantity;
                                    quantityAdded = maxQuantity - currentQuantity;
                                    quantityLeft = potentialQuantity - maxQuantity;
                                    return true;
                                }
                                else
                                {
                                    finalQuantity = potentialQuantity;
                                    quantityAdded = quantityToAdd;
                                    quantityLeft = 0;
                                    return false;
                                }
                            }
                        }

                        public override bool Saturate()
                        {
                            int maxQuantity = ((ItemIntegerQuantifyingStrategy)ItemStrategy).Max;
                            if (maxQuantity == 0)
                            {
                                return false;
                            }
                            else
                            {
                                Quantity = maxQuantity;
                                return true;
                            }
                        }

                        protected override Type GetAllowedQuantityType()
                        {
                            return typeof(int);
                        }

                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            int q = (int)quantity;
                            return q >= 0 && q <= ((ItemIntegerQuantifyingStrategy)ItemStrategy).Max;
                        }

                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            return ((int)quantity) == 0;
                        }

                        protected override bool IsFullQuantity(object quantity)
                        {
                            return ((int)quantity) == ((ItemIntegerQuantifyingStrategy)ItemStrategy).Max;
                        }

                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            return (int)quantity + (int)delta;
                        }

                        protected override object QuantitySub(object quantity, object delta)
                        {
                            return (int)quantity - (int)delta;
                        }
                    }
                }
            }
        }
    }
}
