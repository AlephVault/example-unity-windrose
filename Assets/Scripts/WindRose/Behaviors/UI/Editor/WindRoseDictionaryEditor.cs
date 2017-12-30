using UnityEditor;
using Support.Editors;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            [CustomPropertyDrawer(typeof(Interactors.InteractorsManager.InteractorsDictionary))]
            [CustomPropertyDrawer(typeof(Interactors.ButtonsInteractor.ButtonKeyDictionary))]
            public class WindRoseDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
        }
    }
}