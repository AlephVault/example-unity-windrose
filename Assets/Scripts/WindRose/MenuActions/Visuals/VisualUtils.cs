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
        namespace Visuals
        {
            using Support.Utils;
            using ScriptableObjects.Animations;
            using Behaviours.Entities.Visuals;
            using Behaviours.Entities.Visuals.StateBundles.Moving;
            using Behaviours.Entities.Common;

            public static class VisualUtils
            {
                public class CreateVisualWindow : EditorWindow
                {
                    private static int[] visualTypes = new int[] { 0, 1, 2, 3, 4, 5 };
                    private static string[] visualTypeLabels = new string[] {
                        "Static (Visual - Using Sprite)",
                        string.Format("Animated (Visual, Animated - Using {0})", typeof(Animation).FullName),
                        string.Format("Rose-Animated (Visual, Animated, RoseAnimated - Using {0})", typeof(AnimationRose).FullName),
                        "Multi-State static (Visual, MultiSprite - Using Sprite)",
                        string.Format("Multi-State Animated (Visual, Animated, MultiAnimated - Using {0})", typeof(Animation).FullName),
                        string.Format("Multi-State Rose-Animated (Visual, Animated, RoseAnimated, MultiRoseAnimated - Using {0})", typeof(AnimationRose).FullName),
                    };

                    private string visualObjectName = "";
                    private int visualType = 0;
                    private ushort visualLevel = 1 << 14;
                    private bool addMovingBundle = false;

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle captionLabelStyle = MenuActionUtils.GetCaptionLabelStyle();

                        minSize = new Vector2(689, 138);
                        maxSize = minSize;

                        // General settings start here.

                        titleContent = new GUIContent("Wind Rose - Creating a new visual");
                        EditorGUILayout.LabelField("This wizard will create a visual object in the hierarchy of the current scene, under the selected object in the hierarchy.", longLabelStyle);

                        // Visual properties.

                        EditorGUILayout.LabelField("This is the name the game object will have when added to the hierarchy.", longLabelStyle);
                        visualObjectName = EditorGUILayout.TextField("Object name", visualObjectName);
                        visualObjectName = MenuActionUtils.SimplifySpaces(visualObjectName);
                        if (visualObjectName == "") visualObjectName = "New Visual";

                        EditorGUILayout.LabelField("Visual type", captionLabelStyle);
                        visualType = EditorGUILayout.IntPopup(visualType, visualTypeLabels, visualTypes);
                        visualLevel = (ushort)Values.Clamp(0, EditorGUILayout.IntField("Level [1 to 32767]", visualLevel), (1 << 15) - 1);

                        if (visualType >= 3)
                        {
                            // Visual bundles.

                            EditorGUILayout.LabelField("While the Multi-State behaviours already provide a setting for idle state display, more state bundles can be added to support states in standard behaviours:", longLabelStyle);
                            addMovingBundle = EditorGUILayout.ToggleLeft("Moving (e.g. for walking characters)", addMovingBundle);
                        }

                        if (GUILayout.Button("Create Visual")) Execute();
                    }

                    private void Execute()
                    {
                        GameObject gameObject = new GameObject(visualObjectName);
                        gameObject.transform.parent = Selection.activeTransform;
                        gameObject.SetActive(false);
                        Layout.AddComponent<Pausable>(gameObject);
                        Layout.AddComponent<SpriteRenderer>(gameObject);
                        Layout.AddComponent<Visual>(gameObject, new Dictionary<string, object>() {
                            { "level", visualLevel }
                        });

                        switch (visualType)
                        {
                            case 1:
                                Layout.AddComponent<Animated>(gameObject);
                                break;
                            case 2:
                                Layout.AddComponent<Animated>(gameObject);
                                Layout.AddComponent<RoseAnimated>(gameObject);
                                break;
                            case 3:
                                Layout.AddComponent<MultiSprite>(gameObject);
                                if (addMovingBundle)
                                {
                                    Layout.AddComponent<MovingSpriteBundle>(gameObject);
                                }
                                break;
                            case 4:
                                Layout.AddComponent<Animated>(gameObject);
                                if (addMovingBundle)
                                {
                                    Layout.AddComponent<MovingAnimationBundle>(gameObject);
                                }
                                break;
                            case 5:
                                Layout.AddComponent<Animated>(gameObject);
                                Layout.AddComponent<RoseAnimated>(gameObject);
                                if (addMovingBundle)
                                {
                                    Layout.AddComponent<MovingAnimationRoseBundle>(gameObject);
                                }
                                break;
                        }
                        gameObject.SetActive(true);
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Wind Rose > visuals > Create Visual.
                ///   It creates a <see cref="Behaviours.Entities.Visuals.Visual"/> under the selected transform,
                ///     in the scene editor, that has a <see cref="Behaviours.Entities.Objects.Object"/> component.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Visuals/Create Visual", false, 11)]
                public static void CreateVisual()
                {
                    CreateVisualWindow window = ScriptableObject.CreateInstance<CreateVisualWindow>();
                    window.position = new Rect(new Vector2(57, 336), new Vector2(689, 138));
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item GameObject > Wind Rose > visuals > Create Visual.
                ///   It enables such menu option when an <see cref="Behaviours.Entities.Objects.Object"/>
                ///     is selected in the scene editor.
                /// </summary>
                [MenuItem("GameObject/Wind Rose/Visuals/Create Visual", true)]
                public static bool CanCreateVisual()
                {
                    return Selection.activeTransform && Selection.activeTransform.GetComponent<Behaviours.Entities.Objects.Object>();
                }
            }
        }
    }
}