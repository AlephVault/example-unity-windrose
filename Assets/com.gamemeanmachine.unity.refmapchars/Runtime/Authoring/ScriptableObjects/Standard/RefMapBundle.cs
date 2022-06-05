using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;


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
                ///   This is a full bundle and has two sexes which
                ///   contain all the relevant data.
                /// </summary>
                public class RefMapBundle : ScriptableObject
                {
                    /// <summary>
                    ///   The available sexs.
                    /// </summary>
                    public enum SexCode
                    {
                        Male,
                        Female
                    }
                    
                    /// <summary>
                    ///   The dictionary to use (maps a sex code to a <see cref="RefMapSex"/>).
                    /// </summary>
                    [Serializable]
                    public class RefMapSexDictionary : Dictionary<SexCode, RefMapSex> {}

                    /// <summary>
                    ///   A dictionary of the sex types to use in this main bundle.
                    /// </summary>
                    [SerializeField]
                    private RefMapSexDictionary sexes = new RefMapSexDictionary();
                    
                    /// <summary>
                    ///   Gets a <see cref="SexData"/> at a given sex code.
                    /// </summary>
                    /// <param name="sexCode">The code to retrieve the data for</param>
                    public RefMapSex this[SexCode sexCode] => sexes[sexCode];

                    /// <summary>
                    ///   The count of sexes in this main bundle.
                    /// </summary>
                    public int Count => sexes.Count;
                    
                    /// <summary>
                    ///   Gets the available sex data elements in this main bundle.
                    /// </summary>
                    /// <returns>An enumerable of pairs item type sex code/sex data</returns>
                    public IEnumerable<KeyValuePair<SexCode, RefMapSex>> Items()
                    {
                        return from sex in sexes 
                               where sex.Value != null
                               select sex;
                    }
                    
#if UNITY_EDITOR
                    [MenuItem("Assets/Create/RefMap Chars/Full RefMap Bundle", true, 101)]
                    private static bool CanCreateFullRefMapBundle()
                    {
                        Object obj = Selection.activeObject;
                        if (obj is null)
                        {
                            return false;
                        }

                        string path = AssetDatabase.GetAssetPath(obj);
                        // The object is a directory if (and only if) a
                        // directory exists by this path. Otherwise, the
                        // object is something else.
                        return AssetDatabase.IsValidFolder(path);
                    }

                    [MenuItem("Assets/Create/RefMap Chars/Full RefMap Bundle", false, 101)]
                    private static void CreateFullRefMapBundle()
                    {
                        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                        string parent = Path.GetDirectoryName(path);
                        string refmap = Path.Combine(parent, "RefMap");
                        // Preliminary: {parent}/RefMap must NOT exist. Otherwise, this
                        // is an error to be logged and everything to be aborted.
                        if (AssetDatabase.IsValidFolder(refmap))
                        {
                            Debug.LogError($"Directory {refmap} already exists. Please delete it or " +
                                           $"move it to another location and try again");
                            return;
                        }
                        
                        // First: Create the bundle and populate it.
                        RefMapBundle bundle = CreateInstance<RefMapBundle>();
                        Populate(path, bundle);
                        AssetDatabase.CreateFolder(parent, "RefMap");
                        
                        // Then: Save EACH element appropriately under the refmap
                        // directory.
                        foreach (KeyValuePair<SexCode, RefMapSex> sexPair in bundle.sexes)
                        {
                            // Save a RefMapSex : Save many RefMapItemType + Save body.
                            
                            SexCode sexCode = sexPair.Key;
                            RefMapSex sex = sexPair.Value;
                            foreach (KeyValuePair<RefMapSex.ItemTypeCode, RefMapItemType> itemTypePair in sex.Items())
                            {
                                // Save a RefMapItemType : Save many RefMapItem.
                                
                                RefMapSex.ItemTypeCode typeCode = itemTypePair.Key;
                                RefMapItemType type_ = itemTypePair.Value;
                                foreach (KeyValuePair<ushort, RefMapItem> itemPair in type_.Items())
                                {
                                    // Save a RefMapItem.

                                    ushort itemIdx = itemPair.Key;
                                    RefMapItem item = itemPair.Value;

                                    string itemPath = Path.Combine(refmap, $"{sexCode}_{typeCode}_{itemIdx}.asset");
                                    AssetDatabase.CreateAsset(item, itemPath);
                                }

                                string typePath = Path.Combine(refmap, $"{sexCode}_{typeCode}.asset");
                                AssetDatabase.CreateAsset(type_, typePath);
                            }

                            string bodyPath = Path.Combine(refmap, $"{sexCode}_Body.asset");
                            AssetDatabase.CreateAsset(sex.Body, bodyPath);

                            string sexPath = Path.Combine(refmap, $"{sexCode}.asset");
                            AssetDatabase.CreateAsset(sex, sexPath);
                        }
                        
                        string bundlePath = Path.Combine(refmap, "Bundle.asset");
                        AssetDatabase.CreateAsset(bundle, bundlePath);
                    }

                    /// <summary>
                    ///   Populates a main bundle from a given path. This
                    ///   also involves creating each sex instance, and
                    ///   populating them on their own.
                    /// </summary>
                    /// <param name="path">The path to read from</param>
                    /// <param name="main">The main bundle to read into</param>
                    internal static void Populate(string path, RefMapBundle main)
                    {
                        RefMapSex male = CreateInstance<RefMapSex>();
                        RefMapSex female = CreateInstance<RefMapSex>();
                        RefMapSex.Populate(Path.Combine(path, "Male"), male);
                        RefMapSex.Populate(Path.Combine(path, "Female"), female);
                        main.sexes.Add(SexCode.Male, male);
                        main.sexes.Add(SexCode.Female, female);
                    }
#endif
                }
            }
        }
    }    
}
