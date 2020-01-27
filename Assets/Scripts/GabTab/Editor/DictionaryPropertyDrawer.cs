﻿using UnityEditor;
using GMM.Editors;

namespace GabTab
{
    namespace Behaviours
    {
        [CustomPropertyDrawer(typeof(Interactors.InteractorsManager.InteractorsDictionary))]
        [CustomPropertyDrawer(typeof(Interactors.ButtonsInteractor.ButtonKeyDictionary))]
        public class DictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
    }
}