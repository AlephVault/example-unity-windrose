using UnityEditor;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using AlephVault.Unity.Scenes.Authoring.Types;

namespace NetRose
{
    namespace Types
    {
        [CustomPropertyDrawer(typeof(SceneConfigDictionary))]
        public class SceneConfigDictionaryPropertyDrawer : DictionaryPropertyDrawer { }
    }
}