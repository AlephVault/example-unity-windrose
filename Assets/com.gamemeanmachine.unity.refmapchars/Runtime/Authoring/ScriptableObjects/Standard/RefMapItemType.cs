using System;
using System.Collections.Generic;
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
                }
            }
        }
    }    
}
