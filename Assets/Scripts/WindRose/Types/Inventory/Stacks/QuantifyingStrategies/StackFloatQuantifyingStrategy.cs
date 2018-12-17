using System;
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
                    class StackFloatQuantifyingStrategy : StackQuantifyingStrategy
                    {
                        public StackFloatQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        public override bool WillSaturate(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft)
                        {
                            CheckQuantityType(quantity);
                            float quantityToAdd = ((float)quantity);
                            float currentQuantity = ((float)Quantity);
                            float maxQuantity = ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;

                            if (quantityToAdd <= 0)
                            {
                                // negative amounts will be discarded as 0
                                finalQuantity = currentQuantity;
                                quantityAdded = 0;
                                quantityLeft = 0;
                                return false;
                            }
                            else
                            {
                                float potentialQuantity = currentQuantity + quantityToAdd;
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

                        protected override Type GetAllowedQuantityType()
                        {
                            return typeof(float);
                        }

                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            float q = (float)quantity;
                            return q >= 0 && q <= ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;
                        }

                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            return ((float)quantity) == 0;
                        }

                        protected override bool IsFullQuantity(object quantity)
                        {
                            return ((float)quantity) == ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;
                        }

                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            return (float)quantity + (float)delta;
                        }

                        protected override object QuantitySub(object quantity, object delta)
                        {
                            return (float)quantity - (float)delta;
                        }
                    }
                }
            }
        }
    }
}
