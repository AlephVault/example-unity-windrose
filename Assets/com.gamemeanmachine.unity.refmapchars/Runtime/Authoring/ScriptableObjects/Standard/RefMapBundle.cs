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
                }
            }
        }
    }    
}
