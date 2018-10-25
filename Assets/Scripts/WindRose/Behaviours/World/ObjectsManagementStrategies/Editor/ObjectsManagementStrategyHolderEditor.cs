using System.Linq;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                [CustomEditor(typeof(ObjectsManagementStrategyHolder))]
                [CanEditMultipleObjects]
                public class ObjectsManagementStrategyHolderEditor : Editor
                {
                    SerializedProperty strategy;

                    protected virtual void OnEnable()
                    {
                        strategy = serializedObject.FindProperty("strategy");
                    }

                    public override void OnInspectorGUI()
                    {
                        serializedObject.Update();

                        ObjectsManagementStrategyHolder underlyingObject = (serializedObject.targetObject as ObjectsManagementStrategyHolder);
                        ObjectsManagementStrategy[] strategies = underlyingObject.GetComponents<ObjectsManagementStrategy>();
                        GUIContent[] strategyNames = (from strategy in strategies select new GUIContent(strategy.GetType().Name)).ToArray();

                        int index = ArrayUtility.IndexOf(strategies, strategy.objectReferenceValue as ObjectsManagementStrategy);
                        index = EditorGUILayout.Popup(new GUIContent("Main Strategy"), index, strategyNames);
                        strategy.objectReferenceValue = index >= 0 ? strategies[index] : null;

                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
