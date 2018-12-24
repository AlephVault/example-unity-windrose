using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                public class Stack
                {
                    /**
                     * A stack is the only implementation of PackHeld
                     *   since it will be held by the pack BUT will also
                     *   define all the needed strategies.
                     * 
                     * It will refer an item, and will hold paralell
                     *   strategies created by the item (which aside
                     *   from being the reference item, it is a factory).
                     */

                    private UsageStrategies.StackUsageStrategy[] usageStrategies;
                    private Dictionary<Type, UsageStrategies.StackUsageStrategy> usageStrategiesByType;
                    public UsageStrategies.StackUsageStrategy MainUsageStrategy
                    {
                        get; private set;
                    }

                    private RenderingStrategies.StackRenderingStrategy[] renderingStrategies;
                    private Dictionary<Type, RenderingStrategies.StackRenderingStrategy> renderingStrategiesByType;
                    public RenderingStrategies.StackRenderingStrategy MainRenderingStrategy
                    {
                        get; private set;
                    }

                    /**
                     * Tools to get a component strategy, as we have in BundledTiles.
                     */

                    public QuantifyingStrategies.StackQuantifyingStrategy QuantifyingStrategy
                    {
                        get; private set;
                    }

                    public SpatialStrategies.StackSpatialStrategy SpatialStrategy
                    {
                        get; private set;
                    }

                    public ScriptableObjects.Inventory.Items.Item Item
                    {
                        get; private set;
                    }

                    public T GetUsageStrategy<T>() where T : UsageStrategies.StackUsageStrategy
                    {
                        return usageStrategiesByType[typeof(T)] as T;
                    }

                    public T GetRenderingStrategy<T>() where T : RenderingStrategies.StackRenderingStrategy
                    {
                        return renderingStrategiesByType[typeof(T)] as T;
                    }

                    /**
                     * In this constructor I won't need to compute dependencies, since they are already assumed
                     *   and sorted from the items when the constructor is invoked.
                     */

                    public Stack(ScriptableObjects.Inventory.Items.Item item,
                                 QuantifyingStrategies.StackQuantifyingStrategy quantifyingStrategy,
                                 SpatialStrategies.StackSpatialStrategy spatialStrategy,
                                 UsageStrategies.StackUsageStrategy[] usageStrategies,
                                 UsageStrategies.StackUsageStrategy mainUsageStrategy,
                                 RenderingStrategies.StackRenderingStrategy[] renderingStrategies,
                                 RenderingStrategies.StackRenderingStrategy mainRenderingStrategy)
                    {
                        Item = item;
                        QuantifyingStrategy = quantifyingStrategy;
                        SpatialStrategy = spatialStrategy;
                        this.usageStrategies = usageStrategies;
                        this.renderingStrategies = renderingStrategies;
                        MainUsageStrategy = mainUsageStrategy;
                        MainRenderingStrategy = mainRenderingStrategy;
                    }

                    /**
                     * Export will not account for rendering strategies.
                     */
                    public Support.Types.Tuple<ScriptableObjects.Inventory.Items.Item, object, object> Dump()
                    {
                        return new Support.Types.Tuple<ScriptableObjects.Inventory.Items.Item, object, object>(Item, QuantifyingStrategy.Quantity, MainUsageStrategy.Export());
                    }

                    /**
                     * All the public methods go here.
                     */

                    /**
                     * Clones the stack almost entirely - only quantity is not included but passed externally.
                     */
                    private Stack Clone(QuantifyingStrategies.StackQuantifyingStrategy quantifyingStrategy)
                    {
                        UsageStrategies.StackUsageStrategy[] clonedUsageStrategies = new UsageStrategies.StackUsageStrategy[usageStrategies.Length];
                        UsageStrategies.StackUsageStrategy clonedMainUsageStrategy = null;
                        RenderingStrategies.StackRenderingStrategy[] clonedRenderingStrategies = new RenderingStrategies.StackRenderingStrategy[renderingStrategies.Length];
                        RenderingStrategies.StackRenderingStrategy clonedMainRenderingStrategy = null;
                        int index = 0;
                        foreach (UsageStrategies.StackUsageStrategy strategy in usageStrategies)
                        {
                            clonedUsageStrategies[index] = strategy.Clone();
                            if (MainUsageStrategy == strategy)
                            {
                                // We know that MainUsageStrategy will enter this condition at least once.
                                // Otherwise, we should need to call clonedMainUsageStrategy = MainUsageStrategy.Clone()
                                //   but outside the loop.
                                clonedMainUsageStrategy = clonedUsageStrategies[index];
                            }
                            index++;
                        }

                        index = 0;
                        foreach (RenderingStrategies.StackRenderingStrategy strategy in renderingStrategies)
                        {
                            clonedRenderingStrategies[index] = strategy.Clone();
                            if (MainRenderingStrategy == strategy)
                            {
                                // We know that MainRenderingStrategy will enter this condition at least once.
                                // Otherwise, we should need to call clonedMainRenderingStrategy = MainRenderingStrategy.Clone()
                                //   but outside the loop.
                                clonedMainRenderingStrategy = clonedRenderingStrategies[index];
                            }
                            index++;
                        }

                        return new Stack(Item, quantifyingStrategy, SpatialStrategy.Clone(),
                                         clonedUsageStrategies, clonedMainUsageStrategy,
                                         clonedRenderingStrategies, clonedMainRenderingStrategy);
                    }

                    /**
                     * Clones the stack, entirely.
                     */
                    public Stack Clone()
                    {
                        return Clone(QuantifyingStrategy.Clone());
                    }

                    /**
                     * Clones the stack, but with a different quantity. The stack will not be bound to any inventory.
                     */
                    public Stack Clone(object quantity)
                    {
                        return Clone(QuantifyingStrategy.Clone(quantity));
                    }

                    /**
                     * Checks whether this stack has an allowed (in-constraints) nonzero
                     *   quantity. Stacks not being able to satisfy this condition will not
                     *   be added to an inventory.
                     */
                    public bool IsAllowedNonZeroQuantity()
                    {
                        return QuantifyingStrategy.HasAllowedQuantity() && !QuantifyingStrategy.IsEmpty();
                    }

                    /**
                     * Tries to take part of the stack, defined by a quantity. It does not allow taking
                     *   the whole stack, but just part of it.
                     */
                    public Stack Take(object quantity)
                    {
                        if (QuantifyingStrategy.ChangeQuantityBy(quantity, true, true))
                        {
                            return Clone(quantity);
                        }
                        return null;
                    }

                    /**
                     * Tries to merge a stack into another.
                     * 
                     * Please consider the following notes: This method does not affect the source stack
                     *   but instead affects the target stack. This means: without the cares of manually
                     *   handling the source stack later, you could end with twice the expected amount
                     *   somewhere.
                     * 
                     * The result of the merge may be:
                     * 1. Denied: This may occur because the underlying item of the stacks is not the
                     *    same in both cases, the usage strategies are not mergeable (either by their
                     *    nature or by their circumstance), the quantity of either stacks is invalid
                     *    (i.e. non-positive) or the quantity of the destination stack is full.
                     * 2. Partial: The merge was successful but not with the whole quantity. This means
                     *    that the destination stack filled up and 
                     * 
                     * As an output parameter, you get the quantity left on the source stack. You should
                     *   explicitly set such value in the source stack by calling the following method:
                     *   -> source.ChangeQuantityTo(quantityLeft)
                     * However this will vary depending on your needs.
                     */
                    public enum MergeResult { Denied, Partial, Total }
                    public MergeResult Merge(Stack source, out object quantityLeft)
                    {
                        // preset to null so we can leave control safely
                        quantityLeft = null;

                        // this one would tell the quantity effectively added to, and final in, the stack
                        object quantityAdded = null;
                        object finalQuantity = null;

                        if (Item != source.Item || IsFull() || !IsAllowedNonZeroQuantity() || !source.IsAllowedNonZeroQuantity())
                        {
                            return MergeResult.Denied;
                        }

                        // We test saturation to know which quantities to add
                        bool saturates = QuantifyingStrategy.WillSaturate(source.QuantifyingStrategy.Quantity, out finalQuantity, out quantityAdded, out quantityLeft);

                        /*
                         * This will happen now:
                         * 1. The spatial strategy will not be affected.
                         * 2. The quantifying strategy will be set to the final quantity.
                         * 3. The rendering strategies will not be affected.
                         * 4. The usage strategies will behave differently:
                         */

                        // Now we compute the interpolations for each usagestrategy (stacks will have them
                        //   in the same order) by manually zipping everything.
                        int index = 0;
                        Action[] interpolators = new Action[usageStrategies.Length];
                        foreach(UsageStrategies.StackUsageStrategy usageStrategy in usageStrategies)
                        {
                            Action interpolator = usageStrategy.Interpolate(source.usageStrategies[index], QuantifyingStrategy.Quantity, quantityAdded);
                            if (interpolator == null)
                            {
                                // If at least an interpolator fails, we abort everything.
                                return MergeResult.Denied;
                            }
                            interpolators[index++] = interpolator;
                        }

                        // Now we run all the interpolators.
                        foreach(Action interpolator in interpolators)
                        {
                            interpolator();
                        }

                        // We reached this point because all the interpolators have been found.
                        // If you coded the saturation method appropriately, this will work.
                        QuantifyingStrategy.ChangeQuantityTo(finalQuantity, true);

                        // We are ok with this.
                        return saturates ? MergeResult.Partial : MergeResult.Total;
                    }

                    /**
                     * Tells whether this stack equals the other stack. This will be checked
                     *   in terms of the usage strategies, and not in terms of quantity or
                     *   (spatial) position.
                     */
                    public bool Equals(Stack otherStack)
                    {
                        if (Item == otherStack.Item)
                        {
                            int index = 0;
                            foreach(UsageStrategies.StackUsageStrategy usageStrategy in usageStrategies)
                            {
                                if (!usageStrategy.Equals(otherStack.usageStrategies[index++]))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                        return false;
                    }

                    /**
                     * Checks whether the quantity is full.
                     */
                    public bool IsFull()
                    {
                        return QuantifyingStrategy.IsFull();
                    }

                    /**
                     * Changes the underlying quantity by certain amount.
                     */
                    public bool ChangeQuantityBy(object quantity)
                    {
                        return QuantifyingStrategy.ChangeQuantityBy(quantity, false, false);
                    }

                    /**
                     * Changes the underlying quantity to certain amount.
                     */
                    public bool ChangeQuantityTo(object quantity)
                    {
                        return QuantifyingStrategy.ChangeQuantityTo(quantity, false);
                    }
                }
            }
        }
    }
}
