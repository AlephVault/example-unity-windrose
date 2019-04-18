﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace UI
        {
            using Behaviours.UI;
            using Support.Utils;
            
            /// <summary>
            ///   Menu actions to create HUD objects (and, perhaps, cameras).
            /// </summary>
            public static class HUDUtils
            {
                private class CreateHUDWindow : EditorWindow
                {
                    private bool useSameObjectForCanvasAndHUD = true;
                    private bool addANewCamera = true;
                    private string hudObjectName = "New HUD";
                    private string canvasObjectName = "New HUD Canvas";
                    private string newCameraObjectName = "New HUD Camera";
                    private int orthographicSize = 6;

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        titleContent = new GUIContent("Wind Rose - Creating a new HUD");

                        Rect contentRect = EditorGUILayout.BeginVertical();

                        // General settings start here.

                        EditorGUILayout.LabelField("This wizard will create a HUD and, optionally, a camera.", longLabelStyle);

                        // HUD settings start here.

                        useSameObjectForCanvasAndHUD = EditorGUILayout.ToggleLeft("Put Canvas and HUD in the same object", useSameObjectForCanvasAndHUD);
                        addANewCamera = EditorGUILayout.ToggleLeft("Add a new camera", addANewCamera);

                        EditorGUILayout.LabelField("This is the name the HUD game object will have when added to the hierarchy.", longLabelStyle);
                        hudObjectName = EditorGUILayout.TextField("HUD name", hudObjectName);
                        hudObjectName = MenuActionUtils.SimplifySpaces(hudObjectName);
                        if (hudObjectName == "") hudObjectName = "New HUD";

                        if (!useSameObjectForCanvasAndHUD)
                        {
                            EditorGUILayout.LabelField("This is the name the Canvas game object for the HUD will have when added to the hierarchy.", longLabelStyle);
                            canvasObjectName = EditorGUILayout.TextField("Canvas name", canvasObjectName);
                            canvasObjectName = MenuActionUtils.SimplifySpaces(canvasObjectName);
                            if (canvasObjectName == "") canvasObjectName = "New HUD";
                        }

                        if (addANewCamera)
                        {
                            EditorGUILayout.LabelField("This is the name the Canvas game object for the HUD will have when added to the hierarchy.", longLabelStyle);
                            newCameraObjectName = EditorGUILayout.TextField("Camera name", newCameraObjectName);
                            newCameraObjectName = MenuActionUtils.SimplifySpaces(newCameraObjectName);
                            if (newCameraObjectName == "") newCameraObjectName = "New HUD";

                            orthographicSize = EditorGUILayout.IntField("Orthographic size", orthographicSize);
                            EditorGUILayout.LabelField("If lower than (or equal to) zero, the orthographic size will be set to 1.", longLabelStyle);
                            if (orthographicSize <= 0) orthographicSize = 1;
                        }

                        if (GUILayout.Button("Create HUD")) Execute();
                        EditorGUILayout.EndVertical();

                        if (contentRect.size != Vector2.zero)
                        {
                            minSize = contentRect.max + contentRect.min;
                            maxSize = minSize;
                        }
                    }

                    private void Execute()
                    {
                        string newCanvasName = useSameObjectForCanvasAndHUD ? hudObjectName : canvasObjectName;
                        GameObject newCanvasObject = new GameObject(newCanvasName);

                        Camera newCameraComponent = null;
                        if (addANewCamera)
                        {
                            GameObject newCameraObject = new GameObject(newCameraObjectName);
                            newCameraComponent = Layout.AddComponent<Camera>(newCameraObject);
                            newCameraComponent.orthographic = true;
                            newCameraComponent.orthographicSize = orthographicSize;
                        }

                        Canvas newCanvasComponent = Layout.AddComponent<Canvas>(newCanvasObject);
                        newCanvasComponent.worldCamera = newCameraComponent;
                        newCanvasComponent.renderMode = RenderMode.ScreenSpaceCamera;

                        GameObject newHudObject = useSameObjectForCanvasAndHUD ? newCanvasObject : new GameObject(hudObjectName);
                        HUD newHudComponent = Layout.AddComponent<HUD>(newHudObject, new Dictionary<string, object>()
                        {
                            { "canvas", useSameObjectForCanvasAndHUD ? null : newCanvasComponent }
                        });

                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Wind Rose > UI > Create HUD.
                ///   It creates a <see cref="Behaviours.UI.HUD"/>, perhaps with a camera, in the scene.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/UI/Create HUD", false, 11)]
                public static void CreateHUD()
                {
                    CreateHUDWindow window = ScriptableObject.CreateInstance<CreateHUDWindow>();
                    window.maxSize = new Vector2(550, 196);
                    window.minSize = window.maxSize;
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item GameObject > Wind Rose > UI > Create HUD.
                ///   It enables such menu option when no transform is selected in the scene
                ///     hierarchy.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/UI/Create HUD", true)]
                public static bool CanCreateObject()
                {
                    return Selection.activeTransform == null;
                }
            }
        }
    }
}
