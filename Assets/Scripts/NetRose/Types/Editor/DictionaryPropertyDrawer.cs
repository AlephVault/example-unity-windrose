using UnityEditor;
using AlephVault.Unity.Support.Generic.Authoring.Types;

namespace NetRose
{
    namespace Types
    {
        [CustomPropertyDrawer(typeof(SceneConfigDictionary))]
        public class SceneConfigDictionaryPropertyDrawer : DictionaryPropertyDrawer { }
    }
}