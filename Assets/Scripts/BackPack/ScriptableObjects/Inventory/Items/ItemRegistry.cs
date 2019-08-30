using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BackPack
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                /// <summary>
                ///   <para>An inventory item registry. It is optional but useful for online / saveable games.</para>
                ///   <para>
                ///     You should have many different registries to correctly organize your inventory assets.
                ///     You will be able to obtain the appropriate registry and the appropriate item if you
                ///       know the keys.
                ///   </para>
                /// </summary>
                [CreateAssetMenu(fileName = "NewInventoryItemRegistry", menuName = "Wind Rose/Inventory/Item Registry", order = 202)]
                public class ItemRegistry : ScriptableObject
                {
                    private static Dictionary<string, ItemRegistry> registries = new Dictionary<string, ItemRegistry>();

                    /// <summary>
                    ///   Gets a registry by a given registry key, or null.
                    /// </summary>
                    /// <param name="key">The key to search</param>
                    /// <returns>The registry by that key, or null</returns>
                    public static ItemRegistry GetRegistry(string key)
                    {
                        ItemRegistry registry;
                        registries.TryGetValue(key, out registry);
                        return registry;
                    }

                    /// <summary>
                    ///   Enumerates all the registries (and their respective per-registry key).
                    /// </summary>
                    /// <returns>An enumerable of pairs of keys and registries</returns>
                    public static IEnumerable<KeyValuePair<string, ItemRegistry>> Registries()
                    {
                        return registries.AsEnumerable();
                    }

                    private Dictionary<uint, Item> items = new Dictionary<uint, Item>();

                    /// <summary>
                    ///   Gets an item from the registry, by its key.
                    /// </summary>
                    /// <param name="key">The item key to search</param>
                    /// <returns>The registered item by that key, or null</returns>
                    public Item GetItem(uint key)
                    {
                        Item item;
                        items.TryGetValue(key, out item);
                        return item;
                    }

                    /// <summary>
                    ///   Enumerates all the items in the registry (and their respective per-item key).
                    /// </summary>
                    /// <returns>An enumerable of pairs of keys and items</returns>
                    public IEnumerable<KeyValuePair<uint, Item>> Items()
                    {
                        return items.AsEnumerable();
                    }

                    /// <summary>
                    ///   Tells whether the given key is registered in this registry.
                    /// </summary>
                    /// <param name="key">The item key to search</param>
                    /// <returns>A boolean value telling whether the key is in use</returns>
                    public bool Contains(uint key)
                    {
                        return items.ContainsKey(key);
                    }

                    /// <summary>
                    ///   Tries to add an item to this registry under its key. It will
                    ///     fail to add it if its key is use by another item.
                    /// </summary>
                    /// <param name="item">The item to add</param>
                    /// <returns>Whether the item could be added or not</returns>
                    public bool AddItem(Item item)
                    {
                        if (!Contains(item.Key))
                        {
                            items[item.Key] = item;
                            return true;
                        }
                        return false;
                    }

                    /// <summary>
                    ///   Tries to get an item from a registry by specifying both
                    ///     the item key and the registry key.
                    /// </summary>
                    /// <param name="registryKey">The registry key to search</param>
                    /// <param name="itemKey">The item key to search</param>
                    /// <returns>A registered item by both keys, or null if either the registry or the item do not exist</returns>
                    public static Item GetItem(string registryKey, uint itemKey)
                    {
                        ItemRegistry registry = GetRegistry(registryKey);
                        return registry != null ? registry.GetItem(itemKey) : null;
                    }

                    /// <summary>
                    ///   The registry key
                    /// </summary>
                    [SerializeField]
                    private string key;

                    public string Key
                    {
                        get { return key; }
                    }

                    /// <summary>
                    ///   Invoked externally by an <see cref="Item"/> referencing it,
                    ///     this registry initializes and tries to register in the
                    ///     global registry.
                    /// </summary>
                    public void Init()
                    {
                        if (key != "" && !registries.ContainsKey(key))
                        {
                            registries[key] = this;
                        }
                    }
                }
            }
        }
    }
}
