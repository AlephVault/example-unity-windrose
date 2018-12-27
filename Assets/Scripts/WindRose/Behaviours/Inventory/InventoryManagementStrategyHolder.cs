using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            using Support.Types;
            using Types.Inventory.Stacks;

            public class InventoryManagementStrategyHolder : MonoBehaviour
            {
                /**
                 * This class is the counterpart of the item and the stack:
                 *   the pack manager will hold the strategies being the
                 *   respective counterparts of the stack strategies and
                 *   the item strategies (item-stack-pack will work their
                 *   strategies in an aligned way).
                 */

                public class InvalidStrategyComponentException : Types.Exception
                {
                    public InvalidStrategyComponentException(string message) : base(message) { }
                }

                public class StackRejectedException : Types.Exception
                {
                    public enum RejectionReason { InvalidQuantity, IncompatibleSpatialStrategy, IncompatibleUsageStrategy }

                    public readonly RejectionReason Reason;

                    public StackRejectedException(RejectionReason reason) : base(string.Format("The stack cannot be accepted into this inventory. Reason: {}", reason))
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

                /**
                 * This is the main usage strategy this holder will have. This one is required, and must be present
                 *   among the components.
                 */
                [SerializeField]
                private ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy mainUsageStrategy;

                /**
                 * This is the rendering strategy. It will depend on the other strategies since it will have to collect
                 *   the appropriate data to render.
                 */
                private ManagementStrategies.RenderingStrategies.InventoryRenderingManagementStrategy renderingStrategy;

                /**
                 * Default setting to apply when calling PUT with a null position. 
                 */
                [SerializeField]
                private bool optimalPutOnNullPosition = true;

                private void Awake()
                {
                    positioningStrategy = GetComponent<ManagementStrategies.PositioningStrategies.InventoryPositioningManagementStrategy>();
                    spatialStrategy = GetComponent<ManagementStrategies.SpatialStrategies.InventorySpatialManagementStrategy>();
                    ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy[] usageStrategies = GetComponents<ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy>();
                    sortedUsageStrategies = (from component in Support.Utils.Layout.SortByDependencies(usageStrategies) select (component as ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy)).ToArray();
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

                public IEnumerable<Tuple<object, Stack>> StackPairs(object containerPosition, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.StackPairs(containerPosition, reverse);
                }

                public Stack Find(object containerPosition, object stackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.Find(containerPosition, stackPosition);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, Func<Tuple<object, Stack>, bool> predicate, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, predicate, reverse);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, item, reverse);
                }

                public Stack First(object containerPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.First(containerPosition);
                }

                public Stack Last(object containerPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.Last(containerPosition);
                }

                public Stack FindOne(object containerPosition, Func<Tuple<object, Stack>, bool> predicate, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, predicate, reverse);
                }

                public Stack FindOne(object containerPosition, ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, item, reverse);
                }

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

                        // And we will iterate computing saturations here. Stacks to saturate will be
                        //   queued in the list above.
                        foreach (Stack matchedStack in spatialStrategy.FindAll(containerPosition, stack, false))
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
                            foreach(Stack queuedStack in stacksToSaturate)
                            {
                                queuedStack.Saturate();
                            }
                            unsaturatedLastStack.ChangeQuantityBy(currentQuantity);

                            // Render everything
                            foreach (Stack queuedStack in stacksToSaturate)
                            {
                                renderingStrategy.StackWasUpdated(containerPosition, queuedStack.QualifiedPosition.First, queuedStack);
                            }
                            renderingStrategy.StackWasUpdated(containerPosition, unsaturatedLastStack.QualifiedPosition.First, unsaturatedLastStack);

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
                                    renderingStrategy.StackWasUpdated(containerPosition, queuedStack.QualifiedPosition.First, queuedStack);
                                }
                                renderingStrategy.StackWasUpdated(containerPosition, stackWithRemainder.QualifiedPosition.First, stackWithRemainder);
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

                public bool Merge(object containerPosition, object destinationStackPosition, object sourceStackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return Merge(containerPosition, destinationStackPosition, this, containerPosition, sourceStackPosition);
                }

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

                public Stack Take(object containerPosition, object stackPosition, object quantity)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    Stack found = Find(containerPosition, stackPosition);
                    if (found != null)
                    {
                        Stack result = found.Take(quantity);
                        if (result != null)
                        {
                            renderingStrategy.StackWasUpdated(containerPosition, stackPosition, found);
                        }
                    }
                    return null;
                }

                public bool Split(object sourceContainerPosition, object sourceStackPosition, object quantity,
                                  object newStackContainerPosition, object newStackPosition, out object finalNewStackPosition)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        Stack newStack = found.Take(quantity);
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

                public bool Use(object containerPosition, object sourceStackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    Stack found = Find(containerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        mainUsageStrategy.Use(found);
                        return true;
                    }
                    return false;
                }

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
                 * These methods go straight to the listener.
                 ******/

                public bool AddListener(MonoBehaviour listener)
                {
                    return renderingStrategy.AddListener(listener);
                }

                public bool RemoveListener(MonoBehaviour listener)
                {
                    return renderingStrategy.RemoveListener(listener);
                }

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

                /**
                 * Blinks a single stack on a container in the inventory.
                 */
                public void Blink(object containerPosition, object stackPosition)
                {
                    DoBlink(containerPosition, stackPosition, Find(containerPosition, stackPosition));
                }

                private void DoBlink(object containerPosition, IEnumerable<Tuple<object, Stack>> pairs)
                {
                    foreach (Tuple<object, Stack> pair in pairs)
                    {
                        DoBlink(containerPosition, pair.First, pair.Second);
                    }
                }

                /**
                 * Blinks all the stacks in a container inside the inventory.
                 */
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

                /**
                 * Blinks the whole inventory.
                 */
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

                public void Import(Types.Inventory.SerializedInventory serializedInventory)
                {
                    Clear();
                    foreach(KeyValuePair<object, Types.Inventory.SerializedContainer> containerPair in serializedInventory)
                    {
                        foreach(KeyValuePair<object, Types.Inventory.SerializedStack> stackPair in containerPair.Value)
                        {
                            ScriptableObjects.Inventory.Items.Item item = ScriptableObjects.Inventory.Items.ItemRegistry.GetItem(stackPair.Value.First, stackPair.Value.Second);
                            if (item != null)
                            {
                                object finalStackPosition;
                                Put(containerPair.Key, stackPair.Key, item.Create(stackPair.Value.Third, stackPair.Value.Fourth), out finalStackPosition);
                            }
                        }
                    }
                }

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
                            object stackPosition = stackPair.First;
                            Stack stack = stackPair.Second;
                            Tuple<ScriptableObjects.Inventory.Items.Item, object, object> dumped = stack.Dump();
                            serializedInventory[containerPosition][stackPosition] = new Types.Inventory.SerializedStack(dumped.First.Registry.Key, dumped.First.Key, dumped.Second, dumped.Third);
                        }
                    }
                    return serializedInventory;
                }
            }
        }
    }
}
