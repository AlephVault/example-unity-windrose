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
                ///   A list of the available bodies, given an index.
                ///   These are intended to categorize body by color.
                ///   Only 8 variations are allowed. A body belongs
                ///   to a certain sex trait.
                /// </summary>
                public class RefMapBody : ScriptableObject
                {
                    /// <summary>
                    ///   The body has 8 possible colors.
                    /// </summary>
                    public enum ColorCode : byte
                    {
                        Black,
                        Blue,
                        Green,
                        Orange,
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
                    ///   The count of variations in the body.
                    /// </summary>
                    public int Count => variations.Count;

                    /// <summary>
                    ///   Gets the available variations of the body.
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
