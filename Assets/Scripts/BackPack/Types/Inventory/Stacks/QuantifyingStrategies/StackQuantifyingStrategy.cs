using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BackPack
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                namespace QuantifyingStrategies
                {
                    using ScriptableObjects.Inventory.Items.QuantifyingStrategies;

                    /// <summary>
                    ///   Stack quantifying strategies are related to item quantifying strategies and
                    ///     allow us to perform several checks on quantities for the different stack
                    ///     operations that involve them.
                    /// </summary>
                    public abstract class StackQuantifyingStrategy : StackStrategy<ItemQuantifyingStrategy>
                    {
                        /// <summary>
                        ///   Exception class to be raised when operating with
                        ///     quantities in an invalid way.
                        /// </summary>
                        public class InvalidQuantityType : Exception
                        {
                            public InvalidQuantityType(string message) : base(message) { }
                        }

                        /// <summary>
                        ///   The allowed quantity value's type for this strategy.
                        /// </summary>
                        public Type AllowedQuantityType
                        {
                            get; private set;
                        }

                        /// <summary>
                        ///   Fetches the allowed quantity value's type for this strategy.
                        /// </summary>
                        /// <returns>The allowed type</returns>
                        protected abstract Type GetAllowedQuantityType();

                        /// <summary>
                        ///   Checks whether a given quantity respects the allowed type or not.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <exception cref="InvalidQuantityType">This happens when the quantity is not allowed</exception>
                        protected virtual void CheckQuantityType(object quantity)
                        {
                            if (!GMM.Utils.Classes.IsSameOrSubclassOf(quantity.GetType(), AllowedQuantityType))
                            {
                                throw new InvalidQuantityType(string.Format("Given quantity's type for stack quantifying strategy must be an instance of {0}", AllowedQuantityType.FullName));
                            }
                        }

                        private void PrepareAllowedQuantityType()
                        {
                            if (AllowedQuantityType != null)
                            {
                                return;
                            }

                            AllowedQuantityType = GetAllowedQuantityType();
                        }

                        public StackQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy)
                        {
                            PrepareAllowedQuantityType();
                            CheckQuantityType(argument);
                            Quantity = argument;
                        }

                        /// <summary>
                        ///   The current quantity.
                        /// </summary>
                        public object Quantity
                        {
                            get; protected set;
                        }

                        /// <summary>
                        ///   Tells whether the quantity is allowed (e.g. bounded into the max).
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether it is allowed or not</returns>
                        protected abstract bool IsAllowedQuantity(object quantity);
                        /// <summary>
                        ///   Zero-checks the quantity. Empty stacks are destroyed by the inventory.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether is zero/empty</returns>
                        protected abstract bool IsEmptyQuantity(object quantity);
                        /// <summary>
                        ///   Full-checks the quanttiy.
                        /// </summary>
                        /// <param name="quantity">The quantity to check</param>
                        /// <returns>Whether is full</returns>
                        protected abstract bool IsFullQuantity(object quantity);
                        /// <summary>
                        ///   Performs addition of quantities. This is the arithmetical addition for
                        ///     the arbitrary type to be implemented.
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to add</param>
                        /// <returns>The result after the addition</returns>
                        /// <remarks>No side effect occurs in this method</remarks>
                        protected abstract object QuantityAdd(object quantity, object delta);
                        /// <summary>
                        ///   Performs addition of quantities. This is the arithmetical subtraction for
                        ///     the arbitrary type to be implemented.
                        /// </summary>
                        /// <param name="quantity">Source quantity</param>
                        /// <param name="delta">Delta to subtract</param>
                        /// <returns>The result after the subtraction</returns>
                        /// <remarks>No side effect occurs on this method</remarks>
                        protected abstract object QuantitySub(object quantity, object delta);
                        /**
                         * Calculates the quantity that cannot be held by this object.
                         * It will be taken into account:
                         * - The current quantity.
                         * - The given quantity.
                         * You will get the following out values:
                         * - The quantity that would be effectively added (between 0 and quantity).
                         * - The quantity that whould not be added (between 0 and quantity, as well).
                         * - The final quantity (it could be understood as the minimum between the max capacity and quantity+object)
                         */
                        /// <summary>
                        ///   <para>
                        ///     Tells whether this strategy would overflow its maximum if we try
                        ///       adding a specific (always counting as positive!) quantity.
                        ///   </para>
                        ///   <para>
                        ///     If the hypothetical result goes strictly above the maximum, this function
                        ///       returns <c>true</c> and the output arguments will hold the final quantity
                        ///       to the maximum, the quantity added to the one strictly added to the current
                        ///       quantity, and the quantity not added. Otherwise, the output arguments will
                        ///       hold: the full addition, the full <paramref name="quantity"/> value, and
                        ///       0 (or the corresponding zero value) respectively.
                        ///   </para>
                        /// </summary>
                        /// <param name="quantity">The quantity to add</param>
                        /// <param name="finalQuantity">The quantity that would remain on the strategy</param>
                        /// <param name="quantityAdded">The quantity that would be effectively added</param>
                        /// <param name="quantityLeft">The quantity that would be not added</param>
                        /// <returns>Whether it would overflow or not</returns>
                        public abstract bool WillOverflow(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft);

                        /// <summary>
                        ///   Tries to cap the quantity value.
                        /// </summary>
                        /// <returns>
                        ///   Returns <c>true</c> if the quantity could be set to its maximum, or <c>false</c>
                        ///     if this strategy instance has no maximum.
                        /// </returns>
                        public abstract bool Saturate();

                        /// <summary>
                        ///   Tells whether the current strategy has a valid quantity or not.
                        /// </summary>
                        public bool HasAllowedQuantity()
                        {
                            return IsAllowedQuantity(Quantity);
                        }

                        /// <summary>
                        ///   Tells whether the current strategy has an empty quantity or not.
                        /// </summary>
                        public bool IsEmpty()
                        {
                            return IsEmptyQuantity(Quantity);
                        }

                        /// <summary>
                        ///   Tells whether the current strategy has a full quantity or not.
                        /// </summary>
                        public bool IsFull()
                        {
                            return IsFullQuantity(Quantity);
                        }

                        /// <summary>
                        ///   <para>
                        ///     Tries changing the quantity to a new one. The quantity must be of the allowed type
                        ///       and also allowed in value.
                        ///   </para>
                        ///   <para>
                        ///     if <paramref name="disallowEmpty"/> is true, the quantity must NOT be empty.
                        ///   </para>
                        /// </summary>
                        /// <param name="quantity">The new quantity to set</param>
                        /// <param name="disallowEmpty">Whether to allow zero quantity or not</param>
                        /// <returns>Whether the quantity could be set or not</returns>
                        /// <exception cref="InvalidQuantityType">If the quantity is not of the allowed type</exception>
                        public bool ChangeQuantityTo(object quantity, bool disallowEmpty)
                        {
                            CheckQuantityType(quantity);
                            if (IsAllowedQuantity(quantity) && !(disallowEmpty && IsEmptyQuantity(quantity)))
                            {
                                Quantity = quantity;
                                return true;
                            }
                            return false;
                        }

                        /// <summary>
                        ///   <para>
                        ///     Tries changing the quantity to a new one by specifying a delta. The delta must be
                        ///       something that produces a valid value after arithmetical addition (see
                        ///       <see cref="QuantityAdd(object, object)"/>) or arithmetical subtraction (see
                        ///       <see cref="QuantitySub(object, object)"/>). Such arithmetical result must be
                        ///       of valid type and also allowed in value.
                        ///   </para>
                        ///   <para>
                        ///     If the result is empty and <paramref name="disallowEmpty"/> is true, the change
                        ///       will not be done.
                        ///   </para>
                        /// </summary>
                        /// <param name="delta">The quantity to add or subtract</param>
                        /// <param name="disallowEmpty">Whether to allow zero quantity or not</param>
                        /// <returns>Whether the quantity could be changed or not</returns>
                        /// <exception cref="InvalidQuantityType">If any quantity is not of the allowed type</exception>
                        public bool ChangeQuantityBy(object delta, bool subtract, bool disallowEmpty)
                        {
                            return ChangeQuantityTo(subtract ? QuantitySub(Quantity, delta) : QuantityAdd(Quantity, delta), disallowEmpty);
                        }

                        /// <summary>
                        ///   Creates a new strategy instance with the same item strategy but different quantity.
                        /// </summary>
                        /// <param name="quantity">The quantity to use for the new strategy instance</param>
                        /// <returns>The new strategy instance</returns>
                        public StackQuantifyingStrategy Clone(object quantity)
                        {
                            return ItemStrategy.CreateStackStrategy(quantity);
                        }

                        /// <summary>
                        ///   Creates a new strategy instance with the same item strategy and quantity.
                        /// </summary>
                        /// <returns>The new strategy instance</returns>
                        public StackQuantifyingStrategy Clone()
                        {
                            return Clone(Quantity);
                        }
                    }
                }
            }
        }
    }
}