using System;
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
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks;

                    public abstract class InventoryRenderingManagementStrategy : InventoryManagementStrategy
                    {
                        /**
                         * Provides methods to reflect changes on the stacks being added, modified, or
                         *   removed on certain (inventory, stack) positions.
                         * 
                         * Quite often the rendering strategies WILL depend on specific:
                         * - Positioning strategies: they deal with potentially many simultaneous inventories
                         *     managed in the same way (e.g. drop in the floor).
                         * - Spatial strategies: they deal with the contents of an inventory.
                         * - Usage strategies: they deal with the in-game logic of the objects. They could also
                         *     provide hints of how to render an item or UI component.
                         * - Per-object quantifying strategy: how to render the amount of elements in a stack.
                         * 
                         * Each rendering strategy will do its own, but WILL attend to these callbacks: they
                         *   MUST be implemented.
                         * 
                         * Perhaps these calls do not reflect changes immediately, but they may collect appropriate
                         *   data to be rendered later. It is up to the implementor to decide what to do.
                         */

                        /**
                         * Triggers an update: clears everything.
                         */
                        public abstract void EverythingWasCleared();

                        /**
                         * Triggers an update: stack added/refreshed.
                         */
                        public abstract void StackWasUpdated(object containerPosition, object stackPosition, Stack stack);

                        /**
                         * Triggers an update: stack removed.
                         */
                        public abstract void StackWasRemoved(object containerPosition, object stackPosition);
                    }
                }
            }
        }
    }
}
