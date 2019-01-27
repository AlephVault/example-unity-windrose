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
                    /// <summary>
                    ///   Unstacked quantifying strategies check against unstacked (boolean) quantity values.
                    /// </summary>
                    class StackUnstackedQuantifyingStrategy : StackQuantifyingStrategy
                    {
                        public StackUnstackedQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy) : base(itemStrategy, null)
                        {
                        }

                        /// <summary>
                        ///   <para>
                        ///     Computes overflow considering whether both quantities are true.
                        ///   </para>
                        ///   <para>
                        ///     User has to expect that each argument here is a boolean value, overflow
                        ///       is finding two <c>true</c> values, and quantity being added / left are
                        ///       boolean values as well: they serve well as stacks of 1 element.
                        ///   </para>
                        /// </summary>
                        /// <param name="quantity">The quantity to add</param>
                        /// <param name="finalQuantity">The quantity that would remain on the strategy</param>
                        /// <param name="quantityAdded">The quantity that would be effectively added</param>
                        /// <param name="quantityLeft">The quantity that would be not added</param>
                        /// <returns>Whether it would overflow or not</returns>
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

                        /// <summary>
                        ///   Sets the quantity to true.
                        /// </summary>
                        /// <returns>true - this call always succeeds</returns>
                        public override bool Saturate()
                        {
                            Quantity = true;
                            return true;
                        }

                        /// <summary>
                        ///   Allowed quantity type here is <c>bool</c>.
                        /// </summary>
                        /// <returns>The allowed type</returns>
                        protected override Type GetAllowedQuantityType()
                        {
                            // Type is bool: we'd use a flag to tell whether
                            //   the stack is empty (false) or full (true).
                            return typeof(bool);
                        }

                        /// <summary>
                        ///   Boolean values are allowed - no bound check here.
                        /// </summary>
                        /// <param name="quantity">The quantity to check </param>
                        /// <returns>true - this call always succeed</returns>
                        protected override bool IsAllowedQuantity(object quantity)
                        {
                            // Since the quantity is bool (the type was validated
                            //   beforehand), we need no further checks.
                            return true;
                        }

                        /// <summary>
                        ///   Checks whether the given quantity is false (empty).
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is empty or not</returns>
                        protected override bool IsEmptyQuantity(object quantity)
                        {
                            // `false` is the empty quantity.
                            return (bool)quantity == false;
                        }

                        /// <summary>
                        ///   Tells whether the quantity is true (full).
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is full or not</returns>
                        protected override bool IsFullQuantity(object quantity)
                        {
                            // `true` is the full quantity.
                            return (bool)quantity == true;
                        }

                        /// <summary>
                        ///   Performs the addition using OR operation (overflow is
                        ///     not checked here).
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to add</param>
                        /// <returns>The result after the addition</returns>
                        /// <remarks>No side effect occurs in this method</remarks>
                        protected override object QuantityAdd(object quantity, object delta)
                        {
                            // Adding is done with the OR operator.
                            return (bool)quantity || (bool)delta;
                        }

                        /// <summary>
                        ///   Performs the subtraction using logical difference.
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to subtract</param>
                        /// <returns>The result after the subtraction</returns>
                        /// <remarks>No side effect occurs on this method</remarks>
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
