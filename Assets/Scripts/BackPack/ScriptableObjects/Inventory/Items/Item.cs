using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BackPack
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                using UnityEngine;
                using GMM.Utils;
                using Types.Inventory.Stacks;
                using Types.Inventory.Stacks.QuantifyingStrategies;
                using Types.Inventory.Stacks.RenderingStrategies;
                using Types.Inventory.Stacks.UsageStrategies;

                /// <summary>
                ///   <para>
                ///     Inventory items provide semantic for the inventory
                ///       item render, occupancy, quantity and usage logic.
                ///   </para>
                ///   <para>
                ///     Stacks will be created from these objects, and will
                ///       have meaningful (related) strategies to interact
                ///       in the game.
                ///   </para>
                /// </summary>
                [CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Wind Rose/Inventory/Item", order = 201)]
                public class Item : ScriptableObject
                {
                    /// <summary>
                    ///   <para>
                    ///     This method is mostly internal and it will seldom needed.
                    ///   </para>
                    ///   <para>
                    ///     Tells whether this item is attached to any kind of item registry.
                    ///   </para>
                    /// </summary>
                    /// <remarks>
                    ///   You may disregard the existence of registries and not use them
                    ///     in your game. They are not needed, but useful for complex games.
                    /// </remarks>
                    public bool Attached
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   <para>
                    ///     The registry you MAY attach this item to.
                    ///   </para>
                    /// </summary>
                    /// <remarks>
                    ///   You may disregard the existence of registries and not use them
                    ///     in your game. They are not needed, but useful for complex games.
                    /// </remarks>
                    [SerializeField]
                    private ItemRegistry registry;

                    /// <summary>
                    ///   See <see cref="registry"/>.
                    /// </summary>
                    public ItemRegistry Registry
                    {
                        get { return registry; }
                    }

                    /// <summary>
                    ///   <para>
                    ///     If a registry is specified, this is the key used to attach this
                    ///       item into that registry.
                    ///   </para>
                    /// </summary>
                    /// <remarks>
                    ///   You may disregard the existence of registries and not use them
                    ///     in your game. They are not needed, but useful for complex games.
                    /// </remarks>
                    [SerializeField]
                    private uint key;

                    /// <summary>
                    ///   See <see cref="key"/>.
                    /// </summary>
                    public uint Key
                    {
                        get { return key; }
                    }

                    /// <summary>
                    ///   The quantifying strategy to use. You will often use an instance of
                    ///     <see cref="QuantifyingStrategies.ItemUnstackedQuantifyingStrategy"/>
                    ///     or <see cref="QuantifyingStrategies.ItemIntegerQuantifyingStrategy"/>,
                    ///     or perhaps <see cref="QuantifyingStrategies.ItemFloatQuantifyingStrategy"/>
                    ///     but this is a rare case. You will rarely need to create your own subclass,
                    ///     but you may.
                    /// </summary>
                    [SerializeField]
                    private QuantifyingStrategies.ItemQuantifyingStrategy quantifyingStrategy;

                    /// <summary>
                    ///   See <see cref="QuantifyingStrategy"/>.
                    /// </summary>
                    public QuantifyingStrategies.ItemQuantifyingStrategy QuantifyingStrategy
                    {
                        get { return quantifyingStrategy; }
                    }

                    /// <summary>
                    ///   <para>
                    ///     The spatial strategies used for this item. <see cref="SpatialStrategies.ItemSpatialStrategy"/>
                    ///       for more details.
                    ///   </para>
                    ///   <para>
                    ///     More than one spatial strategy can be specified. Different inventories will
                    ///       have different spatial strategies (e.g. a matrix spatial strategy for bags
                    ///       may be used for inventories, while the floor will require simple spatial
                    ///       strategy).
                    ///   </para>
                    /// </summary>
                    /// <remarks>You cannot add two or more strategies of the same type!</remarks>
                    [SerializeField]
                    private SpatialStrategies.ItemSpatialStrategy[] spatialStrategies;
                    private Dictionary<Type, SpatialStrategies.ItemSpatialStrategy> spatialStrategiesByType;

                    /// <summary>
                    ///   <para>
                    ///     Usage strategies involve logic in the game. When you try to use your item, the
                    ///       inventory will look for these strategies and try to execute their respective
                    ///       logic.
                    ///   </para>
                    ///   <para>
                    ///     These strategies will depend among themselves, and one of them will be marked
                    ///       as main by setting it as value in <see cref="mainUsageStrategy"/>.
                    ///   </para>
                    /// </summary>
                    /// <remarks>you cannot add two or more strategies of the same type!</remarks>
                    [SerializeField]
                    private UsageStrategies.ItemUsageStrategy[] usageStrategies;
                    private UsageStrategies.ItemUsageStrategy[] sortedUsageStrategies;
                    private Dictionary<Type, UsageStrategies.ItemUsageStrategy> usageStrategiesByType;

                    /// <summary>
                    ///   Among the given <see cref="usageStrategies"/>, this one is marked as the main. Other
                    ///     strategies will count as dependencies for this strategy and among themselves.
                    /// </summary>
                    [SerializeField]
                    private UsageStrategies.ItemUsageStrategy mainUsageStrategy;

                    /// <summary>
                    ///   See <see cref="mainUsageStrategy"/>.
                    /// </summary>
                    public UsageStrategies.ItemUsageStrategy MainUsageStrategy
                    {
                        get
                        {
                            return mainUsageStrategy;
                        }
                    }

                    /// <summary>
                    ///   <para>
                    ///     Rendering strategies are the ones responsible of providing data to the rendering
                    ///       strategy on the inventory strategy.
                    ///   </para>
                    ///   <para>
                    ///     These strategies will depend among themselves, and one of them will be marked
                    ///       as main by setting it as value in <see cref="mainRenderingStrategy"/>.
                    ///   </para>
                    /// </summary>
                    /// <remarks>you cannot add two or more strategies of the same type!</remarks>
                    [SerializeField]
                    private RenderingStrategies.ItemRenderingStrategy[] renderingStrategies;
                    private RenderingStrategies.ItemRenderingStrategy[] sortedRenderingStrategies;
                    private Dictionary<Type, RenderingStrategies.ItemRenderingStrategy> renderingStrategiesByType;

                    /// <summary>
                    ///   Among the given <see cref="renderingStrategies"/>, this one is marked as the main. Other
                    ///     strategies will count as dependencies for this strategy and among themselves.
                    /// </summary>
                    [SerializeField]
                    private RenderingStrategies.ItemRenderingStrategy mainRenderingStrategy;

                    /// <summary>
                    ///   See <see cref="mainRenderingStrategy"/>.
                    /// </summary>
                    public RenderingStrategies.ItemRenderingStrategy MainRenderingStrategy
                    {
                        get
                        {
                            return mainRenderingStrategy;
                        }
                    }

                    /**
                     * Appropriately initializes the item strategies and dependencies.
                     */
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
                            AssetsLayout.CrossCheckDependencies<UsageStrategies.ItemUsageStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(usageStrategies, quantifyingStrategy);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, QuantifyingStrategies.ItemQuantifyingStrategy, RequireQuantifyingStrategy>(sortedRenderingStrategies, quantifyingStrategy);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, UsageStrategies.ItemUsageStrategy, RequireUsageStrategy>(sortedRenderingStrategies, usageStrategies);
                            AssetsLayout.CrossCheckDependencies<RenderingStrategies.ItemRenderingStrategy, SpatialStrategies.ItemSpatialStrategy, RequireSpatialStrategy>(sortedRenderingStrategies, spatialStrategies);
                            // Check both main strategies
                            AssetsLayout.CheckMainComponent(usageStrategies, mainUsageStrategy);
                            AssetsLayout.CheckMainComponent(renderingStrategies, mainRenderingStrategy);
                        }
                        catch (Exception exc)
                        {
                            Debug.Log(string.Format("Item::OnEnable() threw: {0}", exc));
                        }
                    }

                    /// <summary>
                    ///   Gets a spatial strategy for a given type.
                    /// </summary>
                    /// <typeparam name="T">The strategy type to choose</typeparam>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public T GetSpatialStrategy<T>() where T : SpatialStrategies.ItemSpatialStrategy
                    {
                        return GetSpatialStrategy(typeof(T)) as T;
                    }

                    /// <summary>
                    ///   Gets a spatial strategy for a given type.
                    /// </summary>
                    /// <param name="type">The strategy type to choose</param>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public SpatialStrategies.ItemSpatialStrategy GetSpatialStrategy(Type type)
                    {
                        SpatialStrategies.ItemSpatialStrategy spatialStrategy;
                        spatialStrategiesByType.TryGetValue(type, out spatialStrategy);
                        return spatialStrategy;
                    }

                    /// <summary>
                    ///   Gets a usage strategy for a given type.
                    /// </summary>
                    /// <typeparam name="T">The strategy type to choose</typeparam>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public T GetUsageStrategy<T>() where T : UsageStrategies.ItemUsageStrategy
                    {
                        return GetUsageStrategy(typeof(T)) as T;
                    }

                    /// <summary>
                    ///   Gets a usage strategy for a given type.
                    /// </summary>
                    /// <param name="type">The strategy type to choose</param>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public UsageStrategies.ItemUsageStrategy GetUsageStrategy(Type type)
                    {
                        UsageStrategies.ItemUsageStrategy usageStrategy;
                        usageStrategiesByType.TryGetValue(type, out usageStrategy);
                        return usageStrategy;
                    }

                    /// <summary>
                    ///   Gets a rendering strategy for a given type.
                    /// </summary>
                    /// <typeparam name="T">The strategy type to choose</typeparam>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public T GetRenderingStrategy<T>() where T : RenderingStrategies.ItemRenderingStrategy
                    {
                        return GetRenderingStrategy(typeof(T)) as T;
                    }

                    /// <summary>
                    ///   Gets a rendering strategy for a given type.
                    /// </summary>
                    /// <param name="type">The strategy type to choose</param>
                    /// <returns>An attached item strategy instance for the given type, or null</returns>
                    public RenderingStrategies.ItemRenderingStrategy GetRenderingStrategy(Type type)
                    {
                        RenderingStrategies.ItemRenderingStrategy renderingStrategy;
                        renderingStrategiesByType.TryGetValue(type, out renderingStrategy);
                        return renderingStrategy;
                    }

                    /// <summary>
                    ///   <para>
                    ///     Creates a stack. See <see cref="Stack"/> for more details.
                    ///   </para>
                    ///   <para>
                    ///     The stack will be created using strategies that will be derived
                    ///       from this item's strategies, and a certain quantity.
                    ///   </para>
                    /// </summary>
                    /// <param name="quantity">The quantity to give to the new stack</param>
                    /// <param name="argument">
                    ///   Usage strategies may read data from this object to initialize themselves
                    /// </param>
                    /// <returns>
                    ///   A new stack, not bound to any inventory but referencing this item
                    ///     and the appropriate strategies
                    /// </returns>
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

#if UNITY_EDITOR
                [CustomEditor(typeof(Item))]
                [CanEditMultipleObjects]
                public class ItemEditor : Editor
                {
                    SerializedProperty quantifyingStrategy;
                    SerializedProperty spatialStrategies;
                    SerializedProperty usageStrategies;
                    SerializedProperty renderingStrategies;
                    SerializedProperty mainUsageStrategy;
                    SerializedProperty mainRenderingStrategy;

                    protected virtual void OnEnable()
                    {
                        quantifyingStrategy = serializedObject.FindProperty("quantifyingStrategy");
                        spatialStrategies = serializedObject.FindProperty("spatialStrategies");
                        usageStrategies = serializedObject.FindProperty("usageStrategies");
                        renderingStrategies = serializedObject.FindProperty("renderingStrategies");
                        mainUsageStrategy = serializedObject.FindProperty("mainUsageStrategy");
                        mainRenderingStrategy = serializedObject.FindProperty("mainRenderingStrategy");
                    }

                    private Object RelatedPopup(string caption, SerializedProperty arrayProperty, Object selectedElement)
                    {
                        List<Object> fetchedElements = new List<Object>();
                        List<GUIContent> fetchedElementTypeNames = new List<GUIContent>();

                        for (int i = 0; i < arrayProperty.arraySize; i++)
                        {
                            Object element = arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                            if (element != null)
                            {
                                fetchedElements.Add(element);
                                fetchedElementTypeNames.Add(new GUIContent(string.Format("{0} ({1})", element.name, element.GetType().Name)));
                            }
                        }

                        int index = fetchedElements.IndexOf(selectedElement);
                        index = EditorGUILayout.Popup(new GUIContent(caption), index, fetchedElementTypeNames.ToArray());
                        return index >= 0 ? fetchedElements[index] : null;
                    }

                    public override void OnInspectorGUI()
                    {
                        serializedObject.Update();

                        EditorGUILayout.PropertyField(quantifyingStrategy);
                        EditorGUILayout.PropertyField(spatialStrategies, true);
                        EditorGUILayout.PropertyField(usageStrategies, true);
                        mainUsageStrategy.objectReferenceValue = RelatedPopup("Main Usage Strategy", usageStrategies, mainUsageStrategy.objectReferenceValue);
                        EditorGUILayout.PropertyField(renderingStrategies, true);
                        mainRenderingStrategy.objectReferenceValue = RelatedPopup("Main Rendering Strategy", renderingStrategies, mainRenderingStrategy.objectReferenceValue);

                        serializedObject.ApplyModifiedProperties();
                    }
                }
#endif
            }
        }
    }
}
