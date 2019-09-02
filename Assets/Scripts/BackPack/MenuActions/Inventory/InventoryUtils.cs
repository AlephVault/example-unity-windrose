﻿using System.IO;
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
            ///   Menu actions to create inventory-related assets / add inventory-related components.
            /// </summary>
            public static class InventoryUtils
            {
                private class CreateDropContainerRendererPrefabWindow : EditorWindow
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
                [MenuItem("Assets/Create/Back Pack/Inventory/Drop Container Renderer Prefab")]
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

                private class AddBagWindow : EditorWindow
                {
                    public Transform selectedTransform;
                    private bool finiteBag = true;
                    private int bagSize = 10;

                    private void OnGUI()
                    {
                        minSize = new Vector2(590, 144);
                        maxSize = new Vector2(590, 144);
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        EditorGUILayout.LabelField("Bags involve a specific set of strategies like:\n" +
                                                   "> simple spatial management strategy inside stack containers (finite or infinite, but slot-indexed)\n" +
                                                   "> Single-positioning strategy to locate stack containers (only one stack container: the bag)\n" +
                                                   "> Slot/Drop-styled rendering strategy\n" +
                                                   "All that contained inside a new inventory manager component. For the usage strategy, " +
                                                   "the NULL strategy will be added, which should be changed later or the items in the bag will have no logic.", longLabelStyle);
                        finiteBag = EditorGUILayout.ToggleLeft("Has a limited size", finiteBag);
                        if (finiteBag)
                        {
                            bagSize = EditorGUILayout.IntField("Bag size (>= 0)", bagSize);
                            if (bagSize < 1) bagSize = 1;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Bag size is potentially infinite.");
                        }
                        if (GUILayout.Button("Add a bag behaviour to the selected object"))
                        {
                            Execute();
                        }
                        Debug.LogFormat("Position: {0}", this.position);
                    }

                    private void Execute()
                    {
                        GameObject gameObject = Selection.activeTransform.gameObject;
                        Behaviours.Inventory.ManagementStrategies.UsageStrategies.InventoryNullUsageManagementStrategy usageStrategy =
                            Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.UsageStrategies.InventoryNullUsageManagementStrategy>(gameObject);
                        Layout.AddComponent<Behaviours.Inventory.InventoryManagementStrategyHolder>(gameObject, new Dictionary<string, object>() {
                            { "mainUsageStrategy", usageStrategy }
                        });
                        if (finiteBag)
                        {
                            Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.SpatialStrategies.InventoryFiniteSimpleSpatialManagementStrategy>(gameObject, new Dictionary<string, object>()
                            {
                                { "size", bagSize }
                            });
                        }
                        else
                        {
                            Layout.AddComponent<Behaviours.Inventory.ManagementStrategies.SpatialStrategies.InventoryInfiniteSimpleSpatialManagementStrategy>(gameObject);
                        }
                        Layout.AddComponent<Behaviours.Entities.Objects.Bags.SimpleBag>(gameObject);
                        Close();
                    }
                }

                [MenuItem("GameObject/Back Pack/Inventory/Add Bag", false, 11)]
                public static void AddBag()
                {
                    AddBagWindow window = ScriptableObject.CreateInstance<AddBagWindow>();
                    window.selectedTransform = Selection.activeTransform;
                    window.position = new Rect(275, 327, 590, 144);
                    window.ShowUtility();
                }

                [MenuItem("GameObject/Back Pack/Inventory/Add Bag", true)]
                public static bool CanAddBag()
                {
                    return Selection.activeTransform && Selection.activeTransform.GetComponent<WindRose.Behaviours.Entities.Objects.MapObject>();
                }
            }
        }
    }
}