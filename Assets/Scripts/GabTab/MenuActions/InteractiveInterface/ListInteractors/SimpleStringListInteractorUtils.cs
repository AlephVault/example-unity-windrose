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

                        private GameObject[] AddOptionLabels(RectTransform parent, float buttonsOffset, float controlHeight, bool fillSpace)
                        {
                            GameObject[] labels = new GameObject[3];
                            float leftX = (fillSpace ? 1 : 2) * (controlHeight + buttonsOffset);
                            float rightX = (fillSpace ? (2 * buttonsOffset + controlHeight) : (3 * buttonsOffset + 2 * controlHeight));
                            float y = (withContinueButton || withCancelButton) ? (2 * buttonsOffset + controlHeight) : buttonsOffset;
                            float innerWidth = parent.rect.width - leftX - rightX;
                            float labelWidth = innerWidth / 3 - buttonsOffset;
                            for(int index = 0; index < 3; index++)
                            {
                                InteractorUtils.ButtonSettings settings = new InteractorUtils.ButtonSettings("label-" + index, "");
                                Button button = MenuActionUtils.AddButton(parent, new Vector2(leftX + buttonsOffset + index * (buttonsOffset + labelWidth), y), new Vector2(labelWidth, controlHeight), settings.caption, settings.key, settings.textColor, settings.colors, (int)(controlHeight / 1.2));
                                button.transition = Selectable.Transition.None;
                                labels[index] = button.gameObject;
                            }
                            return labels;
                        }

                        private void AddNavigationButtons(RectTransform parent, float buttonsOffset, float controlHeight, bool fillSpace, out Button prevButton, out Button nextButton)
                        {
                            float y = (withContinueButton || withCancelButton) ? controlHeight + 2 * buttonsOffset : buttonsOffset;
                            float xPrev = fillSpace ? buttonsOffset : (2 * buttonsOffset + controlHeight);
                            float xNext = parent.rect.width - (fillSpace ? 1 : 2) * (buttonsOffset + controlHeight);
                            string key = slowNavigationButtonsSettings.key;
                            string caption = slowNavigationButtonsSettings.caption;
                            // set prev button settings
                            slowNavigationButtonsSettings.key = key + "-prev";
                            slowNavigationButtonsSettings.caption = "◀";
                            prevButton = MenuActionUtils.AddButton(parent, new Vector2(xPrev, y), Vector2.one * controlHeight, slowNavigationButtonsSettings.caption, slowNavigationButtonsSettings.key, slowNavigationButtonsSettings.textColor, slowNavigationButtonsSettings.colors, (int)controlHeight / 3);
                            // set next button settings
                            slowNavigationButtonsSettings.key = key + "-next";
                            slowNavigationButtonsSettings.caption = "▶";
                            nextButton = MenuActionUtils.AddButton(parent, new Vector2(xNext, y), Vector2.one * controlHeight, slowNavigationButtonsSettings.caption, slowNavigationButtonsSettings.key, slowNavigationButtonsSettings.textColor, slowNavigationButtonsSettings.colors, (int)controlHeight / 3);
                            // reset
                            slowNavigationButtonsSettings.key = key;
                            slowNavigationButtonsSettings.caption = caption;
                        }

                        private void AddFastNavigationButtons(RectTransform parent, float buttonsOffset, float controlHeight, out Button prevPageButton, out Button nextPageButton)
                        {
                            float y = (withContinueButton || withCancelButton) ? controlHeight + 2 * buttonsOffset : buttonsOffset;
                            float xPrev = buttonsOffset;
                            float xNext = parent.rect.width - buttonsOffset - controlHeight;
                            string key = fastNavigationButtonsSettings.key;
                            string caption = fastNavigationButtonsSettings.caption;
                            // set prev button settings
                            fastNavigationButtonsSettings.key = key + "-prev";
                            fastNavigationButtonsSettings.caption = "◀◀";
                            prevPageButton = MenuActionUtils.AddButton(parent, new Vector2(xPrev, y), Vector2.one * controlHeight, fastNavigationButtonsSettings.caption, fastNavigationButtonsSettings.key, fastNavigationButtonsSettings.textColor, fastNavigationButtonsSettings.colors, (int)controlHeight / 3);
                            // set next button settings
                            fastNavigationButtonsSettings.key = key + "-next";
                            fastNavigationButtonsSettings.caption = "▶▶";
                            nextPageButton = MenuActionUtils.AddButton(parent, new Vector2(xNext, y), Vector2.one * controlHeight, fastNavigationButtonsSettings.caption, fastNavigationButtonsSettings.key, fastNavigationButtonsSettings.textColor, fastNavigationButtonsSettings.colors, (int)controlHeight / 3);
                            // reset
                            fastNavigationButtonsSettings.key = key;
                            fastNavigationButtonsSettings.caption = caption;
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
                            RectTransform listInteractorRectTransform = listInteractorObject.GetComponent<RectTransform>();
                            Button continueButton = null;
                            Button cancelButton = null;
                            Button prevButton = null, nextButton = null;
                            Button prevPageButton = null, nextPageButton = null;
                            AddNavigationButtons(listInteractorRectTransform, interactorOffset, standardControlHeight, occupyFreeSpace && !withFastNavigationButtons, out prevButton, out nextButton);
                            if (withFastNavigationButtons) AddFastNavigationButtons(listInteractorRectTransform, interactorOffset, standardControlHeight, out prevPageButton, out nextPageButton);
                            if (withContinueButton) continueButton = InteractorUtils.AddButtonAtPosition(listInteractorRectTransform, withCancelButton ? 2 : 3, 4, interactorOffset, standardControlHeight, continueButtonSettings);
                            if (withCancelButton) cancelButton = InteractorUtils.AddButtonAtPosition(listInteractorRectTransform, 3, 4, interactorOffset, standardControlHeight, cancelButtonSettings);
                            GameObject[] itemDisplays = AddOptionLabels(listInteractorRectTransform, interactorOffset, standardControlHeight, occupyFreeSpace && !withFastNavigationButtons);

                            /**
                             * Annoying-to-configure properties are being set here (other ones, which belong
                             * exclusively to the simple-string subclass, can be edited later -as normal- in
                             * the inspector) for the to-create interactor.
                             */
                            Layout.AddComponent<SimpleStringListInteractor>(listInteractorObject, new Dictionary<string, object>()
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
                            Undo.RegisterCreatedObjectUndo(listInteractorObject, "Create Simple String List Interactor");
                            Close();
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
