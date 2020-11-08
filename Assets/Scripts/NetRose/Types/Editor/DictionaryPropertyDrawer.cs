using GMM.Types;
using UnityEditor;

namespace NetRose
{
    namespace Types
    {
        [CustomPropertyDrawer(typeof(SceneConfigDictionary))]
        public class SceneConfigDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    }
}