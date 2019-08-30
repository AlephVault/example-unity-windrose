using System;

namespace BackPack
{
	using ScriptableObjects.Inventory.Items.QuantifyingStrategies;

    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace QuantifyingStrategies
                {
                    /// <summary>
                    ///   Float quantifying strategies check against float quantity values.
                    ///   The main element to consider here is the <see cref="ItemFloatQuantifyingStrategy.Max"/>
                    ///     property to compare against.
                    /// </summary>
                    class StackFloatQuantifyingStrategy : StackQuantifyingStrategy
                    {
                        public StackFloatQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                        }

                        /// <summary>
                        ///   Computes overflow involving the maximum quantity of the related item
                        ///     strategy: it will occur when current + new >= max.
                        /// </summary>
                        /// <param name="quantity">The quantity to add</param>
                        /// <param name="finalQuantity">The quantity that would remain on the strategy</param>
                        /// <param name="quantityAdded">The quantity that would be effectively added</param>
                        /// <param name="quantityLeft">The quantity that would be not added</param>
                        /// <returns>Whether it would overflow or not</returns>
                        public override bool WillOverflow(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft)
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
                            else if (maxQuantity == 0)
                            {
                                finalQuantity = currentQuantity + quantityToAdd;
                                quantityAdded = quantityToAdd;
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

                        /// <summary>
                        ///   Pushes the current quantity to the maximum (if set) in the related
                        ///     strategy, or fails.
                        /// </summary>
                        /// <returns>
                        ///   Returns <c>true</c> if the quantity could be set to its maximum, or <c>false</c>
                        ///     if this strategy instance has no maximum.
                        /// </returns>
                        public override bool Saturate()
                        {
                            float maxQuantity = ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;
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

                        /// <summary>
                        ///   Allowed quantity type here is <c>float</c>.
                        /// </summary>
                        /// <returns>The allowed type</returns>
                        protected override Type GetAllowedQuantityType()
                        {
                            return typeof(float);
                        }

                        /// <summary>
                        ///   Tells whether the quantity is allowed (bounded between
                        ///     0 and max) or not.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is allowed or not</returns>
                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            float q = (float)quantity;
                            float max = ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;
                            return q >= 0 && (max == 0 || q <= max);
                        }

                        /// <summary>
                        ///   Tells whether the quantity is zero or not.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is empty or not</returns>
                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            return ((float)quantity) == 0;
                        }

                        /// <summary>
                        ///   Tells whether the quantity is the max or not.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is full or not</returns>
                        protected override bool IsFullQuantity(object quantity)
                        {
                            float max = ((ItemFloatQuantifyingStrategy)ItemStrategy).Max;
                            return max != 0 && ((float)quantity) == max;
                        }

                        /// <summary>
                        ///   Performs the addition using standard arithmetical operations on float
                        ///     values.
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to add</param>
                        /// <returns>The result after the addition</returns>
                        /// <remarks>No side effect occurs in this method</remarks>
                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            return (float)quantity + (float)delta;
                        }

                        /// <summary>
                        ///   Performs the subtraction using standard arithmetical operations on float
                        ///     values.
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to subtract</param>
                        /// <returns>The result after the subtraction</returns>
                        /// <remarks>No side effect occurs on this method</remarks>
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
