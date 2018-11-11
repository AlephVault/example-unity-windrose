using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                [CreateAssetMenu(fileName = "NewBundledTile", menuName = "Wind Rose/Items/Pickable Item", order = 201)]
                public class Item : ScriptableObject
                {
                    /**
                     * A pickable item has a certain bunch of item strategies
                     *   attached, quite like the bundled tile. Those strategies
                     *   are just bundled data and are used by other items. Quite
                     *   like with tiles, drop/pick behaviours will react by
                     *   default if no matching data strategies are present, and
                     *   that depends only on each behaviour.
                     *   
                     * There are two types of data bundles (strategies):
                     * - Ruling/Stacking strategies, which interact with the rules
                     *   of the item being contained.
                     * - Display strategies, which are only considered to fill a
                     *   dictionary of data needed to represent.
                     */

                    // Input stacking strategies are here
                    [SerializeField]
                    private StackStrategies.ItemStackStrategy[] stackStrategies;

                    // Sorted stacking strategies are here
                    private StackStrategies.ItemStackStrategy[] sortedStackStrategies;

                    // Input display strategies are here
                    [SerializeField]
                    private DisplayStrategies.ItemDisplayStrategy[] displayStrategies;

                    // Sorted display strategies are here
                    private DisplayStrategies.ItemDisplayStrategy[] sortedDisplayStrategies;

                    // Display data will be stored here
                    private DisplayStrategies.ItemDisplayStrategy.DisplayData displayData = new DisplayStrategies.ItemDisplayStrategy.DisplayData();

                    // Display data will be accessed here
                    public IEnumerable<KeyValuePair<string, object>> GetDisplayData()
                    {
                        return displayData.AsEnumerable();
                    }

                    // Display strategies may require stacking strategies
                    //   so we will also check dependencies in that way
                    private void EnsureAllDependenciesFromStackToDisplayStrategies()
                    {
                        HashSet<Type> requiredDependencies = new HashSet<Type>();
                        foreach(DisplayStrategies.ItemDisplayStrategy displayStrategy in displayStrategies)
                        {
                            foreach (RequireStackStrategy attribute in displayStrategy.GetType().GetCustomAttributes(typeof(RequireStackStrategy), true))
                            {
                                requiredDependencies.Add(attribute.Dependency);
                            }
                        }
                        HashSet<Type> installed = new HashSet<Type>(from stackStrategy in stackStrategies select stackStrategy.GetType());
                        HashSet<Type> unsatisfiedDependencies = new HashSet<Type>(requiredDependencies.Except(installed));
                        if (unsatisfiedDependencies.Count > 0)
                        {
                            if (unsatisfiedDependencies.Count == 1)
                            {
                                throw new AssetsLayout.DependencyException("Unsatisfied dependency: " + unsatisfiedDependencies.First().FullName);
                            }
                            else
                            {
                                throw new AssetsLayout.DependencyException("Unsatisfied dependencies: " + string.Join(",", (from unsatisfiedDependency in unsatisfiedDependencies select unsatisfiedDependency.FullName).ToArray()));
                            }
                        }
                    }

                    private void Awake()
                    {
                        try
                        {
                            // Flatten (and check!) dependencies of both types of strategies
                            sortedStackStrategies = AssetsLayout.FlattenDependencies<StackStrategies.ItemStackStrategy, RequireStackStrategy>(stackStrategies, true);
                            sortedDisplayStrategies = AssetsLayout.FlattenDependencies<DisplayStrategies.ItemDisplayStrategy, RequireDisplayStrategy>(displayStrategies, true);
                            // Raise an exception if a needed stack strategy dependency, needed by any strategy among display strategies, is not present
                            EnsureAllDependenciesFromStackToDisplayStrategies();
                            foreach(DisplayStrategies.ItemDisplayStrategy displayStrategy in displayStrategies)
                            {
                                displayStrategy.PopulateDisplayData(displayData, this);
                            }
                        }
                        catch(Exception)
                        {
                            Resources.UnloadAsset(this);
                        }
                    }

                    /**
                     * Tools to get a component strategy, as we have in BundledTiles. This will be specially useful for
                     *   display strategies when they may require data from stack strategies
                     */

                    public T GetStackStrategy<T>() where T : StackStrategies.ItemStackStrategy
                    {
                        return (from strategy in stackStrategies where strategy is T select (T)strategy).FirstOrDefault();
                    }

                    public T[] GetStackStrategies<T>() where T : StackStrategies.ItemStackStrategy
                    {
                        return (from strategy in stackStrategies where strategy is T select (T)strategy).ToArray();
                    }

                    public T GetDisplayStrategy<T>() where T : DisplayStrategies.ItemDisplayStrategy
                    {
                        return (from displayStrategy in displayStrategies where displayStrategy is T select (T)displayStrategy).FirstOrDefault();
                    }

                    public T[] GetDsiplayStrategies<T>() where T : DisplayStrategies.ItemDisplayStrategy
                    {
                        return (from displayStrategy in displayStrategies where displayStrategy is T select (T)displayStrategy).ToArray();
                    }
                }
            }
        }
    }
}
