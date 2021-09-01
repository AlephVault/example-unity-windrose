using AlephVault.Unity.Support.Generic.Authoring.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Types
        {
            [CustomPropertyDrawer(typeof(NetworkedScopePrefabDictionary))]
            public class PrefabDictionaryDrawer : DictionaryPropertyDrawer {}
        }
    }
}
