using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace WindRose.Behaviours
{
    [CustomEditor(typeof(MapLoader))]
    public class MapLoaderEditor : Editor
    {
        [CustomPropertyDrawer(typeof(MapLoader.TilemapLayerSpec), true)]
        public class TilemapLayerSpecDrawer : PropertyDrawer
        {
            private class PropertyFieldArgs
            {
                public readonly SerializedProperty property;
                public readonly bool includeChildren;
                public PropertyFieldArgs(SerializedProperty property, bool includeChildren)
                {
                    this.property = property;
                    this.includeChildren = includeChildren;
                }
            }

            private IEnumerable<PropertyFieldArgs> GetPropertiesToDisplay(SerializedProperty property)
            {
                PropertyFieldArgs LayerType = new PropertyFieldArgs(property.FindPropertyRelative("LayerType"), false);
                PropertyFieldArgs FillingSource = new PropertyFieldArgs(property.FindPropertyRelative("FillingSource"), false);
                PropertyFieldArgs FillingSourceRect = new PropertyFieldArgs(property.FindPropertyRelative("FillingSourceRect"), false);
                PropertyFieldArgs FillingBlocks = new PropertyFieldArgs(property.FindPropertyRelative("FillingBlocks"), false);
                PropertyFieldArgs BiomeSource = new PropertyFieldArgs(property.FindPropertyRelative("BiomeSource"), false);
                PropertyFieldArgs BiomePresenceData = new PropertyFieldArgs(property.FindPropertyRelative("BiomePresenceData"), false);
                PropertyFieldArgs BiomeExtendedPresence = new PropertyFieldArgs(property.FindPropertyRelative("BiomeExtendedPresence"), false);
                PropertyFieldArgs BiomeBlockingMode = new PropertyFieldArgs(property.FindPropertyRelative("BiomeBlockingMode"), false);
                PropertyFieldArgs BiomeOffsetX = new PropertyFieldArgs(property.FindPropertyRelative("BiomeOffsetX"), false);
                PropertyFieldArgs BiomeOffsetY = new PropertyFieldArgs(property.FindPropertyRelative("BiomeOffsetY"), false);
                PropertyFieldArgs CustomSource = new PropertyFieldArgs(property.FindPropertyRelative("CustomSource"), false);
                PropertyFieldArgs CustomPalette = new PropertyFieldArgs(property.FindPropertyRelative("CustomPalette"), true);
                PropertyFieldArgs RandomSource = new PropertyFieldArgs(property.FindPropertyRelative("RandomSource"), false);
                PropertyFieldArgs RandomOptions = new PropertyFieldArgs(property.FindPropertyRelative("RandomOptions"), true);

                MapLoader.TilemapLayerSpec.LayerSpecType layerType = (MapLoader.TilemapLayerSpec.LayerSpecType) LayerType.property.enumValueIndex;
                IList<PropertyFieldArgs> propertiesList = new List<PropertyFieldArgs>() { LayerType };
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
                        if (RandomSource.property.objectReferenceValue != null)
                        {
                            propertiesList.Add(RandomOptions);
                        }
                        return propertiesList;
                    case MapLoader.TilemapLayerSpec.LayerSpecType.Filling:
                        propertiesList.Add(FillingSource);
                        propertiesList.Add(FillingSourceRect);
                        propertiesList.Add(FillingBlocks);
                        propertiesList.Add(RandomSource);
                        if (RandomSource.property.objectReferenceValue != null)
                        {
                            propertiesList.Add(RandomOptions);
                        }
                        return propertiesList;
                    case MapLoader.TilemapLayerSpec.LayerSpecType.Custom:
                        propertiesList.Add(CustomSource);
                        propertiesList.Add(CustomPalette);
                        return propertiesList;
                    default:
                        return propertiesList;
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                SerializedProperty EditorUnfolded = property.FindPropertyRelative("EditorUnfolded");
                bool unfolded = EditorGUILayout.Foldout(EditorUnfolded.boolValue, label);
                if (unfolded)
                {
                    EditorGUI.indentLevel++;
                    foreach (PropertyFieldArgs childProperty in GetPropertiesToDisplay(property))
                    {
                        EditorGUILayout.PropertyField(childProperty.property, childProperty.includeChildren);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorUnfolded.boolValue = unfolded;
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
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
