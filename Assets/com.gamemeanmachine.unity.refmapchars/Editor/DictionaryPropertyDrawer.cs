using AlephVault.Unity.Support.Generic.Authoring.Types;
using UnityEditor;


namespace GameMeanMachine.Unity.RefMapChars
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Standard
            {
                [CustomPropertyDrawer(typeof(RefMapBody.RefMapVariationsDictionary))]
                [CustomPropertyDrawer(typeof(RefMapItem.RefMapVariationsDictionary))]
                [CustomPropertyDrawer(typeof(RefMapItemType.RefMapItemsDictionary))]
                [CustomPropertyDrawer(typeof(RefMapSex.RefMapItemTypesDictionary))]
                [CustomPropertyDrawer(typeof(RefMapBundle.RefMapSexDictionary))]
                public class RefMapDictionaryPropertyDrawer : DictionaryPropertyDrawer {}
            }
        }
    }
}

