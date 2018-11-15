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

                    private void Awake()
                    {
                        try
                        {
                            // Flatten (and check!) dependencies among all of them
                            sortedUsageStrategies = AssetsLayout.FlattenDependencies<UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(usageStrategies, true);
                            sortedRenderingStrategies = AssetsLayout.FlattenDependencies<RenderingStrategies.ItemRenderingStrategy, RequireRenderingStrategy>(renderingStrategies, true);
                            // Avoid duplicate dependencies and also check interdependencies
                            renderingStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(sortedRenderingStrategies);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(sortedRenderingStrategies, quantifyingStrategy);
                            usageStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(usageStrategies);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(sortedRenderingStrategies, usageStrategies);
                            // Check both main strategies
                            AssetsLayout.CheckMainComponent(usageStrategies, mainUsageStrategy);
                            AssetsLayout.CheckMainComponent(renderingStrategies, mainRenderingStrategy);
                        }
                        catch(Exception)
                        {
                            Resources.UnloadAsset(this);
                        }
                    }

                    /**
                     * Tools to get a component strategy, as we have in BundledTiles.
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

                    public Stack Create(Dictionary<string, object> arguments)
                    {
                        StackQuantifyingStrategy stackQuantifyingStrategy = quantifyingStrategy.Create(this, arguments);
                        StackSpatialStrategy stackSpatialStrategy = spatialStrategy.Create(this, arguments);
                        StackUsageStrategy[] stackUsageStrategies = (from strategy in sortedUsageStrategies select strategy.Create(this, arguments)).ToArray();
                        StackRenderingStrategy[] stackRenderingStrategies = (from strategy in sortedRenderingStrategies select strategy.Create(this)).ToArray();
                        Stack stack = new Stack(
                            this, stackQuantifyingStrategy, stackSpatialStrategy, stackUsageStrategies, mainUsageStrategy.GetType(), stackRenderingStrategies, mainRenderingStrategy.GetType()
                        );
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
