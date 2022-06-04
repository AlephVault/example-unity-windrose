using System;
using System.Collections.Generic;
using System.Linq;
using GameMeanMachine.Unity.RefMapChars.Types;
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
                ///   A list of the available sources, given an index.
                ///   These are intended to categorize a single object.
                ///   Only 10 variations are allowed. An item belongs
                ///   to a certain sex trait, and a certain asset type.
                ///   An item may, however, be actually a part of a
                ///   bigger trait, whose parts are scattered (e.g.
                ///   a front hair and a back hair would be implemented
                ///   in two different items, but working as a complement
                ///   of each other respectively).
                /// </summary>
                public class RefMapItem : ScriptableObject
                {
                    /// <summary>
                    ///   Each item or body trait (other than the body itself)
                    ///   has 10 possible colors.
                    /// </summary>
                    public enum ColorCode : byte
                    {
                        Black,
                        Blue,
                        DarkBrown,
                        Green,
                        LightBrown,
                        Pink,
                        Purple,
                        Red,
                        White,
                        Yellow
                    }

                    /// <summary>
                    ///   The dictionary to use (maps a byte code to a ref
                    ///   map source).
                    /// </summary>
                    [Serializable]
                    public class RefMapVariationsDictionary : Dictionary<ColorCode, RefMapSource> {}
                
                    /// <summary>
                    ///   A dictionary of the variations to use for this
                    ///   graphical asset.
                    /// </summary>
                    [SerializeField]
                    private RefMapVariationsDictionary variations = new RefMapVariationsDictionary();

                    /// <summary>
                    ///   Gets a <see cref="RefMapSource"/> at a given index.
                    /// </summary>
                    /// <param name="colorCode">The index to retrieve the source for</param>
                    public RefMapSource this[ColorCode colorCode] => variations[colorCode];
                    
                    /// <summary>
                    ///   The count of variations in an item.
                    /// </summary>
                    public int Count => variations.Count;

                    /// <summary>
                    ///   Gets the available variations of an item.
                    /// </summary>
                    /// <returns>An enumerable of pairs color/variation</returns>
                    public IEnumerable<KeyValuePair<ColorCode, RefMapSource>> Items()
                    {
                        return from variation in variations
                            where variation.Value != null
                            select variation;
                    }
                }
            }
        }
    }    
}
