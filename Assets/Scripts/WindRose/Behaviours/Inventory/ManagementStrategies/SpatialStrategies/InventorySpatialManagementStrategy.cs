using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace SpatialStrategies
                {
                    using Types.Inventory.Stacks;
                    using ScriptableObjects.Inventory.Items;
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;

                    public abstract class InventorySpatialManagementStrategy : InventoryManagementStrategy
                    {
                        /**
                         * Manages the position of the elements in the inventory. One example could involve mantaining an R-tree
                         *   or an indexed array to map for matrix-inventories or a linear one. It doesn't just map the appropriate
                         *   position (array index, string position, x/y coordinate in matrix) but also mantains the reference to
                         *   the packs being initialized. Several elements will be involved here:
                         * 
                         * 1. This strategy, which belongs to an inventory manager.
                         * 2. SpatialContainer: This is an abstract class (thus making this a sort of abstract factory) that
                         *    will appropriately map, restrict, translate an arbitrary position to a stored item. Each subclass
                         *    will handle a particular (arbitrary) data type acting as position.
                         * 3. A position: This is an arbitrary data type. An integer, a string, a custom structure resembling
                         *    a byte-sized pair of (x, y) elements,...
                         * 4. A stack: Our object of interest.
                         * 
                         * A position manager will be given to the stack(s) to let them know its container and its position.
                         *   However: the same will be managed by this strategy. In the same way, this strategy must know how to
                         *   parse a position and use the appropriate class to associate that position to a new spatial container,
                         *   which will correspond to particular-respective child strategies.
                         */

                        /**
                         * This class just keeps track of the position value and its container, for a specific item strategy.
                         * The latter is just illustrative: It is a way to say: "I am at position X in container Y, but referring
                         *   to the spatial strategy Z I have among my strategies."
                         */
                        public class QualifiedStackPosition : Support.Types.Tuple<object, ItemSpatialStrategy, SpatialContainer>
                        {
                            public QualifiedStackPosition(object position, ItemSpatialStrategy itemStrategy, SpatialContainer container) : base(position, itemStrategy, container)
                            {
                            }
                        }

                        private static PropertyInfo positionProperty = typeof(Stack).GetProperty("QualifiedPosition");

                        private static void SetPosition(Stack stack, QualifiedStackPosition position)
                        {
                            positionProperty.SetValue(stack, position, null);
                        }

                        public class InvalidItemSpatialStrategyCounterpartType : Types.Exception
                        {
                            public InvalidItemSpatialStrategyCounterpartType(string message) : base(message) { }
                        }

                        public class MissingExpectedItemSpatialStrategyCounterpartType : Types.Exception
                        {
                            public MissingExpectedItemSpatialStrategyCounterpartType(string message) : base(message) { }
                        }

                        public class SpatialContainerDoesNotExist : Types.Exception
                        {
                            public readonly object Position;

                            public SpatialContainerDoesNotExist(object position) : base(string.Format("No spatial container at position: {0}", position))
                            {
                                Position = position;
                            }
                        }

                        public abstract class SpatialContainer
                        {
                            /**
                             * Spatial containers will initialize the workspace of your stacks.
                             * 
                             * They will be initialized differently and that will depend exclusively on the spatial strategy.
                             * They will have such strategy as argument and will fetch needed (settings) data from it.
                             * 
                             * They will belong to a in-inventory contaienr position, but that data will just be informative.
                             * They have nothing to do with such position anyway DIRECTLY, but custom item/stack logic may
                             *   have something to say and thus the data is potentially needed/useful.
                             */

                            public enum StackPositionValidity { Valid, InvalidType, InvalidValue, OutOfBounds }

                            public class SpatialContainerException : Types.Exception
                            {
                                public SpatialContainerException(string message) : base(message) { }
                            }

                            public class InvalidPositionException : SpatialContainerException
                            {
                                /**
                                 * This class tells that a given position is not valid on this spatial
                                 *   container. 
                                 */

                                public readonly StackPositionValidity ErrorType;

                                public InvalidPositionException(string message, StackPositionValidity errorType) : base(message) {
                                    ErrorType = errorType;
                                }
                            }

                            public class UnavailablePositionException : SpatialContainerException
                            {
                                public UnavailablePositionException(string message) : base(message) { }
                            }

                            public class StackAlreadyBelongsHereException : SpatialContainerException
                            {
                                public StackAlreadyBelongsHereException(string message) : base(message) { }
                            }

                            public class StackDoesNotBelongHereException : SpatialContainerException
                            {
                                public StackDoesNotBelongHereException(string message) : base(message) { }
                            }

                            /**
                             * Position of this container inside its Spatial Strategy. This value is only informative,
                             *   and will only be useful for display renderers.
                             */
                            public readonly object Position;
                            public readonly InventorySpatialManagementStrategy SpatialStrategy;
                            private Dictionary<object, Stack> stacks = new Dictionary<object, Stack>();

                            public SpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position)
                            {
                                SpatialStrategy = spatialStrategy;
                                Position = position;
                            }

                            /**
                             * Validates whether a position is VALID in terms of type, value, and bounds.
                             * Bounds, in this context, will relate to settings and also to the given stack, if any.
                             * There are certain situations where the stack may be null. This situation is useful
                             *   to the Search semantic. Mutating semantics will require the stack, so it will not
                             *   be null there.
                             */
                            protected abstract StackPositionValidity ValidateStackPosition(object position, Stack stack);
                            /**
                             * This method has to behave as follows:
                             * - Considering whether the stack is added, or not, to this container.
                             *   If the stack is added, we should exclude its position/dimensions. This happend because
                             *     in this case, the stack is one being MOVED, not added.
                             * - The position was already validated beforehand.
                             */
                            protected abstract bool StackPositionIsAvailable(object position, Stack stack);
                            /**
                             * This function is used to map a position against a stack index. To this point, the
                             *   position is considered valid and authorized, and the stack was added to (or MOVED
                             *   to, in the moving semantic) its new position. The previous position was released,
                             *   and the previous position was also released.
                             * 
                             * This function should not trigger any exception or veto operation.
                             */
                            protected abstract void Occupy(object position, Stack stack);
                            /**
                             * This function is used to release an existing position. You are guaranteed that the
                             *   given stack is the one being released from this inventory. You must get its
                             *   dimensions and combine them with the position to perform your calculations.
                             */
                            protected abstract void Release(object position, Stack stack);
                            /**
                             * This function is to get the contents of a particular position. The return value of
                             *   this method is the actual/registered position of a Stack, or null. Example:
                             * - For indexed inventories, the position will be an index that will be returned
                             *   as is if it is being used. Otherwise, null will be returned.
                             * - For dimensional/matrix inventories, the position will be (x, y), and a different
                             *   (x', y') may be returned. E.g. a query with (2, 2) may return (0, 0) if a stack
                             *   is located at (0, 0) and it occupies a size of (4, 4).
                             * 
                             * The resulting position must be canonical: We must be able to query stacks[pos] and
                             *   get something. Otherwise, the resulting position must be null.
                             */
                            protected abstract object Search(object position);
                            /**
                             * This function iterates over the registered positions. The order of iteration is up
                             *   to the implementor, so it will not be determined in a trivial fashion like, say,
                             *   iterating over the keys of the dictionary.
                             * 
                             * ALL THE POSITIONS SHOULD BE ITERATED FOR THE DISPLAYERS TO WORK PROPERLY. Every
                             *   position being iterated should be valid. This means: stack[position] must not
                             *   fail by absence.
                             */
                            protected abstract IEnumerable<object> Positions(bool reverse);

                            private void CheckValidStackPosition(object position, Stack stack)
                            {
                                StackPositionValidity validity = ValidateStackPosition(position, stack);
                                if (validity != StackPositionValidity.Valid)
                                {
                                    throw new InvalidPositionException(string.Format("Invalid position: {0}", validity), validity);
                                }
                            }

                            private void CheckStackDoesNotBelong(Stack stack)
                            {
                                if (stacks.ContainsValue(stack))
                                {
                                    throw new StackAlreadyBelongsHereException("Stack already belongs here");
                                }
                            }

                            /**
                             * Public methods start here.
                             */

                            /**
                             * Enumerates all the position/stack pairs.
                             */
                            public IEnumerable<Support.Types.Tuple<object, Stack>> StackPairs(bool reverse)
                            {
                                return from position in Positions(reverse) select new Support.Types.Tuple<object, Stack>(position, stacks[position]);
                            }

                            /**
                             * Finds a stack by checking certain position.
                             */
                            public Stack Find(object position)
                            {
                                object canonicalPosition = Search(position);
                                return canonicalPosition == null ? null : stacks[canonicalPosition];
                            }

                            /**
                             * Finds all stacks satisfying a predicate on its position and the stack.
                             * It may reverse the order of enumeration.
                             */
                            public IEnumerable<Stack> FindAll(Func<Support.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                            {
                                return from pair in StackPairs(reverse).Where(predicate) select pair.Second;
                            }

                            /**
                             * Finds all stacks having a particular item.
                             * It may reverse the order of enumeration.
                             */
                            public IEnumerable<Stack> FindAll(Item item, bool reverse)
                            {
                                return FindAll(delegate (Support.Types.Tuple<object, Stack> pair)
                                {
                                    return pair.Second.Item == item;
                                }, reverse);
                            }

                            /**
                             * Gets the first stack.
                             */
                            public Stack First()
                            {
                                return (from pair in StackPairs(false) select pair.Second).FirstOrDefault();
                            }

                            /**
                             * Gets the last stack.
                             */
                            public Stack Last()
                            {
                                return (from pair in StackPairs(true) select pair.Second).FirstOrDefault();
                            }

                            /**
                             * Finds all stacks exactly matching an external stack.
                             * It may reverse the order of enumeration.
                             */
                            public IEnumerable<Stack> FindAll(Stack stack, bool reverse)
                            {
                                return FindAll(delegate (Support.Types.Tuple<object, Stack> pair)
                                {
                                    return pair.Second.Equals(stack);
                                }, reverse);
                            }

                            /**
                             * Finds a stack satisfying a predicate on its position and the stack.
                             * It may reverse the order of enumeration to find the first one.
                             */
                            public Stack FindOne(Func<Support.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                            {
                                return FindAll(predicate, reverse).FirstOrDefault();
                            }

                            /**
                             * Finds a stack having a particular item.
                             * It may reverse the order of enumeration to find the first one.
                             */
                            public Stack FindOne(Item item, bool reverse)
                            {
                                return FindAll(item, reverse).FirstOrDefault();
                            }

                            /**
                             * Finds a stack exactly matching an external stack.
                              * It may reverse the order of enumeration to find the first one.
                            */
                            public Stack FindOne(Stack stack, bool reverse)
                            {
                                return FindAll(stack, reverse).FirstOrDefault();
                            }

                            /**
                             * Tries to find a first-match for the item. It must return null IF
                             *   AND ONLY IF the inventory is full (i.e. no position is available).
                             */
                            public abstract object FirstFree(Stack stack);

                            /**
                             * Puts a new (or moves an existing) stack in this container. It also sets
                             *   the position of the stack. We already know the itemStrategy will be
                             *   compatible with this strategy at this point.
                             * 
                             * Will return false if:
                             * - Position was given and is not available.
                             * - Position was not given and could not get a first-matched position
                             *     because the inventory is "full" for the given item.
                             */
                            public bool Put(object position, ItemSpatialStrategy itemStrategy, Stack stack, out object finalPosition)
                            {
                                finalPosition = null;

                                if (position != null)
                                {
                                    CheckValidStackPosition(position, stack);
                                }
                                else
                                {
                                    position = FirstFree(stack);
                                    if (position == null) return false;
                                }

                                if (!StackPositionIsAvailable(position, stack)) return false;
                                if (stacks.ContainsValue(stack))
                                {
                                    Release(stack.QualifiedPosition.First, stack);
                                    stacks.Remove(stack.QualifiedPosition.First);
                                }
                                Occupy(position, stack);
                                stacks[position] = stack;
                                SetPosition(stack, new QualifiedStackPosition(position, itemStrategy, this));
                                finalPosition = position;
                                return true;
                            }

                            /**
                             * Removes a stack from this container. It also cleans up the position of
                             *   the stack.
                             */
                            public bool Remove(Stack stack)
                            {
                                if (!stacks.ContainsValue(stack)) return false;
                                Release(stack.QualifiedPosition.First, stack);
                                stacks.Remove(stack.QualifiedPosition.First);
                                SetPosition(stack, null);
                                return true;
                            }

                            /**
                             * Tells how many stacks does this container have.
                             */
                            public int Count { get { return stacks.Count; } }
                        }

                        public class NullSpatialContainer : SpatialContainer
                        {
                            /**
                             * Implements an always-empty container that fails to everything.
                             *   This will act as a null spatial container, in order to not
                             *   just return `null` and check it but, instead, act transparently.
                             */

                            public NullSpatialContainer() : base(null, null)
                            {
                            }

                            public override object FirstFree(Stack stack)
                            {
                                return null;
                            }

                            protected override void Occupy(object position, Stack stack)
                            {
                            }

                            protected override IEnumerable<object> Positions(bool reverse)
                            {
                                yield break;
                            }

                            protected override void Release(object position, Stack stack)
                            {
                            }

                            protected override object Search(object position)
                            {
                                return null;
                            }

                            protected override bool StackPositionIsAvailable(object position, Stack stack)
                            {
                                return false;
                            }

                            protected override StackPositionValidity ValidateStackPosition(object position, Stack stack)
                            {
                                return StackPositionValidity.OutOfBounds;
                            }
                        }

                        private static NullSpatialContainer nullSpatialContainer = new NullSpatialContainer();

                        public Type ItemSpatialStrategyCounterpartType { get; private set; }

                        /**
                         * You must implement this: Initializes a new contianer, giving an
                         *   arbitrary position.
                         */
                        protected abstract SpatialContainer InitializeContainer(object position);

                        /**
                         * Keeps track of existing containers.
                         */
                        private Dictionary<object, SpatialContainer> containers = new Dictionary<object, SpatialContainer>();

                        private enum IfAbsent { Init, Null };
                    
                        /**
                         * Gets a container by position. The position will NOT be validated.
                         * Depending on the second parameter, when no stack container is at
                         *   the given position we will:
                         *   - Null: Return null.
                         *   - Init: Initialize a new container in that position and return it.
                         */
                        private SpatialContainer GetContainer(object position, IfAbsent ifAbsent)
                        {
                            SpatialContainer container;
                            try
                            {
                                container = containers[position];
                            }
                            catch(Exception)
                            {
                                switch(ifAbsent)
                                {
                                    case IfAbsent.Null:
                                        return nullSpatialContainer;
                                    default:
                                        // Init
                                        container = InitializeContainer(position);
                                        containers[position] = container;
                                        break;
                                }
                            }
                            return container;
                        }

                        /**
                         * You must implement this: Which one is the counterpart spatial strategy type in
                         *   the items.
                         */
                        protected abstract Type GetItemSpatialStrategyCounterpartType();

                        /**
                         * Gets the appropriate item spatial strategy, based on the counterpart type.
                         * RAISES AN EXCEPTION if no appropriate strategy is found.
                         */
                        private ItemSpatialStrategy GetItemSpatialStrategyCounterpart(Stack stack)
                        {
                            ItemSpatialStrategy spatialStrategy = stack.Item.GetSpatialStrategy(GetItemSpatialStrategyCounterpartType());
                            if (spatialStrategy == null)
                            {
                                throw new MissingExpectedItemSpatialStrategyCounterpartType(string.Format("The stack did not contain an item spatial strategy of type {0} in its underlying item", ItemSpatialStrategyCounterpartType.FullName));
                            }
                            return spatialStrategy;
                        }

                        protected void Awake()
                        {
                            ItemSpatialStrategyCounterpartType = GetItemSpatialStrategyCounterpartType();
                            if (!Support.Utils.Classes.IsSameOrSubclassOf(ItemSpatialStrategyCounterpartType, typeof(ItemSpatialStrategy)))
                            {
                                throw new InvalidItemSpatialStrategyCounterpartType(string.Format("The type returned by GetItemSpatialStrategyCounterpartType must be a subclass of {0}", typeof(ItemSpatialStrategy).FullName));
                            }
                        }

                        /**
                         * public methods accessing everything. ALL THE DOCUMENTED METHODS.
                         */

                        /**
                         * Enumerates all the position/stack pairs for the container in a given position.
                         */
                        public IEnumerable<Support.Types.Tuple<object, Stack>> StackPairs(object containerPosition, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).StackPairs(reverse);
                        }

                        /**
                         * Finds a stack by checking certain stack position for the container in a given position.
                         */
                        public Stack Find(object containerPosition, object stackPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).Find(stackPosition);
                        }

                        /**
                         * Finds all stacks satisfying a predicate on its position and the stack for the container in a given position.
                         */
                        public IEnumerable<Stack> FindAll(object containerPosition, Func<Support.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(predicate, reverse);
                        }

                        /**
                         * Finds all stacks having a particular item.
                         */
                        public IEnumerable<Stack> FindAll(object containerPosition, Item item, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(item, reverse);
                        }

                        /**
                         * Gets the first stack.
                         */
                        public Stack First(object containerPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).First();
                        }

                        /**
                         * Gets the last stack.
                         */
                        public Stack Last(object containerPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).Last();
                        }

                        /**
                         * Finds all stacks exactly matching an external stack.
                         */
                        public IEnumerable<Stack> FindAll(object containerPosition, Stack stack, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(stack, reverse);
                        }

                        /**
                         * Finds a stack satisfying a predicate on its position and the stack.
                         */
                        public Stack FindOne(object containerPosition, Func<Support.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(predicate, reverse);
                        }

                        /**
                         * Finds a stack having a particular item.
                         */
                        public Stack FindOne(object containerPosition, Item item, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(item, reverse);
                        }

                        /**
                         * Finds a stack exactly matching an external stack.
                         */
                        public Stack FindOne(object containerPosition, Stack stack, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(stack, reverse);
                        }

                        /**
                         * Puts a stack inside a specific container.
                         */
                        public bool Put(object containerPosition, object stackPosition, Stack stack, out object finalStackPosition)
                        {
                            SpatialContainer container = GetContainer(containerPosition, IfAbsent.Init);
                            bool couldAdd = false;
                            try
                            {
                                couldAdd = container.Put(stackPosition, GetItemSpatialStrategyCounterpart(stack), stack, out finalStackPosition);
                                return couldAdd;
                            }
                            finally
                            {
                                if (!couldAdd && container.Count == 0)
                                {
                                    containers.Remove(containerPosition);
                                }
                            }
                        }

                        /**
                         * Removes a stack inside a specific container.
                         */
                        public bool Remove(object containerPosition, object stackPosition)
                        {
                            SpatialContainer container = GetContainer(containerPosition, IfAbsent.Null);

                            Stack stack = GetContainer(containerPosition, IfAbsent.Null).Find(stackPosition);
                            if (stack == null)
                            {
                                return false;
                            }

                            bool result = container.Remove(stack);

                            if (container.Count == 0)
                            {
                                containers.Remove(containerPosition);
                            }

                            return result;
                        }

                        /**
                         * Clears everything.
                         */
                        public void Clear()
                        {
                            containers.Clear();
                        }
                    }
                }
            }
        }
    }
}
