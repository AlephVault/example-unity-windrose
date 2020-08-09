using UnityEditor;

namespace GMM
{
    namespace Types
    {
        [CustomPropertyDrawer(typeof(SceneReferenceDictionary))]
        public class SceneReferenceDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
    }
}
