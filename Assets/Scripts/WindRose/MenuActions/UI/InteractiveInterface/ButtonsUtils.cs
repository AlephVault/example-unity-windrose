using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace WindRose
{
    namespace MenuActions
    {
        namespace UI
        {
            namespace InteractiveInterface
            {
                using GabTab.Behaviours;

                /// <summary>
                ///   Menu actions to create button sets (1, 2 or 3 buttons) to be used
                ///     inside an <see cref="InteractiveInterface"/>.
                /// </summary>
                public static class ButtonsUtils
                {
                    private class ButtonsSettings
                    {
                        public string key = "";
                        public string caption = "Button";
                        public Color32 normalColor = new Color32(255, 255, 255, 255);
                        public Color32 highlightColor = new Color32(245, 245, 245, 255);
                        public Color32 pressedColor = new Color32(200, 200, 200, 255);
                        public Color32 disabledColor = new Color32(200, 200, 200, 255);

                        public ButtonsSettings(string key, string caption)
                        {
                            this.key = key;
                            this.caption = caption;
                        }
                    }

                    private class CreateButtonsInteractorWindow : EditorWindow
                    {
                        private string buttonsInteractorName = "New Buttons Interactor";
                        private ButtonsSettings[] buttons = new ButtonsSettings[] {
                            new ButtonsSettings("button-1", "Button 1"), new ButtonsSettings("button-2", "Button 2"), new ButtonsSettings("button-3", "Button 3")
                        };
                        private int buttonsCount = 1;
                        private bool withBackground = false;
                        private Color backgroundTint = Color.white;
                        public Transform selectedTransform = null;

                        private void ButtonsSettingsGUI(int index, ButtonsSettings settings, GUIStyle style)
                        {
                            settings.key = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Button key", settings.key), "button-" + index);
                            EditorGUILayout.BeginVertical(style);
                            settings.caption = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Caption", settings.caption), "Button " + index);
                            if (settings.caption == "") settings.caption = "Button " + index;
                            settings.normalColor = EditorGUILayout.ColorField("Normal color", settings.normalColor);
                            settings.highlightColor = EditorGUILayout.ColorField("Highlight color", settings.highlightColor);
                            settings.pressedColor = EditorGUILayout.ColorField("Pressed color", settings.pressedColor);
                            settings.disabledColor = EditorGUILayout.ColorField("Disabled color", settings.disabledColor);
                            EditorGUILayout.EndVertical();
                        }

                        private void AllButtonsSettingsGUI(GUIStyle style)
                        {
                            buttonsCount = EditorGUILayout.IntSlider("Buttons #", buttonsCount, 1, 3);
                            for(int i = 0; i < buttonsCount; i++)
                            {
                                ButtonsSettingsGUI(i, buttons[i], style);
                            }
                        }

                        private void OnGUI()
                        {
                            GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                            GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                            titleContent = new GUIContent("Wind Rose - Creating a new Buttons Interactor");

                            Rect contentRect = EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("This wizard will create an interactor with one, two, or three buttons.", longLabelStyle);

                            buttonsInteractorName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", buttonsInteractorName), "New Buttons Interactor");

                            AllButtonsSettingsGUI(indentedStyle);
                            withBackground = EditorGUILayout.ToggleLeft("Add background", withBackground);
                            if (withBackground)
                            {
                                EditorGUILayout.BeginVertical(indentedStyle);
                                backgroundTint = EditorGUILayout.ColorField("Background tint", backgroundTint);
                                EditorGUILayout.EndVertical();
                            }
                            if (GUILayout.Button("Create Buttons Interactor"))
                            {
                                Execute();
                            }
                            EditorGUILayout.EndVertical();

                            if (contentRect.size != Vector2.zero)
                            {
                                minSize = contentRect.max + contentRect.min;
                                maxSize = minSize;
                            }
                        }

                        private void Execute()
                        {
                            InteractorUtils.AddBackground(selectedTransform.GetComponent<InteractiveInterface>(), buttonsInteractorName, withBackground, backgroundTint);
                            // Close();
                        }
                    }

                    /// <summary>
                    ///   This method is used in the menu action: GameObject > Wind Rose > UI > HUD > Interactive Interface > Create Buttons Interactor.
                    ///   It creates a <see cref="InteractiveInterface"/>, with their inner message component, in the scene.
                    /// </summary>
                    [MenuItem("GameObject/Wind Rose/UI/HUD/Interactive Interface/Create Buttons Interactor", false, 11)]
                    public static void CreateInteractiveInterface()
                    {
                        CreateButtonsInteractorWindow window = ScriptableObject.CreateInstance<CreateButtonsInteractorWindow>();
                        window.maxSize = new Vector2(400, 176);
                        window.minSize = window.maxSize;
                        window.selectedTransform = Selection.activeTransform;
                        window.ShowUtility();
                    }

                    /// <summary>
                    ///   Validates the menu item: GameObject > Wind Rose > UI > HUD > Interactive Interface > Create Buttons Interactor.
                    ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                    /// </summary>
                    [MenuItem("GameObject/Wind Rose/UI/HUD/Interactive Interface/Create Buttons Interactor", true)]
                    public static bool CanCreateInteractiveInterface()
                    {
                        return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                    }
                }
            }
        }
    }
}
