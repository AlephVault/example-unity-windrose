﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Maps
        {
            using Support.Utils;
            using Behaviours.Inventory.ManagementStrategies.UsageStrategies;
            using Behaviours.Inventory.ManagementStrategies.SpatialStrategies;
            using Behaviours.World.Layers.Drop;
            using Behaviours.Inventory;

            public static class MapUtils
            {
                /// <summary>
                ///   Utility window used to create a Map. It fills its properties, adds optional layers,
                ///     and allows setting the names of the floors.
                /// </summary>
                public class CreateMapWindow : EditorWindow
                {
                    private Vector2Int mapSize = new Vector2Int(8, 6);
                    private Vector3 cellSize = Vector3.one;
                    private string mapObjectName = "New Map";
                    private string[] floors = new string[] { "New Floor" };
                    private bool addCeilingsLayer = false;
                    private bool addDropLayer = false;
                    private int strategy = 0;
                    private Vector2 scrollPosition = Vector2.zero;

                    private string[] UpdateFloors(string[] floors)
                    {
                        int newFloorsLength = EditorGUILayout.IntField("Length", floors.Length);
                        if (newFloorsLength < 0) newFloorsLength = 0;
                        string[] newFloors = new string[newFloorsLength];
                        int copyLength = Values.Min(newFloorsLength, floors.Length);
                        for (int i = 0; i < copyLength; i++) newFloors[i] = floors[i];
                        for (int i = copyLength; i < newFloorsLength; i++) newFloors[i] = "New Floor";
                        // Now the update cycles must run
                        for (int i = 0; i < newFloorsLength; i++)
                        {
                            newFloors[i] = MenuActionUtils.SimplifySpaces(EditorGUILayout.TextField(string.Format("Floor {0} name", i), newFloors[i]));
                            if (newFloors[i] == "") newFloors[i] = "Floor " + i;
                        }
                        return newFloors;
                    }

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle captionLabelStyle = MenuActionUtils.GetCaptionLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        minSize = new Vector2(643, 250);
                        maxSize = new Vector2(643, 300);

                        // General settings start here.

                        titleContent = new GUIContent("Wind Rose - Creating a new map");
                        EditorGUILayout.LabelField("This wizard will create a map in the hierarchy of the current scene, under the selected object in the hierarchy.", longLabelStyle);

                        // Map properties.

                        EditorGUILayout.BeginHorizontal();

                        // -> (Direct) map properties start here.

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Map properties", captionLabelStyle);

                        EditorGUILayout.LabelField("This is the name the game object will have when added to the hierarchy.", longLabelStyle);
                        mapObjectName = EditorGUILayout.TextField("Object name", mapObjectName);
                        mapObjectName = MenuActionUtils.SimplifySpaces(mapObjectName);
                        if (mapObjectName == "") mapObjectName = "New Map";

                        EditorGUILayout.LabelField("These are the map properties in the editor. Can be changed later.", longLabelStyle);
                        mapSize = EditorGUILayout.Vector2IntField("Map width (X) and height (Y) [1 to 32767]", mapSize);
                        mapSize = new Vector2Int(Values.Clamp(1, mapSize.x, 32767), Values.Clamp(1, mapSize.y, 32767)); 
                        cellSize = EditorGUILayout.Vector3Field("Cell Size", cellSize);
                        EditorGUILayout.EndVertical();

                        // -> Map layers properties.

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Map layers properties", captionLabelStyle);

                        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
                        EditorGUILayout.LabelField("Floors, Objects and Visuals layer will be added. There are more layers that can be added as well:", longLabelStyle);
                        addCeilingsLayer = EditorGUILayout.ToggleLeft("Ceilings Layer", addCeilingsLayer);
                        addDropLayer = EditorGUILayout.ToggleLeft("Drop Layer", addDropLayer);
                        if (addDropLayer)
                        {
                            // Specifications for default drop layer will appear here.
                            EditorGUILayout.BeginVertical(indentedStyle);
                            EditorGUILayout.LabelField("Drop Layers involve a specific set of strategies like:\n" +
                                                       "> Infinite simple spatial management strategy inside stack containers\n" +
                                                       "> Map-sized positioning strategy to locate stack containers\n" +
                                                       "> Drop-styled rendering strategy\n" +
                                                       "All that contained inside a new inventory manager component. For the usage strategy, " +
                                                       "the NULL strategy will be added (which usually works for most games), but it may be changed later.", longLabelStyle);
                            EditorGUILayout.LabelField("No drop container renderer prefab will automatically be used or generated in the rendering strategy. " +
                                                       "One MUST be created/reused later.", captionLabelStyle);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.LabelField("These are the names for each of the floors layers to be added in the hierarchy.", longLabelStyle);
                        floors = UpdateFloors(floors);
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();

                        // End Map properties.
                        // Map strategies.

                        EditorGUILayout.LabelField("Map strategies", captionLabelStyle);
                        strategy = EditorGUILayout.IntPopup(strategy, new string[] { "Simple (includes Solidness and Layout)", "Layout", "Nothing (will be added manually later)" }, new int[] { 0, 1, 2 });

                        // End Map strategies.
                        // Call to action.

                        if (GUILayout.Button("Create Map")) Execute();
                    }

                    private void Execute()
                    {
                        GameObject mapObject = new GameObject(mapObjectName);
                        mapObject.transform.parent = Selection.activeTransform;
                        mapObject.SetActive(false);
                        // Creating the map component & sorting group.
                        Layout.AddComponent<SortingGroup>(mapObject);
                        Layout.AddComponent<Behaviours.World.Map>(mapObject, new Dictionary<string, object>() {
                            { "width", (uint)mapSize.x },
                            { "height", (uint)mapSize.y },
                            { "cellSize", cellSize},
                        });
                        // Creating the strategy holder & strategies.
                        Behaviours.World.ObjectsManagementStrategies.ObjectsManagementStrategy mainStrategy = null;
                        switch(strategy)
                        {
                            case 0:
                                Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy>(mapObject);
                                Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Base.LayoutObjectsManagementStrategy>(mapObject);
                                Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Solidness.SolidnessObjectsManagementStrategy>(mapObject);
                                mainStrategy = Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Simple.SimpleObjectsManagementStrategy>(mapObject);
                                break;
                            case 1:
                                Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Base.BaseObjectsManagementStrategy>(mapObject);
                                mainStrategy = Layout.AddComponent<Behaviours.World.ObjectsManagementStrategies.Base.LayoutObjectsManagementStrategy>(mapObject);
                                break;
                            default:
                                Debug.LogWarning("A map is being just created with no main strategy. This map will be destroyed on play if no main strategy is set.");
                                break;
                        }
                        Behaviours.World.ObjectsManagementStrategyHolder holder = Layout.AddComponent<Behaviours.World.ObjectsManagementStrategyHolder>(mapObject, new Dictionary<string, object>() {
                            { "strategy", mainStrategy }
                        });
                        // Now, creating the layers as children AND the floors.
                        // 1. Floors layer.
                        GameObject floorLayer = new GameObject("FloorLayer");
                        floorLayer.transform.parent = mapObject.transform;
                        Layout.AddComponent<Grid>(floorLayer).cellSize = cellSize;
                        Layout.AddComponent<SortingGroup>(floorLayer);
                        Layout.AddComponent<Support.Behaviours.Normalized>(floorLayer);
                        Layout.AddComponent<Behaviours.World.Layers.Floor.FloorLayer>(floorLayer);
                        foreach (string floorName in floors)
                        {
                            GameObject floor = new GameObject(floorName);
                            floor.transform.parent = floorLayer.transform;
                            Layout.AddComponent<Tilemap>(floor);
                            Layout.AddComponent<TilemapRenderer>(floor);
                            Layout.AddComponent<Support.Behaviours.Normalized>(floor);
                            Layout.AddComponent<Behaviours.Floors.Floor>(floor);
                        }
                        // 2. Drop layer.
                        if (addDropLayer)
                        {
                            GameObject dropLayer = new GameObject("DropLayer");
                            dropLayer.transform.parent = mapObject.transform;
                            dropLayer.SetActive(false);
                            Layout.AddComponent<SortingGroup>(dropLayer);
                            Layout.AddComponent<Support.Behaviours.Normalized>(dropLayer);
                            Layout.AddComponent<InventoryInfiniteSimpleSpatialManagementStrategy>(dropLayer);
                            Layout.AddComponent<InventoryMapSizedPositioningManagementStrategy>(dropLayer);
                            InventoryNullUsageManagementStrategy usageStrategy = Layout.AddComponent<InventoryNullUsageManagementStrategy>(dropLayer);
                            InventoryDropLayerRenderingManagementStrategy renderingStrategy = Layout.AddComponent<InventoryDropLayerRenderingManagementStrategy>(dropLayer);
                            Layout.AddComponent<InventoryManagementStrategyHolder>(dropLayer, new Dictionary<string, object>() {
                                { "mainUsageStrategy", usageStrategy }
                            });
                            Layout.AddComponent<DropLayer>(dropLayer);
                            dropLayer.SetActive(true);
                        }
                        // 3. Objects layer.
                        GameObject objectsLayer = new GameObject("ObjectsLayer");
                        objectsLayer.transform.parent = mapObject.transform;
                        Layout.AddComponent<Grid>(objectsLayer).cellSize = cellSize;
                        Layout.AddComponent<SortingGroup>(objectsLayer);
                        Layout.AddComponent<Support.Behaviours.Normalized>(objectsLayer);
                        Layout.AddComponent<Behaviours.World.Layers.Objects.ObjectsLayer>(objectsLayer);
                        // 4. Visuals layer.
                        GameObject visualsLayer = new GameObject("VisualsLayer");
                        visualsLayer.transform.parent = mapObject.transform;
                        Layout.AddComponent<SortingGroup>(visualsLayer);
                        Layout.AddComponent<Support.Behaviours.Normalized>(visualsLayer);
                        Layout.AddComponent<Behaviours.World.Layers.Visuals.VisualsLayer>(visualsLayer);
                        // 5. Ceilings layer.
                        if (addCeilingsLayer)
                        {
                            GameObject ceilings = new GameObject("CeilingLayer");
                            ceilings.transform.parent = mapObject.transform;
                            Layout.AddComponent<Grid>(ceilings).cellSize = cellSize;
                            Layout.AddComponent<SortingGroup>(ceilings);
                            Layout.AddComponent<Support.Behaviours.Normalized>(ceilings);
                            Layout.AddComponent<Behaviours.World.Layers.Ceiling.CeilingLayer>(ceilings);
                        }
                        // Ok. Now activate the object.
                        mapObject.SetActive(true);
                        mapObject.AddComponent<Behaviours.World.Map>();
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in a menu action: GameObject > Wind Rose > Maps > Create Map.
                ///   It creates a map inside the selected object in the hierarchy.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Maps/Create Map", false, 11)]
                public static void CreateMap()
                {
                    CreateMapWindow window = ScriptableObject.CreateInstance<CreateMapWindow>();
                    window.position = new Rect(new Vector2(110, 250), new Vector2(643, 250));
                    window.ShowUtility();
                }
            }
        }
    }
}