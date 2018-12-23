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

                private void Awake()
                {
                    positioningStrategy = GetComponent<ManagementStrategies.PositioningStrategies.InventoryPositioningManagementStrategy>();
                    spatialStrategy = GetComponent<ManagementStrategies.SpatialStrategies.InventorySpatialManagementStrategy>();
                    ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy[] usageStrategies = GetComponents<ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy>();
                    sortedUsageStrategies = (from component in Support.Utils.Layout.SortByDependencies(usageStrategies) select (component as ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy)).ToArray();
                    if (mainUsageStrategy == null || !(new HashSet<ManagementStrategies.UsageStrategies.InventoryUsageManagementStrategy>().Contains(mainUsageStrategy)))
                    {
                        Destroy(gameObject);
                        throw new InvalidStrategyComponentException("The selected strategy component must be non-null and present among the current pack manager's components");
                    }
                    renderingStrategy = GetComponent<ManagementStrategies.RenderingStrategies.InventoryRenderingManagementStrategy>();
                }

                private void CheckStackAcceptance(Stack stack)
                {
                    if (!stack.QuantifyingStrategy.HasAllowedQuantity())
                    {
                        throw new StackRejectedException(StackRejectedException.RejectionReason.InvalidQuantity);
                    }

                    if (!stack.Item.SpatialStrategy.GetType().IsSubclassOf(spatialStrategy.ItemSpatialStrategyCounterpartType))
                    {
                        throw new StackRejectedException(StackRejectedException.RejectionReason.IncompatibleSpatialStrategy);
                    }

                    if (!stack.MainUsageStrategy.GetType().IsSubclassOf(mainUsageStrategy.StackUsageStrategyCounterpartType))
                    {
                        throw new StackRejectedException(StackRejectedException.RejectionReason.IncompatibleUsageStrategy);
                    }
                }

                /*********************************************************************************************
                 *********************************************************************************************
                 * Inventory methods.
                 *********************************************************************************************
                 *********************************************************************************************/

                public IEnumerable<Tuple<object, Stack>> StackPairs(object containerPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.StackPairs(containerPosition);
                }

                public Stack Find(object containerPosition, object stackPosition)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.Find(containerPosition, stackPosition);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, Func<Tuple<object, Stack>, bool> predicate)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, predicate);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, ScriptableObjects.Inventory.Items.Item item)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindAll(containerPosition, item);
                }

                public Stack FindOne(object containerPosition, Func<Tuple<object, Stack>, bool> predicate)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, predicate);
                }

                public Stack FindOne(object containerPosition, ScriptableObjects.Inventory.Items.Item item)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    return spatialStrategy.FindOne(containerPosition, item);
                }

                public bool Put(object containerPosition, object stackPosition, Stack stack)
                {
                    positioningStrategy.CheckPosition(containerPosition);
                    bool result = spatialStrategy.Put(containerPosition, stackPosition, stack);
                    if (result)
                    {
                        renderingStrategy.StackWasUpdated(containerPosition, stackPosition, stack);
                    }
                    return result;
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
                                  object newStackContainerPosition, object newStackPosition)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        Stack newStack = found.Take(quantity);
                        if (!Put(newStackContainerPosition, newStackPosition, newStack))
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
                        pairs = spatialStrategy.StackPairs(containerPosition);
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
                                Put(containerPair.Key, stackPair.Key, item.Create(stackPair.Value.Third, stackPair.Value.Fourth));
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

                        foreach(Tuple<object, Stack> stackPair in spatialStrategy.StackPairs(containerPosition))
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
