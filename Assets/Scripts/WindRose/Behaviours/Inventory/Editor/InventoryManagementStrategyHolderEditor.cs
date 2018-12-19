using System.Linq;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            using ManagementStrategies.UsageStrategies;

            [CustomEditor(typeof(InventoryManagementStrategyHolder))]
            [CanEditMultipleObjects]
            public class InventoryManagementStrategyHolderEditor : Editor
            {
                SerializedProperty strategy;

                protected virtual void OnEnable()
                {
                    strategy = serializedObject.FindProperty("mainUsageStrategy");
                }

                public override void OnInspectorGUI()
                {
                    serializedObject.Update();

                    InventoryManagementStrategyHolder underlyingObject = (serializedObject.targetObject as InventoryManagementStrategyHolder);
                    InventoryUsageManagementStrategy[] strategies = underlyingObject.GetComponents<InventoryUsageManagementStrategy>();
                    GUIContent[] strategyNames = (from strategy in strategies select new GUIContent(strategy.GetType().Name)).ToArray();

                    int index = ArrayUtility.IndexOf(strategies, strategy.objectReferenceValue as InventoryUsageManagementStrategy);
                    index = EditorGUILayout.Popup(new GUIContent("Main Usage Strategy"), index, strategyNames);
                    strategy.objectReferenceValue = index >= 0 ? strategies[index] : null;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
