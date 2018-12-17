using System;
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
                namespace QuantifyingStrategies
                {
                    using ScriptableObjects.Inventory.Items.QuantifyingStrategies;

                    public abstract class StackQuantifyingStrategy : StackStrategy<ItemQuantifyingStrategy>
                    {
                        /**
                         * This stack strategy is related to an ItemQuantifyingStrategy.
                         * 
                         * It will add a method: HasValidQuantity(). This method will determine
                         *   whether this stack strategy has a valid quantity with respect to
                         *   the underlying item. The method is abstract.
                         */

                        public class InvalidQuantityType : Exception
                        {
                            public InvalidQuantityType(string message) : base(message) { }
                        }

                        /**
                         * Compatibility stuff for quantity types.
                         */

                        private Type allowedQuantityType;

                        // Notes: the expected type for non-stackable will be: object.
                        protected abstract Type GetAllowedQuantityType();

                        protected virtual void CheckQuantityType(object quantity)
                        {
                            if (!quantity.GetType().IsSubclassOf(allowedQuantityType))
                            {
                                throw new InvalidQuantityType(string.Format("Given quantity's type for stack quantifying strategy must be an instance of {}", allowedQuantityType.FullName));
                            }
                        }

                        private void PrepareAllowedQuantityType()
                        {
                            if (allowedQuantityType != null)
                            {
                                return;
                            }

                            allowedQuantityType = GetAllowedQuantityType();
                        }

                        public StackQuantifyingStrategy(ItemQuantifyingStrategy itemStrategy, object argument) : base(itemStrategy, argument)
                        {
                            PrepareAllowedQuantityType();
                        }

                        protected override void Import(object argument)
                        {
                            // Notes: the non-stackable one will override all this to check for null instead, and assign then.
                            CheckQuantityType(argument);
                            Quantity = argument;
                        }

                        public object Quantity
                        {
                            get; protected set;
                        }

                        public override object Export()
                        {
                            return Quantity;
                        }

                        /**
                         * Allowed should involve terms of stacking strategy. Usually:
                         *   >= 0 && <= MAX.
                         */
                        protected abstract bool IsAllowedQuantity(object quantity);
                        /**
                         * Emptiness will involve == 0. An object being detected as empty will
                         *   be usually destroyed by its inventory.
                         */
                        protected abstract bool IsEmptyQuantity(object quantity);
                        /**
                         * Fullness will involve == MAX. For unstackable quantifying strategy,
                         *   it will always be full.
                         */
                        protected abstract bool IsFullQuantity(object quantity);
                        protected abstract object QuantityAdd(object quantity, object delta);
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
                        public abstract bool WillSaturate(object quantity, out object finalQuantity, out object quantityAdded, out object quantityLeft);

                        /**
                         * These methods involve checking/changing the quantity of a stack quantifying strategy.
                         */

                        public bool HasAllowedQuantity()
                        {
                            return IsAllowedQuantity(Quantity);
                        }

                        public bool IsEmpty()
                        {
                            return IsEmptyQuantity(Quantity);
                        }

                        public bool IsFull()
                        {
                            return IsFullQuantity(Quantity);
                        }

                        public bool ChangeQuantityTo(object quantity, bool disallowEmpty)
                        {
                            CheckQuantityType(quantity);
                            if (IsAllowedQuantity(quantity) && !(disallowEmpty || IsEmptyQuantity(quantity)))
                            {
                                Quantity = quantity;
                                return true;
                            }
                            return false;
                        }

                        public bool ChangeQuantityBy(object delta, bool subtract, bool disallowEmpty)
                        {
                            return ChangeQuantityTo(subtract ? QuantitySub(Quantity, delta) : QuantityAdd(Quantity, delta), disallowEmpty);
                        }
                        
                        /**
                         * Creates a new stack quantifying strategy as a clone of this one except
                         *   for the quantity - a new one is specified. Useful for splitting.
                         */
                        public StackQuantifyingStrategy Clone(object quantity)
                        {
                            return ItemStrategy.CreateStackStrategy(quantity);
                        }

                        /**
                         * Creates a new stack quantifying strategy as a clone of this one except
                         *   for the quantity - a new one is specified. Useful for cloning.
                         */
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