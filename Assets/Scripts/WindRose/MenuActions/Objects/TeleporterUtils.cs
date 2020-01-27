﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace Objects
        {
            using GMM.Utils;

            /// <summary>
            ///   Menu actions to create a teleporter object inside an Objects layer.
            /// </summary>
            public static class TeleporterUtils
            {
                private class CreateLocalTeleporterWindow : EditorWindow
                {
                    public Transform selectedTransform;
                    private string objectName = "New Teleporter";
                    private Vector2Int objectSize;
                    private bool addTeleportTargetBehaviour = false;

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle captionLabelStyle = MenuActionUtils.GetCaptionLabelStyle();

                        titleContent = new GUIContent("Wind Rose - Creating a new local teleporter object");
                        EditorGUILayout.LabelField("This wizard will create a local teleporter object in the hierarchy of the current scene, under the selected objects layer in the hierarchy.", longLabelStyle);

                        EditorGUILayout.LabelField("This is the name the game object will have when added to the hierarchy.", longLabelStyle);
                        objectName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", objectName), "New Teleporter");

                        EditorGUILayout.LabelField("These are the object properties in the editor. Can be changed later.", longLabelStyle);
                        objectSize = EditorGUILayout.Vector2IntField("Object width (X) and height (Y) [1 to 32767]", objectSize);
                        objectSize = new Vector2Int(Values.Clamp(1, objectSize.x, 32767), Values.Clamp(1, objectSize.y, 32767));

                        addTeleportTargetBehaviour = EditorGUILayout.ToggleLeft("This teleport will also be a target for other possible teleports", addTeleportTargetBehaviour);

                        EditorGUILayout.LabelField("A target must be manually set for this teleporter. The target must have dimensions supporting the intended object(s) to be transferred.", captionLabelStyle);

                        if (GUILayout.Button("Create Object")) Execute();
                    }

                    private void Execute()
                    {
                        GameObject gameObject = new GameObject(objectName);
                        gameObject.transform.parent = selectedTransform;
                        gameObject.SetActive(false);
                        Layout.AddComponent<Behaviours.Entities.Objects.MapObject>(gameObject, new Dictionary<string, object>() {
                            { "width", (uint)objectSize.x },
                            { "height", (uint)objectSize.y }
                        });
                        Layout.AddComponent<Behaviours.Entities.Objects.Teleport.LocalTeleporter>(gameObject);
                        if (addTeleportTargetBehaviour)
                        {
                            Layout.AddComponent<Behaviours.Entities.Objects.Teleport.TeleportTarget>(gameObject);
                        }
                        gameObject.SetActive(true);
                        Undo.RegisterCreatedObjectUndo(gameObject, "Create Local Teleporter");
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Wind Rose > Objects > Create Local Teleporter.
                ///   It creates a <see cref="Behaviours.Entities.Objects.Teleport.LocalTeleporter"/> under the selected objects
                ///     layer, in the scene editor.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Objects/Create Local Teleporter", false, 12)]
                public static void CreateLocalTeleporter()
                {
                    CreateLocalTeleporterWindow window = ScriptableObject.CreateInstance<CreateLocalTeleporterWindow>();
                    window.position = new Rect(60, 180, 700, 468);
                    window.minSize = new Vector2(700, 196);
                    window.maxSize = new Vector2(700, 196);
                    window.selectedTransform = Selection.activeTransform;
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item: GameObject > Wind Rose > Objects > Create Local Teleporter.
                ///   It enables such menu option when an <see cref="Behaviours.World.Layers.Objects.ObjectsLayer"/>
                ///     is selected in the scene editor.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Objects/Create Local Teleporter", true)]
                public static bool CanCreateLocalTeleporter()
                {
                    return Selection.activeTransform && Selection.activeTransform.GetComponent<Behaviours.World.Layers.Objects.ObjectsLayer>();
                }
            }
        }
    }
}
