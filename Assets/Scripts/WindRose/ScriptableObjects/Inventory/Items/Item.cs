using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                using Support.Utils;
                using Types.Inventory.Stacks;
                using Types.Inventory.Stacks.QuantifyingStrategies;
                using Types.Inventory.Stacks.RenderingStrategies;
                using Types.Inventory.Stacks.SpatialStrategies;
                using Types.Inventory.Stacks.UsageStrategies;

                [CreateAssetMenu(fileName = "NewBundledTile", menuName = "Wind Rose/Inventory/Item", order = 201)]
                public class Item : ScriptableObject
                {
                    /**
                     * An inventory item. Will have the following strategies:
                     * - One spatial strategy.
                     * - One quantifying strategy.
                     * - Many usage strategies.
                     * - Many rendering strategies.
                     */

                    [SerializeField]
                    private QuantifyingStrategies.ItemQuantifyingStrategy quantifyingStrategy;

                    [SerializeField]
                    private SpatialStrategies.ItemSpatialStrategy spatialStrategy;

                    [SerializeField]
                    private UsageStrategies.ItemUsageStrategy[] usageStrategies;
                    private UsageStrategies.ItemUsageStrategy[] sortedUsageStrategies;
                    private Dictionary<Type, UsageStrategies.ItemUsageStrategy> usageStrategiesByType;

                    [SerializeField]
                    private UsageStrategies.ItemUsageStrategy mainUsageStrategy;
                    public UsageStrategies.ItemUsageStrategy MainUsageStrategy
                    {
                        get
                        {
                            return mainUsageStrategy;
                        }
                    }

                    [SerializeField]
                    private RenderingStrategies.ItemRenderingStrategy[] renderingStrategies;
                    private RenderingStrategies.ItemRenderingStrategy[] sortedRenderingStrategies;
                    private Dictionary<Type, RenderingStrategies.ItemRenderingStrategy> renderingStrategiesByType;

                    [SerializeField]
                    private RenderingStrategies.ItemRenderingStrategy mainRenderingStrategy;
                    public RenderingStrategies.ItemRenderingStrategy MainRenderingStrategy
                    {
                        get
                        {
                            return mainRenderingStrategy;
                        }
                    }

                    [SerializeField]
                    private DataLoadingStrategies.DataLoadingStrategy dataLoadingStrategy;

                    private void Awake()
                    {
                        try
                        {
                            // Flatten (and check!) dependencies among all of them
                            sortedUsageStrategies = AssetsLayout.FlattenDependencies<UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(usageStrategies, true);
                            sortedRenderingStrategies = AssetsLayout.FlattenDependencies<RenderingStrategies.ItemRenderingStrategy, RequireRenderingStrategy>(renderingStrategies, true);
                            // Avoid duplicate dependencies and also check interdependencies
                            renderingStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(sortedRenderingStrategies);
                            usageStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(usageStrategies);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(sortedRenderingStrategies, quantifyingStrategy);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(sortedRenderingStrategies, usageStrategies);
                            // Check both main strategies
                            AssetsLayout.CheckMainComponent(usageStrategies, mainUsageStrategy);
                            AssetsLayout.CheckMainComponent(renderingStrategies, mainRenderingStrategy);
                            // Finally: Check dependencies for the DataLoadingStrategy
                            AssetsLayout.CheckPresence(dataLoadingStrategy, "dataLoadingStrategy");
                            AssetsLayout.CrossCheckDependencies<DataLoadingStrategies.DataLoadingStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(dataLoadingStrategy, quantifyingStrategy);
                            AssetsLayout.CrossCheckDependencies<DataLoadingStrategies.DataLoadingStrategy, SpatialStrategies.ItemSpatialStrategy, RequireSpatialStrategy>(dataLoadingStrategy, spatialStrategy);
                            AssetsLayout.CrossCheckDependencies<DataLoadingStrategies.DataLoadingStrategy, UsageStrategies.ItemUsageStrategy, RequireQuantifyingStrategy>(dataLoadingStrategy, usageStrategies);
                            AssetsLayout.CrossCheckDependencies<DataLoadingStrategies.DataLoadingStrategy, RenderingStrategies.ItemRenderingStrategy, RequireRenderingStrategy>(dataLoadingStrategy, renderingStrategies);
                        }
                        catch (Exception)
                        {
                            Resources.UnloadAsset(this);
                        }
                    }

                    /**
                     * Tools to get a component strategy.
                     */

                    public QuantifyingStrategies.ItemQuantifyingStrategy QuantifyingStrategy
                    {
                        get
                        {
                            return quantifyingStrategy;
                        }
                    }

                    public SpatialStrategies.ItemSpatialStrategy SpatialStrategy
                    {
                        get
                        {
                            return spatialStrategy;
                        }
                    }

                    public T GetUsageStrategy<T>() where T : UsageStrategies.ItemUsageStrategy
                    {
                        return usageStrategiesByType[typeof(T)] as T;
                    }

                    public T GetRenderingStrategy<T>() where T : RenderingStrategies.ItemRenderingStrategy
                    {
                        return renderingStrategiesByType[typeof(T)] as T;
                    }

                    public Stack Create(object argument)
                    {
                        /*
                         * Creating children strategies.
                         */
                        int index;
                        StackQuantifyingStrategy stackQuantifyingStrategy = quantifyingStrategy.CreateStackStrategy(dataLoadingStrategy.LoadDataFor(quantifyingStrategy, argument));
                        StackSpatialStrategy stackSpatialStrategy = spatialStrategy.CreateStackStrategy(dataLoadingStrategy.LoadDataFor(spatialStrategy, argument));
                        StackUsageStrategy[] stackUsageStrategies = new StackUsageStrategy[sortedUsageStrategies.Length];
                        StackUsageStrategy mainStackUsageStrategy = null;
                        index = 0;
                        foreach(UsageStrategies.ItemUsageStrategy usageStrategy in sortedUsageStrategies)
                        {
                            StackUsageStrategy stackUsageStrategy = usageStrategy.CreateStackStrategy(dataLoadingStrategy.LoadDataFor(usageStrategy, argument));
                            stackUsageStrategies[index] = stackUsageStrategy;
                            if (usageStrategy == mainUsageStrategy) mainStackUsageStrategy = stackUsageStrategy;
                            index++;
                        }
                        StackRenderingStrategy[] stackRenderingStrategies = new StackRenderingStrategy[sortedRenderingStrategies.Length];
                        StackRenderingStrategy mainStackRenderingStrategy = null;
                        index = 0;
                        foreach (RenderingStrategies.ItemRenderingStrategy renderingStrategy in sortedRenderingStrategies)
                        {
                            StackRenderingStrategy stackRenderingStrategy = renderingStrategy.CreateStackStrategy(dataLoadingStrategy.LoadDataFor(renderingStrategy, argument));
                            stackRenderingStrategies[index] = stackRenderingStrategy;
                            if (renderingStrategy == mainRenderingStrategy) mainStackRenderingStrategy = stackRenderingStrategy;
                            index++;
                        }

                        /*
                         * Creating the stack with the strategies.
                         */
                        Stack stack = new Stack(
                            this, stackQuantifyingStrategy, stackSpatialStrategy, stackUsageStrategies, mainStackUsageStrategy, stackRenderingStrategies, mainStackRenderingStrategy, dataLoadingStrategy.CreateStackStrategy(null)
                        );

                        /*
                         * Initializing the stack strategies.
                         */
                        stackQuantifyingStrategy.Initialize(stack);
                        stackSpatialStrategy.Initialize(stack);
                        foreach(StackUsageStrategy strategy in stackUsageStrategies)
                        {
                            strategy.Initialize(stack);
                        }
                        foreach(StackRenderingStrategy strategy in stackRenderingStrategies)
                        {
                            strategy.Initialize(stack);
                        }
                        return stack;
                    }
                }
            }
        }
    }
}
