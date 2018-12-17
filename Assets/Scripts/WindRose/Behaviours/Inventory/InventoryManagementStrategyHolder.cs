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
                 * A spatial strategy needed to tell how does the inventory locates its items. Will be fetched
                 *   from the added components, and only ONE will be allowed.
                 */
                private ManagementStrategies.SpatialStrategies.SpatialInventoryManagementStrategy spatialStrategy;

                /**
                 * Many usage strategies needed to tell how does the inventory uses/interacts-with the stacks.
                 *   They will be fetched from the added components, and will be sorted dependency-wise.
                 */
                private ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy[] sortedUsageStrategies;

                /**
                 * A data marshalling strategy will only be needed if you intend to serialize/deserialize data
                 *   from a kind of storage or network interaction.
                 */
                private ManagementStrategies.DataMarshallingManagementStrategies.DataMarshallingManagementStrategy dataMarshallingStrategy;

                /**
                 * This is the main usage strategy this holder will have. This one is required, and must be present
                 *   among the components.
                 */
                [SerializeField]
                private ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy mainUsageStrategy;

                private void Awake()
                {
                    spatialStrategy = GetComponent<ManagementStrategies.SpatialStrategies.SpatialInventoryManagementStrategy>();
                    dataMarshallingStrategy = GetComponent<ManagementStrategies.DataMarshallingManagementStrategies.DataMarshallingManagementStrategy>();
                    ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy[] usageStrategies = GetComponents<ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy>();
                    sortedUsageStrategies = (from component in Support.Utils.Layout.SortByDependencies(usageStrategies) select (component as ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy)).ToArray();
                    if (mainUsageStrategy == null || !(new HashSet<ManagementStrategies.UsageStrategies.UsageInventoryManagementStrategy>().Contains(mainUsageStrategy)))
                    {
                        Destroy(gameObject);
                        throw new InvalidStrategyComponentException("The selected strategy component must be non-null and present among the current pack manager's components");
                    }
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
                    return spatialStrategy.StackPairs(containerPosition);
                }

                public Stack Find(object containerPosition, object stackPosition)
                {
                    return spatialStrategy.Find(containerPosition, stackPosition);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, Func<Tuple<object, Stack>, bool> predicate)
                {
                    return spatialStrategy.FindAll(containerPosition, predicate);
                }

                public IEnumerable<Stack> FindAll(object containerPosition, ScriptableObjects.Inventory.Items.Item item)
                {
                    return spatialStrategy.FindAll(containerPosition, item);
                }

                public Stack FindOne(object containerPosition, Func<Tuple<object, Stack>, bool> predicate)
                {
                    return spatialStrategy.FindOne(containerPosition, predicate);
                }

                public Stack FindOne(object containerPosition, ScriptableObjects.Inventory.Items.Item item)
                {
                    return spatialStrategy.FindOne(containerPosition, item);
                }

                public bool Put(object containerPosition, object stackPosition, Stack stack)
                {
                    return spatialStrategy.Put(containerPosition, stackPosition, stack);
                    // TODO: rendering events on true.
                }

                public bool Remove(object containerPosition, object stackPosition)
                {
                    return spatialStrategy.Remove(containerPosition, stackPosition);
                    // TODO: rendering events on true.
                }

                public bool Merge(object containerPosition, object destinationStackPosition, object sourceStackPosition)
                {
                    return Merge(containerPosition, destinationStackPosition, this, containerPosition, sourceStackPosition);
                }

                public bool Merge(object destinationContainerPosition, object destinationStackPosition,
                                  InventoryManagementStrategyHolder sourceHolder, object sourceContainerPosition, object sourceStackPosition)
                {
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
                            // TODO refresh destination stack
                            // TODO delete source stack
                            // TODO trigger both rendering events
                        }
                        else
                        {
                            source.ChangeQuantityTo(quantityLeft);
                            // TODO refresh destination stack
                            // TODO refresh source stack
                            // TODO trigger both rendering events
                        }
                        return true;
                    }

                    return false;
                }

                public Stack Take(object sourceContainerPosition, object sourceStackPosition, object quantity)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        return found.Take(quantity);
                        // TODO: rendering events on non-null.
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
                            // TODO: rendering events on true.
                            return true;
                        }
                    }
                    return false;
                }

                public bool Use(object sourceContainerPosition, object sourceStackPosition)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        mainUsageStrategy.Use(found);
                        return true;
                    }
                    return false;
                }

                public bool Use(object sourceContainerPosition, object sourceStackPosition, object argument)
                {
                    Stack found = Find(sourceContainerPosition, sourceStackPosition);
                    if (found != null)
                    {
                        mainUsageStrategy.Use(found, argument);
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
