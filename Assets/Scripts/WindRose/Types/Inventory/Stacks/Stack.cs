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
                public class Stack : Pack.PackHeld
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

                    private QuantifyingStrategies.StackQuantifyingStrategy quantifyingStrategy;
                    private SpatialStrategies.StackSpatialStrategy spatialStrategy;
                    private UsageStrategies.StackUsageStrategy[] usageStrategies;
                    private Dictionary<Type, UsageStrategies.StackUsageStrategy> usageStrategiesByType;
                    private UsageStrategies.StackUsageStrategy mainUsageStrategy;
                    public UsageStrategies.StackUsageStrategy MainUsageStrategy
                    {
                        get
                        {
                            return mainUsageStrategy;
                        }
                    }

                    private RenderingStrategies.StackRenderingStrategy[] renderingStrategies;
                    private Dictionary<Type, RenderingStrategies.StackRenderingStrategy> renderingStrategiesByType;
                    private RenderingStrategies.StackRenderingStrategy mainRenderingStrategy;
                    public RenderingStrategies.StackRenderingStrategy MainRenderingStrategy
                    {
                        get
                        {
                            return mainRenderingStrategy;
                        }
                    }

                    /**
                     * Tools to get a component strategy, as we have in BundledTiles.
                     */

                    public QuantifyingStrategies.StackQuantifyingStrategy QuantifyingStrategy
                    {
                        get
                        {
                            return quantifyingStrategy;
                        }
                    }

                    public SpatialStrategies.StackSpatialStrategy SpatialStrategy
                    {
                        get
                        {
                            return spatialStrategy;
                        }
                    }

                    private ScriptableObjects.Inventory.Items.Item item;
                    public ScriptableObjects.Inventory.Items.Item Item
                    {
                        get
                        {
                            return item;
                        }
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
                                 Type mainUsageStrategyType,
                                 RenderingStrategies.StackRenderingStrategy[] renderingStrategies,
                                 Type mainRenderingStrategyType)
                    {
                        this.item = item;
                        this.quantifyingStrategy = quantifyingStrategy;
                        this.spatialStrategy = spatialStrategy;
                        this.usageStrategies = usageStrategies;
                        this.renderingStrategies = renderingStrategies;
                        usageStrategiesByType = new Dictionary<Type, UsageStrategies.StackUsageStrategy>();
                        foreach(UsageStrategies.StackUsageStrategy strategy in usageStrategies)
                        {
                            usageStrategiesByType[strategy.GetType()] = strategy;
                        }
                        mainUsageStrategy = usageStrategiesByType[mainUsageStrategyType];
                        renderingStrategiesByType = new Dictionary<Type, RenderingStrategies.StackRenderingStrategy>();
                        foreach(RenderingStrategies.StackRenderingStrategy strategy in renderingStrategies)
                        {
                            renderingStrategiesByType[strategy.GetType()] = strategy;
                        }
                        mainRenderingStrategy = renderingStrategiesByType[mainRenderingStrategyType];
                    }

                    private void MergeInto(Dictionary<string, object> target, Dictionary<string, object> source)
                    {
                        foreach(string key in source.Keys)
                        {
                            target[key] = source[key];
                        }
                    }

                    /**
                     * Export will not account for rendering strategies.
                     */
                    public Dictionary<string, object> Export()
                    {
                        Dictionary<string, object> result = new Dictionary<string, object>();
                        MergeInto(result, quantifyingStrategy.Export());
                        MergeInto(result, spatialStrategy.Export());
                        foreach(UsageStrategies.StackUsageStrategy strategy in usageStrategies)
                        {
                            MergeInto(result, strategy.Export());
                        }
                        return result;
                    }
                }
            }
        }
    }
}
