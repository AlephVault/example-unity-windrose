using UnityEditor;
using GMM.Types;

namespace NetRose
{
    namespace Worlds
    {
        [CustomPropertyDrawer(typeof(SceneConfigDictionary))]
        public class SceneConfigDictionaryDrawer : SerializableDictionaryPropertyDrawer { }
    }
}
