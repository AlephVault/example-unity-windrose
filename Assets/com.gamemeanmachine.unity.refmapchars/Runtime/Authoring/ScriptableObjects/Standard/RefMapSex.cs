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
                ///   A sex directory holds everything that is
                ///   suitable for the standard RefMap resources
                ///   (e.g. weapons are not included). Inside the
                ///   sex traits, there are sub-directories but
                ///   also inside them there are the actual items.
                ///   One particular group is the body trait, which
                ///   is kept aside.
                /// </summary>
                public class RefMapSex : ScriptableObject
                {
                    /// <summary>
                    ///   The available item types. In this coding,
                    ///   the hair is broken into two different
                    ///   parts: front, and tail.
                    /// </summary>
                    public enum ItemTypeCode
                    {
                        Boots,
                        Pants,
                        Shirt,
                        Chest,
                        LongShirt,
                        Arms,
                        Waist,
                        Shoulder,
                        Hair,
                        HairTail
                    }
                
                    /// <summary>
                    ///   The dictionary to use (maps a category code to a ref
                    ///   map item type).
                    /// </summary>
                    [Serializable]
                    public class RefMapItemTypesDictionary : Dictionary<ItemTypeCode, RefMapItemType> {}

                    /// <summary>
                    ///   The available bodies.
                    /// </summary>
                    [SerializeField]
                    private RefMapBody body;
                    
                    /// <summary>
                    ///   A dictionary of the item categories to use in this sex.
                    /// </summary>
                    [SerializeField]
                    private RefMapItemTypesDictionary itemTypes = new RefMapItemTypesDictionary();

                    /// <summary>
                    ///   Gets a <see cref="RefMapItemType"/> at a given item type.
                    /// </summary>
                    /// <param name="index">The item type to retrieve the items for</param>
                    public RefMapItemType this[ItemTypeCode itemTypeCode] => itemTypes[itemTypeCode];
                    
                    /// <summary>
                    ///   The count of item types in a sex data.
                    /// </summary>
                    public int Count => itemTypes.Count;

                    /// <summary>
                    ///   Gets the available item types of the sex data.
                    /// </summary>
                    /// <returns>An enumerable of pairs item type code/item type</returns>
                    public IEnumerable<KeyValuePair<ItemTypeCode, RefMapItemType>> Items()
                    {
                        return from itemType in itemTypes
                               where itemType.Value != null
                               select itemType;
                    }
                }
            }
        }
    }    
}
