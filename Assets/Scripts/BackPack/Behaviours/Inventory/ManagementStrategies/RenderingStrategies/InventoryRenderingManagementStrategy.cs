using System;
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
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks;

                    /// <summary>
                    ///   <para>
                    ///     Provides methods to reflect changes on the stacks being added, modified, or
                    ///       removed on certain (inventory, stack) positions.
                    ///   </para>
                    ///   <para>
                    ///     Quite often the rendering strategies WILL depend on specific:
                    ///     <list type="bullet">
                    ///       <item>
                    ///         <term>Positioning strategies</term>
                    ///         <description>
                    ///           They deal with potentially many simultaneous inventories managed in
                    ///             the same way (e.g. drop in the floor).
                    ///         </description>
                    ///       </item>
                    ///       <item>
                    ///         <term>Spatial strategies</term>
                    ///         <description>They deal with the contents of an inventory.</description>
                    ///       </item>
                    ///       <item>
                    ///         <term>Usage strategies</term>
                    ///         <description>
                    ///           They deal with the in-game logic of the objects. They could also
                    ///             provide hints of how to render an item or UI component.
                    ///         </description>
                    ///       </item>
                    ///       <item>
                    ///         <term>Per-object quantifying strategy</term>
                    ///         <description>How to render the amount of elements in a stack.</description>
                    ///       </item>
                    ///     </list>
                    ///   </para>
                    ///   <para>
                    ///     Each rendering strategy will do its own, but WILL attend to these callbacks: they
                    ///       MUST be implemented.
                    ///   </para>
                    ///   <para>
                    ///     Perhaps these calls do not reflect changes immediately, but they may collect
                    ///       appropriate data to be rendered later. It is up to the implementor to decide what to do.
                    ///   </para>
                    /// </summary>
                    public abstract class InventoryRenderingManagementStrategy : InventoryManagementStrategy
                    {
                        /// <summary>
                        ///   Handles the update of everything being cleared / removed.
                        /// </summary>
                        public abstract void EverythingWasCleared();

                        /// <summary>
                        ///   Handles the update of a stack being updated or refreshed.
                        /// </summary>
                        /// <param name="containerPosition">The container ID of the stack</param>
                        /// <param name="stackPosition">The position of the stack</param>
                        /// <param name="stack">The stack at the given position</param>
                        public abstract void StackWasUpdated(object containerPosition, object stackPosition, Stack stack);

                        /// <summary>
                        ///   Handles the update of a stack being removed.
                        /// </summary>
                        /// <param name="containerPosition">The container ID of the removed stack</param>
                        /// <param name="stackPosition">The position of the stack</param>
                        /// <remarks>At this point, the stack is already removed.</remarks>
                        public abstract void StackWasRemoved(object containerPosition, object stackPosition);
                    }
                }
            }
        }
    }
}
