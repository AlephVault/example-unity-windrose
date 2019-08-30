using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace BackPack
{
    namespace MenuActions
    {
        namespace Inventory
        {
            using Support.Utils;
            using Behaviours.Drops;

            /// <summary>
            ///   Menu actions to create inventory-related assets.
            /// </summary>
            public static class InventoryUtils
            {
                public class CreateDropContainerRendererPrefabWindow : EditorWindow
                {
                    private int imagesCount = 3;
                    private string prefabName = "DropDisplay";
                    public string prefabPath;

                    private void OnGUI()
                    {
                        minSize = new Vector2(360, 110);
                        maxSize = minSize;

                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

                        titleContent = new GUIContent("Wind Rose - Creating a new drop container renderer prefab");
                        EditorGUILayout.LabelField("This wizard will create a simple drop container renderer prefab, which is used in DropLayer's rendering strategy.", longLabelStyle);
                        EditorGUILayout.Separator();
                        prefabName = EditorGUILayout.TextField("Name", prefabName);
                        imagesCount = Values.Clamp(1, EditorGUILayout.IntField("Slots [1 to 32767]", imagesCount), 32767);
                        EditorGUILayout.Separator();
                        if (GUILayout.Button("Save"))
                        {
                            Execute();
                        }
                    }

                    private void Execute()
                    {
                        string relativePrefabPath = string.Format("{0}/{1}.prefab", prefabPath, prefabName);
                        GameObject prefab = new GameObject(prefabName);
                        prefab.AddComponent<SortingGroup>();
                        prefab.AddComponent<SimpleDropContainerRenderer>();
                        for(int i = 0; i < imagesCount; i++)
                        {
                            GameObject image = new GameObject("Img" + i);
                            image.AddComponent<SpriteRenderer>();
                            image.transform.parent = prefab.transform;
                        }
                        PrefabUtility.CreatePrefab(relativePrefabPath, prefab);
                        DestroyImmediate(prefab);
                        Close();
                        EditorUtility.DisplayDialog("Save Successful", "The drop container prefab was successfully saved. However, it can be configured to add/remove slots and/or set a background image.", "OK");
                    }
                }

                /// <summary>
                ///   This method is used in the assets menu action: Create > Wind Rose > Inventory > Drop Container Renderer Prefab.
                /// </summary>
                [MenuItem("Assets/Create/Wind Rose/Inventory/Drop Container Renderer Prefab")]
                public static void CreatePrefab()
                {
                    CreateDropContainerRendererPrefabWindow window = ScriptableObject.CreateInstance<CreateDropContainerRendererPrefabWindow>();
                    window.position = new Rect(new Vector2(230, 350), new Vector2(360, 110));
                    string newAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    if (newAssetPath == "")
                    {
                        newAssetPath = Path.Combine("Assets", "Prefabs");
                    }
                    string projectPath = Path.GetDirectoryName(Application.dataPath);
                    if (!Directory.Exists(Path.Combine(projectPath, newAssetPath)))
                    {
                        newAssetPath = Path.GetDirectoryName(newAssetPath);
                    }
                    Debug.Log("Using path: " + newAssetPath);
                    window.prefabPath = newAssetPath;
                    window.ShowUtility();
                }
            }
        }
    }
}