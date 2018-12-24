using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                [CustomEditor(typeof(Item))]
                [CanEditMultipleObjects]
                public class ItemEditor : Editor
                {
                    SerializedProperty quantifyingStrategy;
                    SerializedProperty spatialStrategies;
                    SerializedProperty usageStrategies;
                    SerializedProperty renderingStrategies;
                    SerializedProperty mainUsageStrategy;
                    SerializedProperty mainRenderingStrategy;

                    protected virtual void OnEnable()
                    {
                        quantifyingStrategy = serializedObject.FindProperty("quantifyingStrategy");
                        spatialStrategies = serializedObject.FindProperty("spatialStrategies");
                        usageStrategies = serializedObject.FindProperty("usageStrategies");
                        renderingStrategies = serializedObject.FindProperty("renderingStrategies");
                        mainUsageStrategy = serializedObject.FindProperty("mainUsageStrategy");
                        mainRenderingStrategy = serializedObject.FindProperty("mainRenderingStrategy");
                    }

                    private Object RelatedPopup(string caption, SerializedProperty arrayProperty, SerializedProperty selectedElement)
                    {
                        List<Object> fetchedElements = new List<Object>();
                        List<GUIContent> fetchedElementTypeNames = new List<GUIContent>();

                        for(int i = 0; i < arrayProperty.arraySize; i++)
                        {
                            Object element = arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                            fetchedElements.Add(element);
                            fetchedElementTypeNames.Add(new GUIContent(element.GetType().FullName));
                        }

                        int index = fetchedElements.IndexOf(selectedElement.objectReferenceValue);
                        index = EditorGUILayout.Popup(new GUIContent(caption), index, fetchedElementTypeNames.ToArray());

                        return index > 0 ? fetchedElements[index] : null;
                    }

                    public override void OnInspectorGUI()
                    {
                        serializedObject.Update();

                        EditorGUILayout.PropertyField(quantifyingStrategy);
                        EditorGUILayout.PropertyField(spatialStrategies);
                        EditorGUILayout.PropertyField(usageStrategies);
                        mainUsageStrategy.objectReferenceValue = RelatedPopup("Main Usage Strategy", usageStrategies, mainUsageStrategy);
                        EditorGUILayout.PropertyField(renderingStrategies);
                        mainRenderingStrategy.objectReferenceValue = RelatedPopup("Main Rendering Strategy", renderingStrategies, mainRenderingStrategy);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
