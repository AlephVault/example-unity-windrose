using UnityEditor;
using GMM.Types;

namespace GabTab
{
    namespace Behaviours
    {
        [CustomPropertyDrawer(typeof(Interactors.InteractorsManager.InteractorsDictionary))]
        [CustomPropertyDrawer(typeof(Interactors.ButtonsInteractor.ButtonKeyDictionary))]
        public class DictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
    }
}