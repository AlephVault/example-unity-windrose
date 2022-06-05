using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameMeanMachine.Unity.RefMapChars.Types;
using UnityEditor;
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
                    
#if UNITY_EDITOR
                    /// <summary>
                    ///   Populates a body from a given path. This path is typically
                    ///   {path}/(Male|Female)/{ItemType}.
                    /// </summary>
                    /// <param name="path">The path to read from</param>
                    /// <param name="item">The body to read into</param>
                    /// <param name="back">Whether this asset is populated from _b images</param>
                    public static void Populate(string path, int idx, RefMapItem item, bool back = false)
                    {
                        string suffix = back ? "_b" : "";
                        foreach (ColorCode code in Enum.GetValues(typeof(ColorCode)))
                        {
                            string file = Path.Combine(path, $"{idx}_{code.Name()}{suffix}.png");
                            try
                            {
                                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
                                if (tex == null) throw new Exception();
                                item.variations.Add(code, new RefMapSource { Texture = tex });
                            }
                            catch (Exception e)
                            {
                                Debug.Log($"Missing texture {idx}_{code}{suffix}.png - skipping");
                            }
                        }
                    }
#endif
                }
                
                /// <summary>
                ///   Methods for the <see cref="RefMapItem.ColorCode" /> class.
                /// </summary>
                public static class ItemColorCodeMethods
                {
                    /// <summary>
                    ///   Gives a code name for the color.
                    /// </summary>
                    /// <param name="code">The color code</param>
                    /// <returns>The in-file code name</returns>
                    /// <exception cref="ArgumentException">An invalid or unexpected color was provided</exception>
                    public static string Name(this RefMapItem.ColorCode code)
                    {
                        switch (code)
                        {
                            case RefMapItem.ColorCode.Black:
                                return "black";
                            case RefMapItem.ColorCode.Blue:
                                return "blue";
                            case RefMapItem.ColorCode.DarkBrown:
                                return "dbrown";
                            case RefMapItem.ColorCode.Green:
                                return "green";
                            case RefMapItem.ColorCode.LightBrown:
                                return "lbrown";
                            case RefMapItem.ColorCode.Pink:
                                return "pink";
                            case RefMapItem.ColorCode.Purple:
                                return "purple";
                            case RefMapItem.ColorCode.Red:
                                return "red";
                            case RefMapItem.ColorCode.White:
                                return "white";
                            case RefMapItem.ColorCode.Yellow:
                                return "yellow";
                            default:
                                throw new ArgumentException($"Invalid item color code: {code}");
                        }
                    }
                }
            }
        }
    }    
}
