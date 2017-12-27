using System.Collections;
using UnityEditor;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            [CustomPropertyDrawer(typeof(Interactors.InteractorsManager.InteractorsDictionary))]
            public class InteractorsDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
        }
    }
}