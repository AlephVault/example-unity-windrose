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
                using Support.Utils;
                using GabTab.Behaviours;
                using GabTab.Behaviours.Interactors;
                using System.Reflection;

                /// <summary>
                ///   Menu actions to create button sets (1, 2 or 3 buttons) to be used
                ///     inside an <see cref="InteractiveInterface"/>.
                /// </summary>
                public static class ButtonsUtils
                {
                    private class ButtonSettings
                    {
                        public string key = "";
                        public string caption = "Button";
                        public ColorBlock colors;
                        public Color textColor = Color.black;

                        public ButtonSettings(string key, string caption)
                        {
                            this.key = key;
                            this.caption = caption;
                            colors.normalColor = new Color32(255, 255, 255, 255);
                            colors.highlightedColor = new Color32(245, 245, 245, 255);
                            colors.pressedColor = new Color32(200, 200, 200, 255);
                            colors.disabledColor = new Color32(200, 200, 200, 255);
                            colors.fadeDuration = 0.1f;
                            colors.colorMultiplier = 1f;
                        }
                    }

                    private class CreateButtonsInteractorWindow : EditorWindow
                    {
                        private string buttonsInteractorName = "New Buttons Interactor";
                        private ButtonSettings[] buttonsSettings = new ButtonSettings[] {
                            new ButtonSettings("button-1", "Button 1"), new ButtonSettings("button-2", "Button 2"), new ButtonSettings("button-3", "Button 3")
                        };
                        private int buttonsCount = 1;
                        private bool withBackground = false;
                        private Color backgroundTint = Color.white;
                        public Transform selectedTransform = null;

                        private void ButtonsSettingsGUI(int index, ButtonSettings settings, GUIStyle style)
                        {
                            settings.key = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Button key", settings.key), "button-" + index);
                            EditorGUILayout.BeginVertical(style);
                            settings.caption = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Caption", settings.caption), "Button " + index);
                            if (settings.caption == "") settings.caption = "Button " + index;
                            settings.colors.normalColor = EditorGUILayout.ColorField("Normal color", settings.colors.normalColor);
                            settings.colors.highlightedColor = EditorGUILayout.ColorField("Highlighted color", settings.colors.highlightedColor);
                            settings.colors.pressedColor = EditorGUILayout.ColorField("Pressed color", settings.colors.pressedColor);
                            settings.colors.disabledColor = EditorGUILayout.ColorField("Disabled color", settings.colors.disabledColor);
                            settings.textColor = EditorGUILayout.ColorField("Text color", settings.textColor);
                            EditorGUILayout.EndVertical();
                        }

                        private void AllButtonsSettingsGUI(GUIStyle style)
                        {
                            buttonsCount = EditorGUILayout.IntSlider("Buttons #", buttonsCount, 1, 3);
                            for(int i = 0; i < buttonsCount; i++)
                            {
                                ButtonsSettingsGUI(i, buttonsSettings[i], style);
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

                        private void AddButton(ButtonsInteractor.ButtonKeyDictionary buttons, int index, RectTransform parent, ButtonSettings settings, float buttonsOffset, Rect interactorRect)
                        {
                            GameObject buttonObject = new GameObject(settings.key);
                            buttonObject.transform.parent = parent;
                            float buttonWidth = (interactorRect.width - 5 * buttonsOffset) / 4;
                            float buttonHeight = (interactorRect.height - 2 * buttonsOffset);
                            RectTransform rectTransformComponent = Layout.AddComponent<RectTransform>(buttonObject);
                            rectTransformComponent.pivot = Vector2.zero;
                            rectTransformComponent.anchorMin = Vector2.zero;
                            rectTransformComponent.anchorMax = Vector2.zero;
                            int position = 4 - index;
                            rectTransformComponent.offsetMin = new Vector2(position * buttonsOffset + (position - 1) * buttonWidth, buttonsOffset);
                            rectTransformComponent.offsetMax = rectTransformComponent.offsetMin;
                            rectTransformComponent.sizeDelta = new Vector2(buttonWidth, buttonHeight);
                            Image buttonImageComponent = Layout.AddComponent<Image>(buttonObject);
                            buttonImageComponent.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                            buttonImageComponent.type = Image.Type.Sliced;
                            buttonImageComponent.fillCenter = true;
                            Button buttonComponent = Layout.AddComponent<Button>(buttonObject);
                            buttonComponent.colors = settings.colors;
                            buttonComponent.targetGraphic = buttonImageComponent;
                            GameObject textObject = new GameObject("Text");
                            textObject.transform.parent = buttonObject.transform;
                            Text textComponent = Layout.AddComponent<Text>(textObject);
                            textComponent.text = settings.caption;
                            textComponent.fontSize = (int)(buttonHeight / 2);
                            textComponent.alignment = TextAnchor.MiddleCenter;
                            textComponent.color = settings.textColor;
                            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
                            textRectTransform.pivot = Vector2.one / 2f;
                            textRectTransform.anchorMin = Vector2.zero;
                            textRectTransform.anchorMax = Vector2.one;
                            textRectTransform.offsetMin = Vector2.zero;
                            textRectTransform.offsetMax = Vector2.zero;
                            buttons.Add(buttonComponent, settings.key);
                        }

                        private void Execute()
                        {
                            Rect canvasRect = selectedTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
                            float buttonsOffset = Values.Max(canvasRect.height, canvasRect.width) * 0.01f;

                            GameObject interactorObject = InteractorUtils.AddBaseInteractorLayout(selectedTransform.GetComponent<InteractiveInterface>(), buttonsInteractorName, withBackground, backgroundTint);
                            Rect interactorRect = interactorObject.GetComponent<RectTransform>().rect;
                            ButtonsInteractor.ButtonKeyDictionary buttons = new ButtonsInteractor.ButtonKeyDictionary();
                            RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                            ButtonsInteractor buttonsInteractorComponent = Layout.AddComponent<ButtonsInteractor>(interactorObject, new Dictionary<string, object>()
                            {
                                { "buttons", buttons }
                            });
                            for (int index = 0; index < 3; index++)
                            {
                                if (buttonsCount > index) AddButton(buttons, index, interactorRectTransformComponent, buttonsSettings[index], buttonsOffset, interactorRect);
                            }
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
