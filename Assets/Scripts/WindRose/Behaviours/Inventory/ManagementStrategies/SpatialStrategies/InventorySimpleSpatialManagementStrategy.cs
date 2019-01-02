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

                    public abstract class InventorySimpleSpatialManagementStrategy : InventorySpatialManagementStrategy
                    {
                        /**
                         * This spatial strategy involves an inventory with indexed positions like
                         *   Baldur's Gate characters' bags. There will be two subclasses here:
                         *   Finite and infinite containers.
                         */

                        public abstract class SimpleSpatialContainer : SpatialContainer
                        {
                            // Flags to occupy the respective positions
                            private List<bool> elements = new List<bool>();

                            public SimpleSpatialContainer(InventorySpatialManagementStrategy spatialStrategy, object position) : base(spatialStrategy, position)
                            {
                            }

                            /**
                             * Finds the first empty position to put the element in.
                             */
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

                            /**
                             * Enumerates the positions being occupied.
                             */
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

                            /**
                             * Tells whether the position is occupied by checking the index.
                             */
                            private bool StackPositionIsOccupied(object position)
                            {
                                int index = (int)position;
                                return elements.Count > index && elements[index];
                            }

                            /**
                             * Returns the position if it is occupied. Otherwise, returns null.
                             */
                            protected override object Search(object position)
                            {
                                return StackPositionIsOccupied(position) ? position : null;
                            }

                            /**
                             * Tells whether the position is available to add an element.
                             */
                            protected override bool StackPositionIsAvailable(object position, Stack stack)
                            {
                                return !StackPositionIsOccupied(position);
                            }

                            /**
                             * Tells whether a position is valid in terms of being an index and with respect
                             *   to the Size (as a limit) if any.
                             */
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

                            protected abstract bool ValidateStackPositionAgainstUpperBound(int index);
                        }

                        /**
                         * Get the max size. 0 means "infinite".
                         */
                        public abstract int GetSize();

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
