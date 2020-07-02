using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BackPack
{
    namespace Behaviours
    {
        namespace Inventory
        {
            using GMM.Types;
            using Types.Inventory.Stacks;
            using ManagementStrategies.UsageStrategies;

            /// <summary>
            ///   <para>
            ///     Inventory management strategy holders are the live
            ///       counterpart of the <see cref="ScriptableObjects.Inventory.Items.Item"/>
            ///       and their instantiated <see cref="Stack"/> objects, since
            ///       the management strategy holders perform the actual logic
            ///       of each aspect.
            ///   </para>
            ///   <para>
            ///     In the same game object, instances of the other strategy types
            ///       must be added (usage, spatial, container positioning, and rendering).
            ///   </para>
            /// </summary>
            public class InventoryManagementStrategyHolder : MonoBehaviour
            {
                /// <summary>
                ///   Tells when an invalid strategy is chosen as the value
                ///     of <see cref="mainUsageStrategy"/>, or that property
                ///     is null.
                /// </summary>
                public class InvalidStrategyComponentException : GMM.Types.Exception
                {
                    public InvalidStrategyComponentException(string message) : base(message) { }
                }

                /// <summary>
                ///   Tells when a stack is rejected in the inventory due to
                ///     having invalid quantity value, not having the required
                ///     spatial strategy, or having an incompatible usage strategy.
                /// </summary>
                public class StackRejectedException : GMM.Types.Exception
                {
                    /// <summary>
                    ///   Rejection may involve quantity of invalid type, not having
                    ///     an instance of <see cref="ScriptableObjects.Inventory.Items.SpatialStrategies.ItemSpatialStrategy"/>
                    ///     suitable for this inventory, or not having a compatible
                    ///     main usage strategy.
                    /// </summary>
                    public enum RejectionReason { InvalidQuantity, IncompatibleSpatialStrategy, IncompatibleUsageStrategy }

                    /// <summary>
                    ///   The reason of the rejection.
                    /// </summary>
                    public readonly RejectionReason Reason;

                    public StackRejectedException(RejectionReason reason) : base(string.Format("The stack cannot be accepted into this inventory. Reason: {0}", reason))
                    {
                        Reason = reason;
                    }
                }

                /**
                 * Positioning strategies will tell how many inventories will we be able to manage, and how are
                 *   they distributed. Spatial strategies will make indirect use of this data.
                 */
                private ManagementStrategies.PositioningStrategies.InventoryPositioningManagementStrategy positioningStrategy;

                /**
                 * A spatial strategy needed to tell how does the inventory locates its items. Will be fetched
                 *   from the added components, and only ONE will be allowed.
                 */
                private ManagementStrategies.SpatialStrategies.InventorySpatialManagementStrategy spatialStrategy;

                /**
                 * Many usage strategies needed to tell how does the inventory uses/interacts-with the stacks.
                 *   They will be fetched from the added components, and will be sorted dependency-wise.
                 */
                private ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy[] sortedUsageStrategies;

                /// <summary>
                ///   This is the main usage strategy this holder will have. This one is required, and must be present
                ///     among the components.
                /// </summary>
                [SerializeField]
                private ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy mainUsageStrategy;

                /**
                 * This is the rendering strategy. It will depend on the other strategies since it will have to collect
                 *   the appropriate data to render.
                 */
                private ManagementStrategies.RenderingStrategies.InventoryRenderingManagementStrategy renderingStrategy;

                /// <summary>
                ///   Determines how should this handler behave when calling <see cref="Put(object, object, Stack, out object, bool?)"/>
                ///     and leaving the 5th parameter (also named <c>optimalPutOnNullPosition</c>) null: in such case, the value
                ///     of this field will be taken. The meaning of each possible value is already described in the Put function.
                /// </summary>
                [SerializeField]
                private bool optimalPutOnNullPosition = true;

                private void Awake()
                {
                    positioningStrategy = GetComponent<ManagementStrategies.PositioningStrategies.InventoryPositioningManagementStrategy>();
                    spatialStrategy = GetComponent<ManagementStrategies.SpatialStrategies.InventorySpatialManagementStrategy>();
                    ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy[] usageStrategies = GetComponents<ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy>();
                    sortedUsageStrategies = (from component in GMM.Utils.Layout.SortByDependencies(usageStrategies) select (component as ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy)).ToArray();
                    if (mainUsageStrategy == null || !(new HashSet<ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy>(sortedUsageStrategies).Contains(mainUsageStrategy)))
                    {
                        Destroy(gameObject);
                        throw new InvalidStrategyComponentException("The selected strategy component must be non-null and present among the current pack manager's components");
                    }
                    renderingStrategy = GetComponent<ManagementStrategies.RenderingStrategies.InventoryRenderingManagementStrategy>();
                }

                /*********************************************************************************************
                 *********************************************************************************************
                 * Inventory methods.
                 *********************************************************************************************
                 *********************************************************************************************/

                /// <summary>
                ///   Given a particular container ID, this method returns an iterable traversing all the stacks
                ///     in such container.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="reverse">Whether the items should be traversed in reverse or straight way</param>
                /// <returns>An iterable of pairs (in-container position, stack)</returns>
                public IEnumerable<Tuple<object, Stack>> StackPairs(object containerPosition, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.StackPairs(containerPosition, reverse);
                }

                /// <summary>
                ///   Finds a stack located in a particular container and a particular position.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The in-container position to check</param>
                /// <returns>A stack occupying that position</returns>
                public Stack Find(object containerPosition, object stackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.Find(containerPosition, stackPosition);
                }

                /// <summary>
                ///   Finds all the stacks in a particular container that satisfy a condition.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="predicate">The predicate they must satisfy</param>
                /// <param name="reverse">Whether the items should be traversed in reverse or straight way</param>
                /// <returns>An iterable of stacks satisfying the predicate</returns>
                public IEnumerable<Stack> FindAll(object containerPosition, Func<Tuple<object, Stack>, bool> predicate, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, predicate, reverse);
                }

                /// <summary>
                ///   Finds all the stacks in a particular container of a particular item.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="item">The item they have to be of</param>
                /// <param name="reverse">Whether the items should be traversed in reverse or straight way</param>
                /// <returns>An iterable of stacks being of that item</returns>
                public IEnumerable<Stack> FindAll(object containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, item, reverse);
                }

                /// <summary>
                ///   Returns the first item inside the given container.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <returns>The first item inside that container</returns>
                public Stack First(object containerPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.First(containerPosition);
                }

                /// <summary>
                ///   Returns the last item inside the given container.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <returns>The last item inside that container</returns>
                public Stack Last(object containerPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.Last(containerPosition);
                }

                /// <summary>
                ///   Like <see cref="FindAll(object, Func{Tuple{object, Stack}, bool}, bool)"/>
                ///     but only returns the first matched stack, or null.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="predicate">The predicate they must satisfy</param>
                /// <param name="reverse">Whether the items should be traversed in reverse or straight way</param>
                /// <returns>The first stack matched</returns>
                public Stack FindOne(object containerPosition, Func<Tuple<object, Stack>, bool> predicate, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, predicate, reverse);
                }

                /// <summary>
                ///   Like <see cref="FindAll(object, ScriptableObjects.Inventory.Items.Item, bool)"/>
                ///     but only returns the first matched stack, or null.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="predicate">The predicate they must satisfy</param>
                /// <param name="reverse">Whether the items should be traversed in reverse or straight way</param>
                /// <returns>The first stack matched</returns>
                public Stack FindOne(object containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, item, reverse);
                }

                /**
                 * This method will perfirm an optimal put: filling existing matching stacks before adding this new stack to
                 *   the inventory.
                 */
                private bool OptimalPut(object containerPosition, object stackPosition, Stack stack, out object finalStackPosition)
                {

                    // This list is to queue stacks that will saturate on optimal put for cases when position
                    //   is not chosen by the user, and optimal put is chosen/preset.
                    List<Stack> stacksToSaturate = new List<Stack>();

                    // This stack is the last stack that would receive quantity. In this case, this stack
                    //   does not overflow (i.e. the quantity left is 0).
                    Stack unsaturatedLastStack = null;

                    // The condition was chosen because a fixed position would instead put the stack there.
                    // But since the position was chosen to be determined by the engine, we have a unique
                    //   opportunity to also redistribute the stack to optimize its occupancy.

                    // We will track the current quantity to add/saturate here.
                    object currentQuantity = stack.Quantity;

                    IEnumerable<Stack> matchedStacks = spatialStrategy.FindAll(containerPosition, stack, false);

                    // And we will iterate computing saturations here. Stacks to saturate will be
                    //   queued in the list above.
                    foreach (Stack matchedStack in matchedStacks)
                    {
                        object quantityAdded;
                        object quanityLeft;
                        object finalQuantity;
                        bool wouldSaturate = matchedStack.WillOverflow(currentQuantity, out finalQuantity, out quantityAdded, out quanityLeft);
                        if (wouldSaturate)
                        {
                            currentQuantity = quanityLeft;
                            stacksToSaturate.Add(matchedStack);
                        }
                        else
                        {
                            unsaturatedLastStack = matchedStack;
                            break;
                        }
                    }

                    // Now we have two cases here:
                    // 1. no unsaturated stack is present.
                    // 2. an unsaturated stack is present.
                    if (unsaturatedLastStack != null)
                    {
                        // Saturate the pending ones, and add amount to the last
                        foreach (Stack queuedStack in stacksToSaturate)
                        {
                            queuedStack.Saturate();
                        }
                        unsaturatedLastStack.ChangeQuantityBy(currentQuantity);

                        // Render everything
                        foreach (Stack queuedStack in stacksToSaturate)
                        {
                            renderingStrategy.StackWasUpdated(containerPosition, queuedStack.QualifiedPosition.Item1, queuedStack);
                        }
                        renderingStrategy.StackWasUpdated(containerPosition, unsaturatedLastStack.QualifiedPosition.Item1, unsaturatedLastStack);

                        // The stack was put, but not on a new position: instead, it filled other stacks and it should be
                        //   considered destroyed.
                        finalStackPosition = null;

                        // Still we return true because the operation was successful.
                        return true;
                    }
                    else
                    {
                        // Before saturating stacks, we try putting a clone of the current stack with the remaining quantity.
                        // If we can do that, then saturate all the other stacks and proceed.
                        Stack stackWithRemainder = stack.Clone(currentQuantity);
                        bool wasPut = spatialStrategy.Put(containerPosition, null, stackWithRemainder, out finalStackPosition);
                        if (wasPut)
                        {
                            // Saturate the pending ones
                            foreach (Stack queuedStack in stacksToSaturate)
                            {
                                queuedStack.Saturate();
                            }

                            // Render everything
                            foreach (Stack queuedStack in stacksToSaturate)
                            {
                                renderingStrategy.StackWasUpdated(containerPosition, queuedStack.QualifiedPosition.Item1, queuedStack);
                            }
                            renderingStrategy.StackWasUpdated(containerPosition, stackWithRemainder.QualifiedPosition.Item1, stackWithRemainder);
                            return true;
                        }
                        else
                        {
                            // Nothing happened here.
                            finalStackPosition = null;
                            return false;
                        }
                    }
                }

                /// <summary>
                ///   Puts a stack (that must not belong to any inventory) in a specified container.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The in-container position to use - it may be null to let the inventory choose one instead</param>
                /// <param name="stack">The stack to add</param>
                /// <param name="finalStackPosition">The returned final position of the stack - if null, this means the stack was optimally distributed among existing "equal" stacks</param>
                /// <param name="optimalPutOnNullPosition">
                ///   <para>Whether an optimal put should be used on null stack position. options are:</para>
                ///   <list type="bullet">
                ///     <item>
                ///       <term><c>true</c></term>
                ///       <description>On null input position, this stack will not be added at first but instead tried to distribute among existing "equal" stacks.</description>
                ///     </item>
                ///     <item>
                ///       <term><c>false</c></term>
                ///       <description>
                ///         On null input position, this stack will be added at the first-free available position (valid for the spatial strategies to be
                ///           considered).
                ///       </description>
                ///     </item>
                ///     <item>
                ///       <term><c>null</c></term>
                ///       <description>
                ///         Makes the value of this parameter be set from <see cref="optimalPutOnNullPosition"/> property, whose meaning will be the same
                ///           as the previous non-null options.
                ///       </description>
                ///     </item>
                ///   </list> 
                /// </param>
                /// <returns>Whether the stack could be put in the inventory's container (i.e. available position was appropriately got)</returns>
                public bool Put(object containerPosition, object stackPosition, Stack stack, out object finalStackPosition, bool? optimalPutOnNullPosition = null)
                {
                    if (!stack.QuantifyingStrategy.HasAllowedQuantity())
                    {
                        throw new StackRejectedException(StackRejectedException.RejectionReason.InvalidQuantity);
                    }

                    if (!mainUsageStrategy.Accepts(stack.MainUsageStrategy))
                    {
                        throw new StackRejectedException(StackRejectedException.RejectionReason.IncompatibleUsageStrategy);
                    }

                    positioningStrategy.CheckPosition(containerPosition);

                    // The actual logic starts here.

                    // We will determine the optimal put setting from the instance if it is not present as
                    //   argument.
                    if (optimalPutOnNullPosition == null)
                    {
                        optimalPutOnNullPosition = this.optimalPutOnNullPosition;
                    }

                    // Two logics we will consider here:
                    // 1. When position is not specified, and optimal put is true.
                    // 2. When position is specified, or optimal put is false. In both cases, the stack will
                    //      simply be put in a certain position, with no redistribution (this one is the case
                    //      we are having right now).
                    if (stackPosition == null && optimalPutOnNullPosition == true)
                    {
                        return OptimalPut(containerPosition, stackPosition, stack, out finalStackPosition);
                    }
                    else
                    {
                        // Regular way to proceed here.
                        bool result = spatialStrategy.Put(containerPosition, stackPosition, stack, out finalStackPosition);
                        if (result)
                        {
                            renderingStrategy.StackWasUpdated(containerPosition, stackPosition, stack);
                        }
                        return result;
                    }
                }

                /// <summary>
                ///   Removes the stack at certain position inside a container.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The stack position to pop out of the inventory, if present</param>
                /// <returns>Whether an element was found and removed at that position</returns>
                public bool Remove(object containerPosition, object stackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    bool result = spatialStrategy.Remove(containerPosition, stackPosition);
                    if (result)
                    {
                        renderingStrategy.StackWasRemoved(containerPosition, stackPosition);
                    }
                    return result;                    
                }

                /// <summary>
                ///   Merges two stacks in the same containers, provided both stacks are "mergeable" between them.
                ///     The source stack will be merged into the destination stack, and both stacks will be merged
                ///     creating a new stack with added quantities and interpolated properties. For this to work,
                ///     stacks must have usage strategies that CAN be interpolated.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="destinationStackPosition">The destination stack position</param>
                /// <param name="sourceStackPosition">The source stack position</param>
                /// <returns>
                ///   Whether both source and destination stacks were found, quantities could be added, and they
                ///     were compatible enough to succeed in the final step of merge/interpolation.
                /// </returns>
                /// <remarks>
                ///   If quantities are saturated, there will be a partial merge and part of the source stack will
                ///     still exist. Otherwise, the merge will be total and the source stack will be deleted.
                /// </remarks>
                public bool Merge(object containerPosition, object destinationStackPosition, object sourceStackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return Merge(containerPosition, destinationStackPosition, this, containerPosition, sourceStackPosition);
                }

                /// <summary>
                ///   See <see cref="Merge(object, object, object)"/>. This is an alternate version (actually: this is the
                ///     generic one) which can merge across different inventories. The logic will remain the same.
                /// </summary>
                /// <param name="destinationContainerPosition">The ID of the destination spatial container</param>
                /// <param name="destinationStackPosition">The position of the destination stack</param>
                /// <param name="sourceHolder">The source inventory managemnt strategy holder (our current instance is the destination one)</param>
                /// <param name="sourceContainerPosition">The ID of the source spatial container</param>
                /// <param name="sourceStackPosition">The position of the source stack</param>
                /// <returns>
                ///   Whether both source and destination stacks were found, quantities could be added, and they
                ///     were compatible enough to succeed in the final step of merge/interpolation.
                /// </returns>
                /// <remarks>
                ///   If quantities are saturated, there will be a partial merge and part of the source stack will
                ///     still exist. Otherwise, the merge will be total and the source stack will be deleted.
                /// </remarks>
                public bool Merge(object destinationContainerPosition, object destinationStackPosition,
                                  InventoryManagementStrategyHolder sourceHolder, object sourceContainerPosition, object sourceStackPosition)
                {
                    positioningStrategy.CheckPosition(destinationContainerPosition);
                    Stack destination = Find(destinationContainerPosition, destinationStackPosition);
                    if (destination == null)
                    {
                        return false;
                    }

                    Stack source = sourceHolder.Find(sourceContainerPosition, sourceStackPosition);
                    if (source == null)
                    {
                        return false;
                    }

                    object quantityLeft;
                    Stack.MergeResult result = destination.Merge(source, out quantityLeft);

                    if (result == Stack.MergeResult.Denied)
                    {
                        if (result == Stack.MergeResult.Total)
                        {
                            renderingStrategy.StackWasUpdated(destinationContainerPosition, destinationStackPosition, destination);
                            sourceHolder.Remove(sourceContainerPosition, sourceStackPosition);
                        }
                        else
                        {
                            renderingStrategy.StackWasUpdated(destinationContainerPosition, destinationStackPosition, destination);
                            source.ChangeQuantityTo(quantityLeft);
                            sourceHolder.renderingStrategy.StackWasUpdated(sourceContainerPosition, sourceStackPosition, source);
                        }
                        return true;
                    }

                    return false;
                }

                /// <summary>
                ///   Takes part of the specified stack. Just a quantity, and not necessarily the whole stack.
                ///     If this method fails, it returns null (failure may occur by having a bigger quantity
                ///     than what may be taken, or by the element not existing).
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The source stack position</param>
                /// <param name="quantity">The quantity to take</param>
                /// <param name="disallowEmpty">Whether we disallow the case of taking ALL the quantity, or we allow it</param>
                /// <returns>A stack with the same specs, except for the quantity, or null if such quantity could not be taken</returns>
                public Stack Take(object containerPosition, object stackPosition, object quantity, bool disallowEmpty)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    Stack found = Find(containerPosition, stackPosition);
                    if (found != null)
                    {
                        Stack result = found.Take(quantity, disallowEmpty);
                        if (result != null)
                        {
                            if (found.IsEmpty())
                            {
                                spatialStrategy.Remove(containerPosition, stackPosition);
                                renderingStrategy.StackWasRemoved(containerPosition, stackPosition);
                            }
                            else
                            {
                                renderingStrategy.StackWasUpdated(containerPosition, stackPosition, found);
                            }
                        }
                        return result;
                    }
                    return null;
                }

                /// <summary>
                ///   Splits a stack in two, by taking a particular quantity of the source stack and creating a
                ///     new one.
                /// </summary>
                /// <param name="sourceContainerPosition">The ID of the source spatial container</param>
                /// <param name="sourceStackPosition">The position of the source stack</param>
                /// <param name="quantity">The quantity to take from the source stack</param>
                /// <param name="newStackContainerPosition">The ID of the destination spatial container</param>
                /// <param name="newStackPosition">The position of the destination stack</param>
                /// <param name="finalNewStackPosition">Output parameter returning the final position given for the new stack</param>
                /// <returns>Whether the split could be performed (stack was found, quantity was available, and destination was free)</returns>
                public bool Split(object sourceContainerPosition, object sourceStackPosition, object quantity,
                                  object newStackContainerPosition, object newStackPosition, out object finalNewStackPosition)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        Stack newStack = found.Take(quantity, true);
                        if (!Put(newStackContainerPosition, newStackPosition, newStack, out finalNewStackPosition))
                        {
                            // Could not put the new stack - refund its quantity.
                            found.ChangeQuantityBy(quantity);
                            return false;
                        }
                        else
                        {
                            renderingStrategy.StackWasUpdated(sourceContainerPosition, sourceStackPosition, found);
                            renderingStrategy.StackWasUpdated(newStackContainerPosition, newStackPosition, newStack);
                            return true;
                        }
                    }
                    finalNewStackPosition = null;
                    return false;
                }

                /// <summary>
                ///   USES a stack in certain container and position. Using the item is an interaction
                ///     between the usage strategy in the item/stack, and the usage strategy in the
                ///     inventory (which is the one that has logic). The argument given to the use
                ///     callback will be <c>null</c>.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The position of the stack being used</param>
                /// <returns>Whether the usage interaction could be run</returns>
                public bool Use(object containerPosition, object stackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    Stack found = Find(containerPosition, stackPosition);
                    if (found != null)
                    {
                        mainUsageStrategy.Use(found);
                        return true;
                    }
                    return false;
                }

                /// <summary>
                ///   USES a stack in certain container and position. Using the item is an interaction
                ///     between the usage strategy in the item/stack, and the usage strategy in the
                ///     inventory (which is the one that has logic). The argument given to the use
                ///     callback will be also given.
                /// </summary>
                /// <param name="containerPosition">The ID of the spatial container</param>
                /// <param name="stackPosition">The position of the stack being used</param>
                /// <param name="argument">The argument for the usage command</param>
                /// <returns>Whether the usage interaction could be run</returns>
                public bool Use(object containerPosition, object sourceStackPosition, object argument)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    Stack found = Find(containerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        mainUsageStrategy.Use(found, argument);
                        return true;
                    }
                    return false;
                }

                /// <summary>
                ///   Clears ALL THE INVENTORY. Deleting all the stacks from all the inventories.
                /// </summary>
                public void Clear()
                {
                    spatialStrategy.Clear();
                    renderingStrategy.EverythingWasCleared();
                }

                /*********************************************************************************************
                 *********************************************************************************************
                 * Rendering methods.
                 *********************************************************************************************
                 *********************************************************************************************/

                /******
                 * Blinking methods. They are means to be used externally, since other calls make direct use
                 *   of the rendering strategy.
                 * 
                 * You can choose to blink an entire inventory, a particular container, or a single stack.
                 ******/

                private void DoBlink(object containerPosition, object stackPosition, Stack stack)
                {
                    if (stack != null)
                    {
                        renderingStrategy.StackWasUpdated(containerPosition, stackPosition, stack);
                    }
                    else
                    {
                        renderingStrategy.StackWasRemoved(containerPosition, stackPosition);
                    }
                }

                /// <summary>
                ///   Refreshes a stack in the rendering.
                /// </summary>
                /// <param name="containerPosition">The ID of the container</param>
                /// <param name="stackPosition">The position of the stack</param>
                public void Blink(object containerPosition, object stackPosition)
                {
                    DoBlink(containerPosition, stackPosition, Find(containerPosition, stackPosition));
                }

                private void DoBlink(object containerPosition, IEnumerable<Tuple<object, Stack>> pairs)
                {
                    foreach (Tuple<object, Stack> pair in pairs)
                    {
                        DoBlink(containerPosition, pair.Item1, pair.Item2);
                    }
                }

                /// <summary>
                ///   Refreshes all the stacks of a container in the rendering.
                /// </summary>
                /// <param name="containerPosition">The ID of the container</param>
                public void Blink(object containerPosition)
                {
                    IEnumerable<Tuple<object, Stack>> pairs = null;

                    try
                    {
                        pairs = spatialStrategy.StackPairs(containerPosition, false);
                    }
                    catch(Exception)
                    {
                        return;
                    }

                    DoBlink(containerPosition, pairs);
                }

                /// <summary>
                ///   Refreshes all the stacks -in the inventory- in the rendering.
                /// </summary>
                public void Blink()
                {
                    foreach(object position in positioningStrategy.Positions())
                    {
                        Blink(position);
                    }
                }

                /*********************************************************************************************
                 *********************************************************************************************
                 * Serializing / Deserializing all the stuff.
                 *********************************************************************************************
                 *********************************************************************************************/

                /// <summary>
                ///   Imports some serialized data and replaces the current content of the
                ///     inventory manager with the parsed content from the serialized data.
                /// </summary>
                /// <param name="serializedInventory">The data to import for this inventory</param>
                public void Import(Types.Inventory.SerializedInventory serializedInventory)
                {
                    Clear();
                    foreach(KeyValuePair<object, Types.Inventory.SerializedContainer> containerPair in serializedInventory)
                    {
                        foreach(KeyValuePair<object, Types.Inventory.SerializedStack> stackPair in containerPair.Value)
                        {
                            ScriptableObjects.Inventory.Items.Item item = ScriptableObjects.Inventory.Items.ItemRegistry.GetItem(stackPair.Value.Item1, stackPair.Value.Item2);
                            if (item != null)
                            {
                                object finalStackPosition;
                                Put(containerPair.Key, stackPair.Key, item.Create(stackPair.Value.Item3, stackPair.Value.Item4), out finalStackPosition);
                            }
                        }
                    }
                }

                /// <summary>
                ///   Counterpart of <see cref="Import(Types.Inventory.SerializedInventory)"/>, this method
                ///     serializes the content of the inventory.
                /// </summary>
                /// <returns>The serialized content</returns>
                public Types.Inventory.SerializedInventory Export()
                {
                    Types.Inventory.SerializedInventory serializedInventory = new Types.Inventory.SerializedInventory();
                    foreach(object containerPosition in positioningStrategy.Positions())
                    {
                        if (!serializedInventory.ContainsKey(containerPosition))
                        {
                            serializedInventory[containerPosition] = new Types.Inventory.SerializedContainer();
                        }

                        foreach(Tuple<object, Stack> stackPair in spatialStrategy.StackPairs(containerPosition, false))
                        {
                            object stackPosition = stackPair.Item1;
                            Stack stack = stackPair.Item2;
                            Tuple<ScriptableObjects.Inventory.Items.Item, object, object> dumped = stack.Dump();
                            serializedInventory[containerPosition][stackPosition] = new Types.Inventory.SerializedStack(dumped.Item1.Registry.Key, dumped.Item1.Key, dumped.Item2, dumped.Item3);
                        }
                    }
                    return serializedInventory;
                }
            }

#if UNITY_EDITOR
            [CustomEditor(typeof(InventoryManagementStrategyHolder))]
            [CanEditMultipleObjects]
            public class InventoryManagementStrategyHolderEditor : Editor
            {
                SerializedProperty strategy;

                protected virtual void OnEnable()
                {
                    strategy = serializedObject.FindProperty("mainUsageStrategy");
                }

                public override void OnInspectorGUI()
                {
                    serializedObject.Update();

                    InventoryManagementStrategyHolder underlyingObject = (serializedObject.targetObject as InventoryManagementStrategyHolder);
                    InventoryUsageManagementStrategy[] strategies = underlyingObject.GetComponents<InventoryUsageManagementStrategy>();
                    GUIContent[] strategyNames = (from strategy in strategies select new GUIContent(strategy.GetType().Name)).ToArray();

                    int index = ArrayUtility.IndexOf(strategies, strategy.objectReferenceValue as InventoryUsageManagementStrategy);
                    index = EditorGUILayout.Popup(new GUIContent("Main Usage Strategy"), index, strategyNames);
                    strategy.objectReferenceValue = index >= 0 ? strategies[index] : null;

                    serializedObject.ApplyModifiedProperties();
                }
            }
#endif
        }
    }
}
