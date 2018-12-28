using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

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
                using Types.Inventory.Stacks.UsageStrategies;

                [CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Wind Rose/Inventory/Item", order = 201)]
                public class Item : ScriptableObject
                {
                    /**
                     * An inventory item. Will have the following strategies:
                     * - One spatial strategy.
                     * - One quantifying strategy.
                     * - Many usage strategies.
                     * - Many rendering strategies.
                     * 
                     * It will also be able to, optionally, relate to a registry.
                     */

                    public bool Attached
                    {
                        get; private set;
                    }

                    [SerializeField]
                    private ItemRegistry registry;
                    public ItemRegistry Registry
                    {
                        get { return registry; }
                    }

                    [SerializeField]
                    private uint key;
                    public uint Key
                    {
                        get { return key; }
                    }

                    [SerializeField]
                    private QuantifyingStrategies.ItemQuantifyingStrategy quantifyingStrategy;
                    public QuantifyingStrategies.ItemQuantifyingStrategy QuantifyingStrategy
                    {
                        get { return quantifyingStrategy; }
                    }

                    [SerializeField]
                    private SpatialStrategies.ItemSpatialStrategy[] spatialStrategies;
                    private Dictionary<Type, SpatialStrategies.ItemSpatialStrategy> spatialStrategiesByType;

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

                    private void OnEnable()
                    {
                        try
                        {
                            if (registry != null && key != 0)
                            {
                                registry.Init();
                                Attached = registry.AddItem(this);
                            }

                            // Flatten (and check!) dependencies among all of them
                            sortedUsageStrategies = AssetsLayout.FlattenDependencies<UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(usageStrategies, true);
                            sortedRenderingStrategies = AssetsLayout.FlattenDependencies<RenderingStrategies.ItemRenderingStrategy, RequireRenderingStrategy>(renderingStrategies, true);
                            // Avoid duplicate dependencies and also check interdependencies
                            renderingStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(sortedRenderingStrategies);
                            usageStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(usageStrategies);
                            spatialStrategiesByType = AssetsLayout.AvoidDuplicateDependencies(spatialStrategies);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(sortedRenderingStrategies, quantifyingStrategy);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(sortedRenderingStrategies, usageStrategies);
                            // Check both main strategies
                            AssetsLayout.CheckMainComponent(usageStrategies, mainUsageStrategy);
                            AssetsLayout.CheckMainComponent(renderingStrategies, mainRenderingStrategy);
                        }
                        catch (Exception exc)
                        {
                            Debug.Log(string.Format("Item::OnEnable() threw: {0}", exc));
                        }
                    }

                    /**
                     * Tools to get a component strategy.
                     */

                    public T GetSpatialStrategy<T>() where T : SpatialStrategies.ItemSpatialStrategy
                    {
                        return GetSpatialStrategy(typeof(T)) as T;
                    }

                    public SpatialStrategies.ItemSpatialStrategy GetSpatialStrategy(Type type)
                    {
                        SpatialStrategies.ItemSpatialStrategy spatialStrategy;
                        spatialStrategiesByType.TryGetValue(type, out spatialStrategy);
                        return spatialStrategy;
                    }

                    public T GetUsageStrategy<T>() where T : UsageStrategies.ItemUsageStrategy
                    {
                        return GetUsageStrategy(typeof(T)) as T;
                    }

                    public UsageStrategies.ItemUsageStrategy GetUsageStrategy(Type type)
                    {
                        UsageStrategies.ItemUsageStrategy usageStrategy;
                        usageStrategiesByType.TryGetValue(type, out usageStrategy);
                        return usageStrategy;
                    }

                    public T GetRenderingStrategy<T>() where T : RenderingStrategies.ItemRenderingStrategy
                    {
                        return GetRenderingStrategy(typeof(T)) as T;
                    }

                    public RenderingStrategies.ItemRenderingStrategy GetRenderingStrategy(Type type)
                    {
                        RenderingStrategies.ItemRenderingStrategy renderingStrategy;
                        renderingStrategiesByType.TryGetValue(type, out renderingStrategy);
                        return renderingStrategy;
                    }

                    public Stack Create(object quantity, object argument)
                    {
                        /*
                         * Creating children strategies. Spatial and rendering strategies need no arguments since spatial strategies
                         *   actually depend on what an inventory determines, and rendering strategies do not have own data.
                         */
                        int index;
                        StackQuantifyingStrategy stackQuantifyingStrategy = quantifyingStrategy.CreateStackStrategy(quantity);
                        // StackSpatialStrategy stackSpatialStrategy = spatialStrategy.CreateStackStrategy();
                        StackUsageStrategy[] stackUsageStrategies = new StackUsageStrategy[sortedUsageStrategies.Length];
                        StackUsageStrategy mainStackUsageStrategy = null;
                        index = 0;
                        foreach(UsageStrategies.ItemUsageStrategy usageStrategy in sortedUsageStrategies)
                        {
                            // Perhaps by misconfiguration there are null slots here
                            if (usageStrategy != null)
                            {
                                StackUsageStrategy stackUsageStrategy = usageStrategy.CreateStackStrategy();
                                stackUsageStrategies[index] = stackUsageStrategy;
                                if (usageStrategy == mainUsageStrategy) mainStackUsageStrategy = stackUsageStrategy;
                                index++;
                            }
                        }

                        if (MainUsageStrategy != null)
                        {
                            mainStackUsageStrategy.Import(argument);
                        }

                        StackRenderingStrategy[] stackRenderingStrategies = new StackRenderingStrategy[sortedRenderingStrategies.Length];
                        StackRenderingStrategy mainStackRenderingStrategy = null;
                        index = 0;
                        foreach (RenderingStrategies.ItemRenderingStrategy renderingStrategy in sortedRenderingStrategies)
                        {
                            // Perhaps by misconfiguration there are null slos here
                            if (renderingStrategy != null)
                            {
                                StackRenderingStrategy stackRenderingStrategy = renderingStrategy.CreateStackStrategy();
                                stackRenderingStrategies[index] = stackRenderingStrategy;
                                if (renderingStrategy == mainRenderingStrategy) mainStackRenderingStrategy = stackRenderingStrategy;
                                index++;
                            }
                        }

                        /*
                         * Creating the stack with the strategies.
                         */
                        Stack stack = new Stack(
                            this, stackQuantifyingStrategy, stackUsageStrategies, mainStackUsageStrategy, stackRenderingStrategies, mainStackRenderingStrategy
                        );

                        return stack;
                    }
                }
            }
        }
    }
}
