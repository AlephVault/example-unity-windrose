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
                    ///   The available bodies.
                    /// </summary>
                    public RefMapBody Body => body;

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
                    
#if UNITY_EDITOR
                    /// <summary>
                    ///   Populates a sex bundle from a given path. This path
                    ///   is typically {path}/(Male|Female). This also involves
                    ///   creating the body instance, and all the item type
                    ///   instances and linking them into the sex bundle.
                    /// </summary>
                    /// <param name="path">The path to read from</param>
                    /// <param name="sex">The sex bundle to read into</param>
                    internal static void Populate(string path, RefMapSex sex)
                    {
                        RefMapBody body = CreateInstance<RefMapBody>();
                        RefMapBody.Populate(Path.Combine(path, "Base"), body);
                        sex.body = body;
                        foreach (ItemTypeCode code in Enum.GetValues(typeof(ItemTypeCode)))
                        {
                            if (code == ItemTypeCode.Hair) continue;
                            RefMapItemType itemType = CreateInstance<RefMapItemType>();
                            RefMapItemType.Populate(Path.Combine(path, itemType.ToString()), itemType);
                            sex.itemTypes.Add(code, itemType);
                        }
                        RefMapItemType hairType = CreateInstance<RefMapItemType>();
                        RefMapItemType hairTailType = CreateInstance<RefMapItemType>();
                        RefMapItemType.Populate(Path.Combine(path, "Hair"), hairType, hairTailType);
                        sex.itemTypes.Add(ItemTypeCode.Hair, hairType);
                        sex.itemTypes.Add(ItemTypeCode.HairTail, hairTailType);
                    }
#endif
                }
            }
        }
    }    
}
