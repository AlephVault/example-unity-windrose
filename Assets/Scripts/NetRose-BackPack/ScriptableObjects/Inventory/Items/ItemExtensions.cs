using System;
using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                using BackPack.ScriptableObjects.Inventory.Items;

                public static class ItemExtensions
                {
                    /// <summary>
                    ///   This exception is triggered when an Item is not registered and tries to
                    ///     serialize itself, oran unexisting item is trying to be deserialized.
                    /// </summary>
                    public class UnregisteredItemException : Exception
                    {
                        public UnregisteredItemException() { }
                        public UnregisteredItemException(string message) : base(message) { }
                        public UnregisteredItemException(string message, Exception inner) : base(message, inner) { }
                    }

                    /// <summary>
                    ///   Writes the item's registry key and ID key into the stream.
                    ///     It fails if the item is not in any registry.
                    /// </summary>
                    /// <param name="writer">The writer to write the data into</param>
                    /// <param name="item">The item to serialize</param>
                    public static void WriteItem(this NetworkWriter writer, Item item)
                    {
                        if (!item.Registered) throw new UnregisteredItemException("Cannot serialize an item that is not registered into a registry");
                        writer.WriteString(item.Registry.Key);
                        writer.WriteUInt32(item.Key);
                    }

                    /// <summary>
                    ///   Reads the item's registry key and ID key from the stream and
                    ///     tries to get a registered item by those data elements.
                    /// </summary>
                    /// <param name="reader">The reader to read the data from</param>
                    /// <returns>The item to deserialize</returns>
                    public static Item ReadItem(this NetworkReader reader)
                    {
                        string registryKey = reader.ReadString();
                        uint itemKey = reader.ReadUInt32();
                        Item item = ItemRegistry.GetItem(registryKey, itemKey);
                        if (item)
                        {
                            return item;
                        }
                        else
                        {
                            throw new UnregisteredItemException("Cannot get a valid item after deserialization because the data seems to not correspond to any valid item entry");
                        }
                    }
                }
            }
        }
    }
}
