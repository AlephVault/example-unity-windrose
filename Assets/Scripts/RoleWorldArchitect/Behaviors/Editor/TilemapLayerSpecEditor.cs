using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace RoleWorldArchitect.Behaviors
{
    [CustomEditor(typeof(MapLoader))]
    public class MapLoaderEditor : Editor
    {
        [CustomPropertyDrawer(typeof(MapLoader.TilemapLayerSpec), true)]
        public class TilemapLayerSpecDrawer : PropertyDrawer
        {
            private IEnumerable<SerializedProperty> GetPropertiesToDisplay(SerializedProperty property)
            {
                SerializedProperty LayerType = property.FindPropertyRelative("LayerType");
                SerializedProperty FillingSource = property.FindPropertyRelative("FillingSource");
                SerializedProperty FillingSourceRect = property.FindPropertyRelative("FillingSourceRect");
                SerializedProperty FillingBlocks = property.FindPropertyRelative("FillingBlocks");
                SerializedProperty BiomeSource = property.FindPropertyRelative("BiomeSource");
                SerializedProperty BiomePresenceData = property.FindPropertyRelative("BiomePresenceData");
                SerializedProperty BiomeExtendedPresence = property.FindPropertyRelative("BiomeExtendedPresence");
                SerializedProperty BiomeBlockingMode = property.FindPropertyRelative("BiomeBlockingMode");
                SerializedProperty BiomeOffsetX = property.FindPropertyRelative("BiomeOffsetX");
                SerializedProperty BiomeOffsetY = property.FindPropertyRelative("BiomeOffsetY");
                SerializedProperty RandomSource = property.FindPropertyRelative("RandomSource");
                SerializedProperty RandomOptions = property.FindPropertyRelative("RandomOptions");

                MapLoader.TilemapLayerSpec.LayerSpecType layerType = (MapLoader.TilemapLayerSpec.LayerSpecType) LayerType.enumValueIndex;
                IList<SerializedProperty> propertiesList = new List<SerializedProperty>() { LayerType };
                switch(layerType)
                {
                    case MapLoader.TilemapLayerSpec.LayerSpecType.Biome:
                        propertiesList.Add(BiomeSource);
                        propertiesList.Add(BiomePresenceData);
                        propertiesList.Add(BiomeExtendedPresence);
                        propertiesList.Add(BiomeBlockingMode);
                        propertiesList.Add(BiomeOffsetX);
                        propertiesList.Add(BiomeOffsetY);
                        propertiesList.Add(RandomSource);
                        if (RandomSource.objectReferenceValue != null)
                        {
                            propertiesList.Add(RandomOptions);
                        }
                        return propertiesList;
                    case MapLoader.TilemapLayerSpec.LayerSpecType.Filling:
                        propertiesList.Add(FillingSource);
                        propertiesList.Add(FillingSourceRect);
                        propertiesList.Add(FillingBlocks);
                        propertiesList.Add(RandomSource);
                        if (RandomSource.objectReferenceValue != null)
                        {
                            propertiesList.Add(RandomOptions);
                        }
                        return propertiesList;
                    default:
                        return propertiesList;
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUILayout.LabelField(label.text);
                //EditorGUI.BeginProperty(position, label, property);
                EditorGUI.indentLevel++;
                foreach (SerializedProperty childProperty in GetPropertiesToDisplay(property))
                {
                    EditorGUILayout.PropertyField(childProperty);
                }
                EditorGUI.indentLevel--;
                //EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return 0;
            }
        }

        SerializedProperty Width;
        SerializedProperty Height;
        SerializedProperty Layers;
        SerializedProperty TileSize;

        private void OnEnable()
        {
            Width = serializedObject.FindProperty("Width");
            Height = serializedObject.FindProperty("Height");
            TileSize = serializedObject.FindProperty("TileSize");
            Layers = serializedObject.FindProperty("Layers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Width);
            EditorGUILayout.PropertyField(Height);
            EditorGUILayout.PropertyField(Layers, true);
            EditorGUILayout.PropertyField(TileSize);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
