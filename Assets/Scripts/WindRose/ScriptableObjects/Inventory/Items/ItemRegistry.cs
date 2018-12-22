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
                [CreateAssetMenu(fileName = "NewInventoryItemRegistry", menuName = "Wind Rose/Inventory/Item Registry", order = 202)]
                public class ItemRegistry : ScriptableObject
                {
                    /**
                     * An inventory item registry. It is optional but useful for online / saveable games.
                     * 
                     * You should have many different registries to correctly organize your inventory assets.
                     * You will be able to obtain the appropriate registry and the appropriate item if you
                     *   know the keys.
                     */

                    private static Dictionary<string, ItemRegistry> registries = new Dictionary<string, ItemRegistry>();

                    public static ItemRegistry GetRegistry(string key)
                    {
                        ItemRegistry registry;
                        registries.TryGetValue(key, out registry);
                        return registry;
                    }

                    public static IEnumerable<KeyValuePair<string, ItemRegistry>> Registries()
                    {
                        return registries.AsEnumerable();
                    }

                    private Dictionary<uint, Item> items = new Dictionary<uint, Item>();

                    public Item GetItem(uint key)
                    {
                        Item item;
                        items.TryGetValue(key, out item);
                        return item;
                    }

                    public IEnumerable<KeyValuePair<uint, Item>> Items()
                    {
                        return items.AsEnumerable();
                    }

                    public bool Contains(uint key)
                    {
                        return items.ContainsKey(key);
                    }

                    public bool AddItem(Item item)
                    {
                        if (!Contains(item.Key))
                        {
                            items[item.Key] = item;
                            return true;
                        }
                        return false;
                    }

                    public static Item GetItem(string registryKey, uint itemKey)
                    {
                        ItemRegistry registry = GetRegistry(registryKey);
                        return registry != null ? registry.GetItem(itemKey) : null;
                    }

                    [SerializeField]
                    private string key;

                    public string Key
                    {
                        get { return key; }
                    }

                    private void Awake()
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
