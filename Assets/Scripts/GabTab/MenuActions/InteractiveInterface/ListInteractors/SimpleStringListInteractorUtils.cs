using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GabTab
{
    namespace MenuActions
    {
        namespace InteractiveInterface
        {
            namespace ListInteractors
            {
                using Support.Utils;
                using Behaviours;
                using Behaviours.Interactors.DefaultLists;

                /// <summary>
                ///   Menu actions to create a <see cref="SimpleStringListInteractor"/>
                ///     inside an <see cref="InteractiveInterface"/>.
                /// </summary>
                public static class SimpleStringListInteractorUtils
                {
                    private class CreateSimpleStringListInteractorWindow : EditorWindow
                    {
                        public Transform selectedTransform;
                        private string simpleStringListInteractorName = "New Simple Strings Interactor";
                        private bool withBackground = false;
                        private Color backgroundTint = Color.white;
                        private bool multiSelect = false;
                        private InteractorUtils.ButtonSettings slowNavigationButtonsSettings = new InteractorUtils.ButtonSettings("nav", "");
                        private bool withFastNavigationButtons = true;
                        private bool occupyFreeSpace = false;
                        private InteractorUtils.ButtonSettings fastNavigationButtonsSettings = new InteractorUtils.ButtonSettings("fast-nav", "");
                        private bool withCancelButton = true;
                        private InteractorUtils.ButtonSettings cancelButtonSettings = new InteractorUtils.ButtonSettings("cancel", "Cancel");
                        private bool withContinueButton = true;
                        private InteractorUtils.ButtonSettings continueButtonSettings = new InteractorUtils.ButtonSettings("continue", "Continue");
                        private bool aboveInteractiveInterface = false;
                        private Color labelContentColor = Color.black;

                        private void OnGUI()
                        {
                            GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                            GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                            titleContent = new GUIContent("Gab Tab - Creating a new Simple String List Interactor");

                            Rect contentRect = EditorGUILayout.BeginVertical();

                            // Rendering starting properties (basic configuration)
                            EditorGUILayout.LabelField("This wizard will create a simple string list interactor with 3 slots.", longLabelStyle);
                            simpleStringListInteractorName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", simpleStringListInteractorName), "New Simple Strings Interactor");
                            multiSelect = EditorGUILayout.ToggleLeft("Multiple items will be selected", multiSelect);
                            withFastNavigationButtons = EditorGUILayout.ToggleLeft("Add page navigation buttons", withFastNavigationButtons);
                            if (!withFastNavigationButtons)
                            {
                                EditorGUILayout.BeginVertical(indentedStyle);
                                occupyFreeSpace = EditorGUILayout.ToggleLeft("The list options will fill the space left", occupyFreeSpace);
                                EditorGUILayout.EndVertical();
                            }
                            if (multiSelect)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                withContinueButton = EditorGUILayout.ToggleLeft("Add a Continue button", true);
                                EditorGUI.EndDisabledGroup();
                            }
                            else
                            {
                                withContinueButton = EditorGUILayout.ToggleLeft("Add a Continue button", withContinueButton);
                            }
                            withCancelButton = EditorGUILayout.ToggleLeft("Add a Cancel button", withCancelButton);
                            EditorGUI.BeginDisabledGroup(withContinueButton || withCancelButton);
                            aboveInteractiveInterface = EditorGUILayout.ToggleLeft("Put this interactor above the whole interactive interface", aboveInteractiveInterface || withContinueButton || withCancelButton);
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.BeginDisabledGroup(aboveInteractiveInterface);
                            withBackground = EditorGUILayout.ToggleLeft("Add Background", withBackground || aboveInteractiveInterface);
                            EditorGUI.EndDisabledGroup();
                            if (withBackground)
                            {
                                EditorGUILayout.BeginVertical(indentedStyle);
                                backgroundTint = EditorGUILayout.ColorField("Background tint", backgroundTint);
                                EditorGUILayout.EndVertical();
                            }
                            labelContentColor = EditorGUILayout.ColorField("Label text color", labelContentColor);
                            // Rendering color properties
                            if (withFastNavigationButtons)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Settings for item navigation buttons");
                                InteractorUtils.ButtonSettingsGUI(slowNavigationButtonsSettings, "item", "Item", new GUIStyle());
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Settings for page navigation buttons");
                                InteractorUtils.ButtonSettingsGUI(fastNavigationButtonsSettings, "page", "Page", new GUIStyle());
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.EndHorizontal();
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Settings for item navigation buttons");
                                InteractorUtils.ButtonSettingsGUI(slowNavigationButtonsSettings, "item", "Item", new GUIStyle());
                            }
                            if (withCancelButton && withContinueButton)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Settings for Continue button");
                                InteractorUtils.ButtonSettingsGUI(continueButtonSettings, "continue", "Continue", new GUIStyle());
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Settings for Cancel button");
                                InteractorUtils.ButtonSettingsGUI(continueButtonSettings, "cancel", "Cancel", new GUIStyle());
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.EndHorizontal();
                            }
                            else if (withCancelButton)
                            {
                                EditorGUILayout.LabelField("Settings for Cancel button");
                                InteractorUtils.ButtonSettingsGUI(continueButtonSettings, "cancel", "Cancel", new GUIStyle());
                            }
                            else if (withContinueButton)
                            {
                                EditorGUILayout.LabelField("Settings for Continue button");
                                InteractorUtils.ButtonSettingsGUI(continueButtonSettings, "continue", "Continue", new GUIStyle());
                            }

                            if (GUILayout.Button("Create Simple String List Interactor")) Execute();

                            EditorGUILayout.EndVertical();

                            if (contentRect.size != Vector2.zero)
                            {
                                minSize = contentRect.max + contentRect.min;
                                maxSize = minSize;
                            }
                        }

                        // In contrast to the method in InteractorUtils, this one takes into account the
                        // size (one or two floors) and the position of this interactor being created.
                        private GameObject AddBaseInteractorLayout(float offset, float controlHeight, float interactiveInterfaceHeight)
                        {
                            GameObject interactorObject = new GameObject(simpleStringListInteractorName);
                            interactorObject.transform.parent = selectedTransform.transform;
                            Layout.AddComponent<Hideable>(interactorObject);
                            Image interactorImage = Layout.AddComponent<Image>(interactorObject);
                            int floors = (withCancelButton || withContinueButton) ? 2 : 1;

                            if (withBackground)
                            {
                                interactorImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                                interactorImage.type = Image.Type.Sliced;
                                interactorImage.color = backgroundTint;
                                interactorImage.fillCenter = true;
                                interactorImage.enabled = true;
                            }
                            else
                            {
                                interactorImage.enabled = false;
                            }
                            Hideable hideable = Layout.AddComponent<Hideable>(interactorObject);
                            hideable.Hidden = false;

                            RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                            Rect canvasRect = selectedTransform.transform.parent.GetComponent<RectTransform>().rect;
                            interactorRectTransformComponent.pivot = new Vector2(0.5f, 0);
                            if (aboveInteractiveInterface)
                            {
                                float currentInteractorHeight = offset * (floors + 1) + controlHeight * floors;
                                interactorRectTransformComponent.anchorMin = new Vector2(0, 1f);
                                interactorRectTransformComponent.anchorMax = new Vector2(1f, 1f + (currentInteractorHeight / interactiveInterfaceHeight));
                                interactorRectTransformComponent.offsetMin = Vector2.zero;
                                interactorRectTransformComponent.offsetMax = Vector2.zero;
                            }
                            else
                            {
                                interactorRectTransformComponent.anchorMin = Vector2.zero;
                                interactorRectTransformComponent.anchorMax = new Vector2(1f, 0.3f);
                                interactorRectTransformComponent.offsetMin = Vector2.one * offset;
                                interactorRectTransformComponent.offsetMax = new Vector2(-offset, 0);
                            }
                            return interactorObject;
                        }

                        private GameObject[] AddOptionLabels(Transform parent, float buttonsOffset, float controlHeight, bool fillSpace)
                        {
                            // TODO
                            // using: labelContentColor
                            return new GameObject[0];
                        }

                        private void AddNavigationButtons(Transform parent, float buttonsOffset, float controlHeight, bool fillSpace, out Button prevButton, out Button nextButton)
                        {
                            // TODO
                            // using: slowNavigationButtonsSettings
                            prevButton = null;
                            nextButton = null;
                        }

                        private void AddFastNavigationButtons(Transform parent, float buttonsOffset, float controlHeight, out Button prevPageButton, out Button nextPageButton)
                        {
                            // TODO
                            // using: fastNavigationButtonsSettings
                            prevPageButton = null;
                            nextPageButton = null;
                        }

                        private Button AddCancelButton(Transform parent, float buttonsOffset, float controlHeight)
                        {
                            // TODO
                            // using: cancelButtonSettings
                            return null;
                        }

                        private Button AddContinueButton(Transform parent, float buttonsOffset, float controlHeight)
                        {
                            // TODO
                            // using: continueButtonSettings
                            GameObject buttonObject = new GameObject(continueButtonSettings.key);
                            buttonObject.transform.parent = parent;
                            float buttonWidth = (parent.GetComponent<RectTransform>().rect.width - 5 * buttonsOffset) / 4;
                            float buttonHeight = controlHeight;
                            int position = (withCancelButton) ? 2 : 3;
                            RectTransform rectTransformComponent = Layout.AddComponent<RectTransform>(buttonObject);
                            rectTransformComponent.pivot = Vector2.zero;
                            rectTransformComponent.anchorMin = Vector2.zero;
                            rectTransformComponent.anchorMax = Vector2.zero;
                            rectTransformComponent.offsetMin = new Vector2(position * (buttonsOffset + buttonWidth), buttonsOffset);
                            rectTransformComponent.offsetMax = rectTransformComponent.offsetMin;
                            rectTransformComponent.sizeDelta = new Vector2(buttonWidth, buttonHeight);
                            Image buttonImageComponent = Layout.AddComponent<Image>(buttonObject);
                            buttonImageComponent.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                            buttonImageComponent.type = Image.Type.Sliced;
                            buttonImageComponent.fillCenter = true;
                            Button buttonComponent = Layout.AddComponent<Button>(buttonObject);
                            buttonComponent.colors = continueButtonSettings.colors;
                            buttonComponent.targetGraphic = buttonImageComponent;
                            GameObject textObject = new GameObject("Text");
                            textObject.transform.parent = buttonObject.transform;
                            Text textComponent = Layout.AddComponent<Text>(textObject);
                            textComponent.text = continueButtonSettings.caption;
                            textComponent.fontSize = (int)(buttonHeight / 2);
                            textComponent.alignment = TextAnchor.MiddleCenter;
                            textComponent.color = continueButtonSettings.textColor;
                            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
                            textRectTransform.pivot = Vector2.one / 2f;
                            textRectTransform.anchorMin = Vector2.zero;
                            textRectTransform.anchorMax = Vector2.one;
                            textRectTransform.offsetMin = Vector2.zero;
                            textRectTransform.offsetMax = Vector2.zero;
                            return null;
                        }

                        private void Execute()
                        {
                            Rect canvasRect = selectedTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
                            Rect interactiveInterfaceRect = selectedTransform.GetComponent<RectTransform>().rect;
                            float interactiveInterfaceHeight = interactiveInterfaceRect.height;
                            float interactiveInterfaceWidth = interactiveInterfaceRect.width;
                            float interactorOffset = Values.Max(canvasRect.height, interactiveInterfaceWidth) * 0.01f;
                            float standardInteractorHeight = interactiveInterfaceHeight * 0.3f;
                            float standardControlHeight = standardInteractorHeight - 2 * interactorOffset;

                            GameObject listInteractorObject = AddBaseInteractorLayout(interactorOffset, standardControlHeight, interactiveInterfaceHeight);
                            Button continueButton = null;
                            Button cancelButton = null;
                            Button prevButton = null, nextButton = null;
                            Button prevPageButton = null, nextPageButton = null;
                            AddNavigationButtons(listInteractorObject.transform, interactorOffset, standardControlHeight, occupyFreeSpace && !withFastNavigationButtons, out prevButton, out nextButton);
                            if (withFastNavigationButtons) AddFastNavigationButtons(listInteractorObject.transform, interactorOffset, standardControlHeight, out prevPageButton, out nextPageButton);
                            if (withContinueButton) continueButton = AddContinueButton(listInteractorObject.transform, interactorOffset, standardControlHeight);
                            if (withCancelButton) cancelButton = AddCancelButton(listInteractorObject.transform, interactorOffset, standardControlHeight);
                            GameObject[] itemDisplays = AddOptionLabels(listInteractorObject.transform, interactorOffset, standardControlHeight, occupyFreeSpace && !withFastNavigationButtons);

                            /**
                             * Annoying-to-configure properties are being set here (other ones, which belong
                             * exclusively to the simple-string subclass, can be edited later -as normal- in
                             * the inspector) for the to-create interactor.
                             */
                            SimpleStringListInteractor listInteractorComponent = Layout.AddComponent<SimpleStringListInteractor>(listInteractorObject, new Dictionary<string, object>()
                            {
                                { "multiSelect", multiSelect },
                                { "continueButton", continueButton },
                                { "cancelButton", cancelButton },
                                { "nextButton", nextButton },
                                { "prevButton", prevButton },
                                { "nextPageButton", nextPageButton },
                                { "prevPageButton", prevPageButton },
                                { "itemDisplays", itemDisplays }
                            });
                        }
                    }

                    /// <summary>
                    ///   This method is used in the menu action: GameObject > Gab Tab > Interactive Interface > Create Simple String List Interactor.
                    ///   It creates a <see cref="TextInteractor"/>, with their inner buttons, in the scene.
                    /// </summary>
                    [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Simple String List Interactor", false, 11)]
                    public static void CreateSimpleStringListInteractor()
                    {
                        CreateSimpleStringListInteractorWindow window = ScriptableObject.CreateInstance<CreateSimpleStringListInteractorWindow>();
                        window.maxSize = new Vector2(600, 176);
                        window.minSize = window.maxSize;
                        window.selectedTransform = Selection.activeTransform;
                        window.ShowUtility();
                    }

                    /// <summary>
                    ///   Validates the menu item: GameObject > Gab Tab > Interactive Interface > Create Simple String List Interactor.
                    ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                    /// </summary>
                    [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Simple String List Interactor", true)]
                    public static bool CanCreateSimpleStringListtInteractor()
                    {
                        return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                    }
                }
            }
        }
    }
}
