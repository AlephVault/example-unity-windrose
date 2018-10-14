using UnityEditor;
using UnityEngine;
using System.Linq;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Tilemaps
        {
            [CustomEditor(typeof(ObjectsTilemap), true)]
            [CanEditMultipleObjects]
            public class ObjectsTilemapEditor : Editor
            {
                SerializedProperty sortingLayer;
                GUIContent[] sortingLayerNames;
                int[] sortingLayerIDs;

                protected virtual void OnEnable()
                {
                    sortingLayer = serializedObject.FindProperty("sortingLayer");
                    sortingLayerNames = SortingLayer.layers.Select((layer) => new GUIContent(layer.name)).ToArray();
                    sortingLayerIDs = SortingLayer.layers.Select((layer) => layer.id).ToArray();
                }

                public override void OnInspectorGUI()
                {
                    serializedObject.Update();

                    EditorGUILayout.IntPopup(sortingLayer, sortingLayerNames, sortingLayerIDs, new GUIContent("Sorting Layer for Children"));

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}


