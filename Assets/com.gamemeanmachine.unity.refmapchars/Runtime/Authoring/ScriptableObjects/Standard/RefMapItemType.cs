using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Standard
            {
                using AlephVault.Unity.Support.Generic.Authoring.Types;
                
                /// <summary>
                ///   A list of the available items, given a category
                ///   index. Only 65536 items are allowed. Categories
                ///   (item types) belong to a sex, and contain items
                ///   that are defined within.
                /// </summary>
                public class RefMapItemType : ScriptableObject
                {
                    /// <summary>
                    ///   The dictionary to use (maps a byte code to a ref
                    ///   map item).
                    /// </summary>
                    [Serializable]
                    public class RefMapItemsDictionary : Dictionary<ushort, RefMapItem> {}
                
                    /// <summary>
                    ///   A dictionary of the items to use in this category.
                    /// </summary>
                    [SerializeField]
                    private RefMapItemsDictionary items = new RefMapItemsDictionary();

                    /// <summary>
                    ///   Gets a <see cref="RefMapItem"/> at a given index.
                    /// </summary>
                    /// <param name="index">The index to retrieve the item for</param>
                    public RefMapItem this[ushort index] => items[index];
                    
                    /// <summary>
                    ///   The count of items in the type.
                    /// </summary>
                    public int Count => items.Count;

                    /// <summary>
                    ///   Get the available items in the type.
                    /// </summary>
                    /// <returns>An enumerable of pairs index/item</returns>
                    public IEnumerable<KeyValuePair<ushort, RefMapItem>> Items()
                    {
                        return from item in items
                               where item.Value != null
                               select item;
                    }
                    
#if UNITY_EDITOR
                    /// <summary>
                    ///   Populates the whole item type from a directory.
                    ///   This path is typically {path}/(Male|Female)/{ItemType}.
                    ///   In this case, populating involves also creating the
                    ///   instances of Items and adding them to the dictionary.
                    /// </summary>
                    /// <param name="path">The path to read from</param>
                    /// <param name="itemType">The item type to read into</param>
                    /// <param name="backType">The back type to read into, if a back image is present</param>
                    public static void Populate(string path, RefMapItemType itemType, RefMapItemType backType = null)
                    {
                        ushort idx = 1;
                        while (true)
                        {
                            // First, look for textures of name {idx}_{color}.png.
                            string[] files = Directory.GetFiles(path, $"{idx}_*.png");
                            if (files.Length != 0)
                            {
                                RefMapItem item = CreateInstance<RefMapItem>();
                                RefMapItem.Populate(path, idx, item);
                                itemType.items.Add(idx, item);
                                if (backType)
                                {
                                    string[] backFiles = Directory.GetFiles(path, $"{idx}_*_b.png");
                                    if (backFiles.Length != 0)
                                    {
                                        RefMapItem backItem = CreateInstance<RefMapItem>();
                                        RefMapItem.Populate(path, idx, backItem, true);
                                        backType.items.Add(idx, backItem);
                                    }
                                }
                                idx += 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
#endif
                }
            }
        }
    }    
}
