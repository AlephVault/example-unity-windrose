using UnityEditor;
using Support.Editors;

namespace GabTab
{
    namespace Behaviours
    {
        [CustomPropertyDrawer(typeof(Interactors.InteractorsManager.InteractorsDictionary))]
        [CustomPropertyDrawer(typeof(Interactors.ButtonsInteractor.ButtonKeyDictionary))]
        public class WindRoseDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
    }
}