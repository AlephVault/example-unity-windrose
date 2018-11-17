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

                    public DataDumpingStrategies.DataDumpingStrategy DataDumpingStrategy
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
                                 RenderingStrategies.StackRenderingStrategy mainRenderingStrategy,
                                 DataDumpingStrategies.DataDumpingStrategy dataDumpingStrategy)
                    {
                        Item = item;
                        QuantifyingStrategy = quantifyingStrategy;
                        SpatialStrategy = spatialStrategy;
                        this.usageStrategies = usageStrategies;
                        this.renderingStrategies = renderingStrategies;
                        MainUsageStrategy = mainUsageStrategy;
                        MainRenderingStrategy = mainRenderingStrategy;
                        DataDumpingStrategy = dataDumpingStrategy;
                    }

                    /**
                     * Export will not account for rendering strategies.
                     */
                    public void Dump(object target)
                    {
                        DataDumpingStrategy.DumpDataFor(QuantifyingStrategy, QuantifyingStrategy.Export(), target);
                        DataDumpingStrategy.DumpDataFor(SpatialStrategy, SpatialStrategy.Export(), target);
                        foreach(UsageStrategies.StackUsageStrategy usageStrategy in usageStrategies)
                        {
                            DataDumpingStrategy.DumpDataFor(usageStrategy, usageStrategy.Export(), target);
                        }
                        foreach(RenderingStrategies.StackRenderingStrategy renderingStrategy in renderingStrategies)
                        {
                            DataDumpingStrategy.DumpDataFor(renderingStrategy, renderingStrategy.Export(), target);
                        }
                    }
                }
            }
        }
    }
}
