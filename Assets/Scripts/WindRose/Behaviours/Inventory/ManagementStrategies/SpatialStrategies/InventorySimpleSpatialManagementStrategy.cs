using System;
using System.Collections.Generic;
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
                    using ScriptableObjects.Inventory.Items.SpatialStrategies;
                    using Types.Inventory.Stacks;

                    /// <summary>
                    ///   Simple spatial management strategies involve indexed accesses. They are a finite or infinite
                    ///     array managers.
                    /// </summary>
                    public abstract class InventorySimpleSpatialManagementStrategy : InventorySpatialManagementStrategy
                    {
                        /// <summary>
                        ///   A simple container mantains a sparse array of stacks in the container.
                        /// </summary>
                        public abstract class SimpleSpatialContainer : SpatialContainer
                        {
                            // Flags to occupy the respective positions
                            private List<bool> elements = new List<bool>();

                            public SimpleSpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
                            {
                            }

                            /// <summary>
                            ///   Find the first free slot.
                            /// </summary>
                            public override object FirstFree(Stack stack)
                            {
                                for(int index = 0; index < elements.Count; index++)
                                {
                                    if (!elements[index]) return index;
                                }

                                int size = ((InventorySimpleSpatialManagementStrategy)SpatialStrategy).GetSize();
                                if (size == 0 || elements.Count < size)
                                {
                                    // Return the count as the new position to add the new element.
                                    return elements.Count;
                                }

                                // The element has no place here
                                return null;
                            }

                            /// <summary>
                            ///   Occupies a particular (integer) slot for the stack.
                            /// </summary>
                            protected override void Occupy(object position, Stack stack)
                            {
                                int index = (int)position;
                                // We will fill with false values until we have a
                                //   position able to be assigned.
                                while(elements.Count <= index)
                                {
                                    elements.Add(false);
                                }
                                elements[index] = true;
                            }

                            /// <summary>
                            ///   Enumerates all the indices being occupied.
                            /// </summary>
                            protected override IEnumerable<object> Positions(bool reverse)
                            {
                                if (!reverse)
                                {
                                    for (int index = 0; index < elements.Count; index++)
                                    {
                                        if (elements[index]) yield return index;
                                    }
                                }
                                else
                                {
                                    for (int index = elements.Count - 1; index >= 0; index--)
                                    {
                                        if (elements[index]) yield return index;
                                    }
                                }
                            }

                            /// <summary>
                            ///   Releases the index being occupied.
                            /// </summary>
                            protected override void Release(object position, Stack stack)
                            {
                                // We will assume the position exists. We clear it.
                                elements[(int)position] = false;
                                // Also, we trim the elements.
                                while(elements.Count > 0 && !elements[elements.Count-1])
                                {
                                    elements.RemoveAt(elements.Count - 1);
                                }
                            }

                            /// <summary>
                            ///   Tells the position occupancy by checking the index.
                            /// </summary>
                            private bool StackPositionIsOccupied(object position)
                            {
                                int index = (int)position;
                                return elements.Count > index && elements[index];
                            }

                            /// <summary>
                            ///   Returns the same index if it is occupied. Otherwise,
                            ///     returns null.
                            /// </summary>
                            protected override object Search(object position)
                            {
                                return StackPositionIsOccupied(position) ? position : null;
                            }

                            /// <summary>
                            ///   Checks availability directly on the index.
                            /// </summary>
                            protected override bool StackPositionIsAvailable(object position, Stack stack)
                            {
                                return !StackPositionIsOccupied(position);
                            }

                            /// <summary>
                            ///   Validates the position is an integer one, and it is appropriately valued
                            ///     and bounded.
                            /// </summary>
                            protected override StackPositionValidity ValidateStackPosition(object position, Stack stack)
                            {
                                if (!(position is int)) return StackPositionValidity.InvalidType;

                                int index = (int)position;
                                if (index < 0 || !ValidateStackPositionAgainstUpperBound(index))
                                {
                                    return StackPositionValidity.OutOfBounds;
                                }

                                return StackPositionValidity.Valid;
                            }

                            /// <summary>
                            ///   This one must be implemented. It is the upper bound check for these containers.
                            /// </summary>
                            protected abstract bool ValidateStackPositionAgainstUpperBound(int index);
                        }

                        /// <summary>
                        ///   Gets the maximum size of each container. 0 means "infinite".
                        /// </summary>
                        /// <returns>The maximum size of each container</returns>
                        public abstract int GetSize();

                        /// <summary>
                        ///   Counterpart type is <see cref="ItemSimpleSpatialStrategy"/>.
                        /// </summary>
                        protected override Type GetItemSpatialStrategyCounterpartType()
                        {
                            return typeof(ItemSimpleSpatialStrategy);
                        }
                    }
                }
            }
        }
    }
}
