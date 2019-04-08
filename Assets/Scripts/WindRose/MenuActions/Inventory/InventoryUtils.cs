using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Inventory
        {
            using Support.Utils;
            using Behaviours.Drops;

            public static class InventoryUtils
            {
                public class CreateDropContainerRendererPrefabWindow : EditorWindow
                {
                    private int imagesCount = 3;

                    private void OnGUI()
                    {
                        minSize = new Vector2(360, 100);
                        maxSize = minSize;

                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

                        titleContent = new GUIContent("Wind Rose - Creating a new drop container renderer prefab");
                        EditorGUILayout.LabelField("This wizard will create a simple drop container renderer prefab, which is used in DropLayer's rendering strategy.", longLabelStyle);
                        EditorGUILayout.Separator();
                        imagesCount = Values.Clamp(1, EditorGUILayout.IntField("Slots [1 to 32767]", imagesCount), 32767);
                        EditorGUILayout.Separator();
                        if (GUILayout.Button("Save"))
                        {
                            Execute();
                        }
                    }

                    private void Execute()
                    {
                        string path = EditorUtility.SaveFilePanel("Save Prefab", Path.Combine(Application.dataPath, "Prefabs"), "DropDisplay", "prefab");
                        if (path != "")
                        {
                            if (!path.StartsWith(Application.dataPath))
                            {
                                EditorUtility.DisplayDialog("Invalid path", "The chosen path is not inside project's data.", "OK");
                                return;
                            }
                            path = path.Substring(Path.GetDirectoryName(Application.dataPath).Length + 1);
                            GameObject prefab = new GameObject("DropDisplay");
                            prefab.AddComponent<SortingGroup>();
                            prefab.AddComponent<SimpleDropContainerRenderer>();
                            for(int i = 0; i < imagesCount; i++)
                            {
                                GameObject image = new GameObject("Img" + i);
                                image.AddComponent<SpriteRenderer>();
                                image.transform.parent = prefab.transform;
                            }
                            PrefabUtility.CreatePrefab(path, prefab);
                            DestroyImmediate(prefab);
                            Close();
                            EditorUtility.DisplayDialog("Save Successful", "The drop container prefab was successfully saved. However, it can be configured to add/remove slots and/or set a background image.", "OK");
                        }
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Wind Rose > visuals > Create Visual.
                /// </summary>
                [MenuItem("Assets/Create/Wind Rose/Inventory/Drop Container Renderer Prefab")]
                public static void CreateVisual()
                {
                    CreateDropContainerRendererPrefabWindow window = ScriptableObject.CreateInstance<CreateDropContainerRendererPrefabWindow>();
                    window.position = new Rect(new Vector2(230, 350), new Vector2(360, 100));
                    window.ShowUtility();
                }
            }
        }
    }
}