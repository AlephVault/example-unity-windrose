using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BackPack
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

                    /// <summary>
                    ///   <para>
                    ///     Spatial management strategies manage the position of the elements in the containers. 
                    ///   </para>
                    ///   <para>
                    ///     There may be different implementations like standard indexed inventory (e.g. Baldur's Gate)
                    ///       or matrix inventory with items of different sizes (e.g. Neverwinter Nights).
                    ///   </para>
                    ///   <para>
                    ///     Spatial management strategies will involve handling a particular type of position values
                    ///       (e.g. integer index, or vector position), and a particular type of inner container, to
                    ///       run the appropriate logic to find or add items therein.
                    ///   </para>
                    /// </summary>
                    public abstract class InventorySpatialManagementStrategy : InventoryManagementStrategy
                    {
                        /// <summary>
                        ///   Represents the current position of a stack in terms of position value, spatial
                        ///     strategy being accounted in the underlying stack for this purpose, and the
                        ///     spatial container this position is valid in.
                        /// </summary>
                        public class QualifiedStackPosition : GMM.Types.Tuple<object, ItemSpatialStrategy, SpatialContainer>
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

                        /// <summary>
                        ///   Tells when the item strategy counterpart type chosen for the spatial management
                        ///     strategy is not a valid type (descending from <see cref="ItemSpatialStrategy"/>).
                        /// </summary>
                        public class InvalidItemSpatialStrategyCounterpartType : GMM.Types.Exception
                        {
                            public InvalidItemSpatialStrategyCounterpartType(string message) : base(message) { }
                        }

                        /// <summary>
                        ///   Tells when the item strategy does not have a spatial strategy component being
                        ///     of the compatible type (counterpart) of this strategy.
                        /// </summary>
						public class MissingExpectedItemSpatialStrategyCounterpartType : GMM.Types.Exception
                        {
                            public MissingExpectedItemSpatialStrategyCounterpartType(string message) : base(message) { }
                        }

                        /// <summary>
                        ///   Tells whether the specified spatial container ID does not belong to any existing
                        ///     spatial container in this strategy.
                        /// </summary>
						public class SpatialContainerDoesNotExist : GMM.Types.Exception
                        {
                            public readonly object Position;

                            public SpatialContainerDoesNotExist(object position) : base(string.Format("No spatial container at position: {0}", position))
                            {
                                Position = position;
                            }
                        }

                        /// <summary>
                        ///   Spatial containers are the actual workspaces of the stacks: stacks are added there.
                        /// </summary>
                        public abstract class SpatialContainer
                        {
                            /// <summary>
                            ///   Stack position validity errors (or success).
                            /// </summary>
                            public enum StackPositionValidity { Valid, InvalidType, InvalidValue, OutOfBounds }

                            /// <summary>
                            ///   Base class for errors regarding these containers.
                            /// </summary>
							public class SpatialContainerException : GMM.Types.Exception
                            {
                                public SpatialContainerException(string message) : base(message) { }
                            }

                            /// <summary>
                            ///   Tells when the given position is not valid for this spatial container
                            ///     type (e.g. invalid type, invalid value, or unbounded).
                            /// </summary>
                            public class InvalidPositionException : SpatialContainerException
                            {
                                /// <summary>
                                ///   The reason of invalidity.
                                /// </summary>
                                public readonly StackPositionValidity ErrorType;

                                public InvalidPositionException(string message, StackPositionValidity errorType) : base(message) {
                                    ErrorType = errorType;
                                }
                            }

                            /// <summary>
                            ///   Tells when a requested position (for insertion) is not available.
                            /// </summary>
                            public class UnavailablePositionException : SpatialContainerException
                            {
                                public UnavailablePositionException(string message) : base(message) { }
                            }

                            /// <summary>
                            ///   Tells whether the stack being added already belongs here.
                            /// </summary>
                            public class StackAlreadyBelongsHereException : SpatialContainerException
                            {
                                public StackAlreadyBelongsHereException(string message) : base(message) { }
                            }

                            /// <summary>
                            ///   Tells whether the stack being removed does not belong here.
                            /// </summary>
                            public class StackDoesNotBelongHereException : SpatialContainerException
                            {
                                public StackDoesNotBelongHereException(string message) : base(message) { }
                            }

                            /// <summary>
                            ///   The position of this container inside the inventory. Actually, this value
                            ///     is only useful in the context of rendering and is tightly related to
                            ///     <see cref="PositioningStrategies.InventoryPositioningManagementStrategy"/>.
                            /// </summary>
                            public readonly object Position;

                            /// <summary>
                            ///   The spatial management strategy owning this ccontainer.
                            /// </summary>
                            public readonly InventorySpatialManagementStrategy SpatialStrategy;
                            private Dictionary<object, Stack> stacks = new Dictionary<object, Stack>();

                            public SpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position)
                            {
                                SpatialStrategy = spatialStrategy;
                                Position = position;
                            }

                            /// <summary>
                            ///   Validates whether a position is VALID in terms of type, value, and bounds.
                            ///   Bounds, in this context, will relate to settings and also to the given stack, if any.
                            ///   There are certain situations where the stack may be null. This situation is useful
                            ///     to the Search semantic.Mutating semantics will require the stack, so it will not
                            ///     be null there.
                            /// </summary>
                            protected abstract StackPositionValidity ValidateStackPosition(object position, Stack stack);

                            /// <summary>
                            ///   This method has to behave as follows:
                            ///   <list type="bullet">
                            ///     <item>
                            ///       <description>
                            ///         Considering whether the stack is added, or not, to this container.
                            ///         If the stack is added, we should exclude its position/dimensions. This happend
                            ///           because in this case, the stack is one being MOVED, not added.
                            ///       </description>
                            ///     </item>
                            ///     <item>
                            ///       <description>
                            ///         The position was already validated beforehand.
                            ///       </description>
                            ///     </item>
                            ///   </list>
                            /// </summary>
                            protected abstract bool StackPositionIsAvailable(object position, Stack stack);

                            /// <summary>
                            ///   <para>
                            ///     This function is used to map a position against a stack index. To this point, the
                            ///       position is considered valid and authorized, and the stack was added to (or MOVED
                            ///       to, in the moving semantic) its new position.The previous position was released,
                            ///       and the previous position was also released.
                            ///   </para>
                            ///   <para>
                            ///     This function should not trigger any exception or veto operation.
                            ///   </para>
                            /// </summary>
                            protected abstract void Occupy(object position, Stack stack);
                            
                            /// <summary>
                            ///   This function is used to release an existing position. It is guaranteed that the
                            ///     given stack is the one being released from this inventory. To perform the
                            ///     appropriate calculations, both dimensions and position must be considered.
                            /// </summary>
                            protected abstract void Release(object position, Stack stack);

                            /// <summary>
                            ///   <para>
                            ///     This function is to get the contents of a particular position. The return value of
                            ///       this method is the actual/registered position of a Stack, or null. Example:
                            ///     <list type="bullet">
                            ///       <item>
                            ///         <description>
                            ///           For indexed inventories, the position will be an index that will be returned
                            ///             as is if it is being used. Otherwise, null will be returned.
                            ///         </description>
                            ///       </item>
                            ///       <item>
                            ///         <description>
                            ///           For dimensional/matrix inventories, the position will be (x, y), and a different
                            ///             (x', y') may be returned. E.g. a query with (2, 2) may return (0, 0) if a stack
                            ///             is located at (0, 0) and it occupies a size of (4, 4).
                            ///         </description>
                            ///       </item>
                            ///     </list>
                            ///   </para>
                            ///   <para>
                            ///     The resulting position must be canonical: We must be able to query stacks[pos] and
                            ///       get something. Otherwise, the resulting position must be null.
                            ///   </para>
                            /// </summary>
                            protected abstract object Search(object position);

                            /// <summary>
                            ///   <para>
                            ///     This function iterates over the registered positions.The order of iteration is up
                            ///       to the implementor, so it will not be determined in a trivial fashion like, say,
                            ///       iterating over the keys of the dictionary.
                            ///   </para>
                            ///   <para>
                            ///     ALL THE POSITIONS SHOULD BE ITERATED FOR THE DISPLAYERS TO WORK PROPERLY. Every
                            ///       position being iterated should be valid. This means: stack[position] must not
                            ///       fail by absence.
                            ///   </para>
                            /// </summary>
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

                            /// <summary>
                            ///   Enumerates all the (position, stack) pairs.
                            /// </summary>
                            public IEnumerable<GMM.Types.Tuple<object, Stack>> StackPairs(bool reverse)
                            {
                                return from position in Positions(reverse) select new GMM.Types.Tuple<object, Stack>(position, stacks[position]);
                            }

                            /// <summary>
                            ///   Finds a stack by checking certain position. See <see cref="Search(object)"/>
                            ///     for more details.
                            /// </summary>
                            /// <param name="position">The position to check</param>
                            /// <returns>The stack, or <c>null</c> if none was found</returns>
                            public Stack Find(object position)
                            {
                                object canonicalPosition = Search(position);
                                return canonicalPosition == null ? null : stacks[canonicalPosition];
                            }

                            /// <summary>
                            ///   Finds all stacks satisfying a predicate on its position and the stack.
                            ///   It may reverse the order of enumeration.
                            /// </summary>
                            /// <param name="predicate">The predicate to test on each stack</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public IEnumerable<Stack> FindAll(Func<GMM.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                            {
                                return from pair in StackPairs(reverse).Where(predicate) select pair.Second;
                            }

                            /// <summary>
                            ///   Finds all stacks having a particular item.
                            ///   It may reverse the order of enumeration.
                            /// </summary>
                            /// <param name="item">The item to check</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public IEnumerable<Stack> FindAll(Item item, bool reverse)
                            {
                                return FindAll(delegate (GMM.Types.Tuple<object, Stack> pair)
                                {
                                    return pair.Second.Item == item;
                                }, reverse);
                            }

                            /// <summary>
                            ///   Finds all stacks exactly matching an external stack.
                            ///   It may reverse the order of enumeration.
                            /// </summary>
                            /// <param name="stack">The stack to compare against</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public IEnumerable<Stack> FindAll(Stack stack, bool reverse)
                            {
                                return FindAll(delegate (GMM.Types.Tuple<object, Stack> pair)
                                {
                                    return pair.Second.Equals(stack);
                                }, reverse);
                            }

                            /// <summary>
                            ///   Gets the first stack, if any.
                            /// </summary>
                            /// <returns>The first stack in this container</returns>
                            public Stack First()
                            {
                                return (from pair in StackPairs(false) select pair.Second).FirstOrDefault();
                            }

                            /// <summary>
                            ///   Gets the last stack, if any.
                            /// </summary>
                            /// <returns>The last stack in this container</returns>
                            public Stack Last()
                            {
                                return (from pair in StackPairs(true) select pair.Second).FirstOrDefault();
                            }

                            /// <summary>
                            ///   Finds a stack satisfying a predicate on its position and the stack.
                            ///   It may reverse the order of enumeration to find the first one.
                            /// </summary>
                            /// <param name="predicate">The predicate to test on each stack</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public Stack FindOne(Func<GMM.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                            {
                                return FindAll(predicate, reverse).FirstOrDefault();
                            }

                            /// <summary>
                            ///   Finds a stack having a particular item.
                            ///   It may reverse the order of enumeration to find the first one.
                            /// </summary>
                            /// <param name="item">The item to check</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public Stack FindOne(Item item, bool reverse)
                            {
                                return FindAll(item, reverse).FirstOrDefault();
                            }

                            /// <summary>
                            ///   Finds a stack exactly matching an external stack.
                            ///   It may reverse the order of enumeration to find the first one.
                            /// </summary>
                            /// <param name="stack">The stack to compare against</param>
                            /// <param name="reverse">Whether the search is in reversed order</param>
                            public Stack FindOne(Stack stack, bool reverse)
                            {
                                return FindAll(stack, reverse).FirstOrDefault();
                            }

                            /// <summary>
                            ///   Tries to find a first-match for the item. It must return null IF
                            ///     AND ONLY IF the inventory is full (i.e. no position is available).
                            /// </summary>
                            /// <param name="stack">The stack the caller is looking a position for</param>
                            public abstract object FirstFree(Stack stack);

                            /// <summary>
                            ///   <para>
                            ///     Puts a new (or moves an existing) stack in this container.It also sets
                            ///       the position of the stack.We already know the itemStrategy will be
                            ///       compatible with this strategy at this point.
                            ///   </para>
                            ///   <para>
                            ///     Will return false if the specified position is not available, or if
                            ///       position was not specified and the container is full (or not
                            ///       strictly full, yet unable to find an appropriate position for the
                            ///       stack).
                            ///   </para>
                            /// </summary>
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

                            /// <summary>
                            ///   Removes a stack from this container. It also cleans up the position of
                            ///     the stack.
                            /// </summary>
                            public bool Remove(Stack stack)
                            {
                                if (!stacks.ContainsValue(stack)) return false;
                                Release(stack.QualifiedPosition.First, stack);
                                stacks.Remove(stack.QualifiedPosition.First);
                                SetPosition(stack, null);
                                return true;
                            }

                            /// <summary>
                            ///   Tells how many stacks does this container have.
                            /// </summary>
                            public int Count { get { return stacks.Count; } }
                        }

                        /// <summary>
                        ///   This fake container is used when we need a dummy container
                        ///     that is always empty and fails on every method. This is
                        ///     a replacement to null-checks.
                        /// </summary>
                        public class NullSpatialContainer : SpatialContainer
                        {
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

                        /// <summary>
                        ///   The counterpart type (a subclass of <see cref="ItemSpatialStrategy"/>).
                        /// </summary>
                        public Type ItemSpatialStrategyCounterpartType { get; private set; }

                        /// <summary>
                        ///   It must be implemented: Initializes a new contianer, giving an arbitrary position.
                        /// </summary>
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

                        /// <summary>
                        ///   You must implement this: Which one is the counterpart spatial strategy type in
                        ///     the items.
                        /// </summary>
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

                        protected override void Awake()
                        {
                            base.Awake();
                            ItemSpatialStrategyCounterpartType = GetItemSpatialStrategyCounterpartType();
                            if (!GMM.Utils.Classes.IsSameOrSubclassOf(ItemSpatialStrategyCounterpartType, typeof(ItemSpatialStrategy)))
                            {
                                throw new InvalidItemSpatialStrategyCounterpartType(string.Format("The type returned by GetItemSpatialStrategyCounterpartType must be a subclass of {0}", typeof(ItemSpatialStrategy).FullName));
                            }
                        }

                        /**
                         * public methods accessing everything. ALL THE DOCUMENTED METHODS.
                         */

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.StackPairs(bool)"/> on a given container.
                        /// </summary>
                        public IEnumerable<GMM.Types.Tuple<object, Stack>> StackPairs(object containerPosition, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).StackPairs(reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.Find(object)"/> on a given container.
                        /// </summary>
                        public Stack Find(object containerPosition, object stackPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).Find(stackPosition);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindAll(Func{GMM.Types.Tuple{object, Stack}, bool}, bool)"/> on a given container.
                        /// </summary>
                        public IEnumerable<Stack> FindAll(object containerPosition, Func<GMM.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(predicate, reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindAll(Item, bool)"/> on a given container.
                        /// </summary>
                        public IEnumerable<Stack> FindAll(object containerPosition, Item item, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(item, reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindAll(Stack, bool)"/> on a given container.
                        /// </summary>
                        public IEnumerable<Stack> FindAll(object containerPosition, Stack stack, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindAll(stack, reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.First"/> on a given container.
                        /// </summary>
                        public Stack First(object containerPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).First();
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.Last"/> on a given container.
                        /// </summary>
                        public Stack Last(object containerPosition)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).Last();
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindOne(Func{GMM.Types.Tuple{object, Stack}, bool}, bool)"/> on a given container.
                        /// </summary>
                        public Stack FindOne(object containerPosition, Func<GMM.Types.Tuple<object, Stack>, bool> predicate, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(predicate, reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindOne(Item, bool)"/> on a given container.
                        /// </summary>
                        public Stack FindOne(object containerPosition, Item item, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(item, reverse);
                        }

                        /// <summary>
                        ///   Invokes <see cref="SpatialContainer.FindOne(Stack, bool)"/> on a given container.
                        /// </summary>
                        public Stack FindOne(object containerPosition, Stack stack, bool reverse)
                        {
                            return GetContainer(containerPosition, IfAbsent.Null).FindOne(stack, reverse);
                        }

                        /// <summary>
                        ///   Puts a stack inside a position (or the first match, if position is null) which in turn
                        ///     is inside a particular container.
                        /// </summary>
                        /// <param name="containerPosition">The ID of the container being added the stack to</param>
                        /// <param name="stackPosition">The position to add the stack to</param>
                        /// <param name="stack">The stack being added</param>
                        /// <param name="finalStackPosition">Output of the final position</param>
                        /// <returns>Whether it could add the stack to the container</returns>
                        /// <remarks>If the container with that ID does not exist, it will be created.</remarks>
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

                        /// <summary>
                        ///   Removes a stack in that position.
                        /// </summary>
                        /// <param name="containerPosition">The container to check</param>
                        /// <param name="stackPosition">The position to check (and remove)</param>
                        /// <returns>Whether it could remove a stack occupying that position</returns>
                        /// <remarks>If the container -at the end- is empty, it will be destroyed.</remarks>
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

                        /// <summary>
                        ///   Clears all the contents in all containers. Actually, it destroys all the containers.
                        /// </summary>
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
