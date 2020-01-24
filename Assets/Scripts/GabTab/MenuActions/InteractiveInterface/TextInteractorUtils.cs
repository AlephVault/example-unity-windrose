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
            using Support.Utils;
            using GabTab.Behaviours;
            using GabTab.Behaviours.Interactors;

            /// <summary>
            ///   Menu actions to create a text interactor
            ///     inside an <see cref="InteractiveInterface"/>.
            /// </summary>
            public static class TextInteractorUtils
            {
                private class CreateTextInteractorWindow : EditorWindow
                {
                    private string textInteractorName = "New Text Interactor";
                    private bool withBackground = false;
                    private Color backgroundTint = Color.white;
                    public Transform selectedTransform = null;
                    private InteractorUtils.ButtonSettings continueButton = new InteractorUtils.ButtonSettings("ok", "OK");
                    private InteractorUtils.ButtonSettings cancelButton = new InteractorUtils.ButtonSettings("cancel", "Cancel");
                    private bool withCancelButton = false;
                    private ColorBlock inputSettings = MenuActionUtils.DefaultColors();
                    private Color inputContentColor = Color.black;
                    private Color inputPlaceholderColor = new Color(7 / 16f, 7 / 16f, 7 / 16f);

                    private void AllButtonsSettingsGUI(GUIStyle style)
                    {
                        withCancelButton = EditorGUILayout.ToggleLeft("Add a 'Cancel' button", withCancelButton);
                        InteractorUtils.ButtonSettingsGUI(0, continueButton, style);
                        if (withCancelButton)
                        {
                            InteractorUtils.ButtonSettingsGUI(1, cancelButton, style);
                        }
                    }

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        titleContent = new GUIContent("Gab Tab - Creating a new Text Interactor");

                        Rect contentRect = EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("This wizard will create an interactor with a text input field, and one or two buttons.", longLabelStyle);

                        textInteractorName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", textInteractorName), "New Text Interactor");

                        AllButtonsSettingsGUI(indentedStyle);
                        withBackground = EditorGUILayout.ToggleLeft("Add background", withBackground);
                        if (withBackground)
                        {
                            EditorGUILayout.BeginVertical(indentedStyle);
                            backgroundTint = EditorGUILayout.ColorField("Background tint", backgroundTint);
                            EditorGUILayout.EndVertical();
                        }
                        inputSettings = InteractorUtils.ColorsGUI(inputSettings);
                        inputContentColor = EditorGUILayout.ColorField("Input content color", inputContentColor);
                        inputPlaceholderColor = EditorGUILayout.ColorField("Input placeholder color", inputPlaceholderColor);

                        if (GUILayout.Button("Create Text Interactor"))
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

                    private Button AddButton(int index, RectTransform parent, InteractorUtils.ButtonSettings settings, float buttonsOffset, Rect interactorRect)
                    {
                        return InteractorUtils.AddButtonAtPosition(parent, index, 4, buttonsOffset, interactorRect.height - 2 * buttonsOffset, settings);
                    }

                    private Text AddRectTransformAndTextComponents(GameObject newObject, float buttonsOffset)
                    {
                        RectTransform childTextRectTransformComponent = Layout.AddComponent<RectTransform>(newObject);
                        childTextRectTransformComponent.pivot = Vector2.one * 0.5f;
                        childTextRectTransformComponent.anchorMin = Vector2.zero;
                        childTextRectTransformComponent.anchorMax = Vector2.one;
                        childTextRectTransformComponent.offsetMin = new Vector2(buttonsOffset * 1.5f, buttonsOffset * 0.75f);
                        childTextRectTransformComponent.offsetMax = new Vector2(buttonsOffset * -1.5f, -buttonsOffset);
                        Text childTextComponent = Layout.AddComponent<Text>(newObject);
                        childTextComponent.alignment = TextAnchor.UpperLeft;
                        childTextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                        childTextComponent.verticalOverflow = VerticalWrapMode.Truncate;
                        childTextComponent.fontSize = (int)(childTextRectTransformComponent.rect.height / 1.2f);
                        return childTextComponent;
                    }

                    private InputField AddInputField(RectTransform parent, InteractorUtils.ButtonSettings settings, float buttonsOffset, Rect interactorRect, int size)
                    {
                        GameObject textInputObject = new GameObject("Content");
                        textInputObject.transform.parent = parent;
                        RectTransform textInputRectTransformComponent = Layout.AddComponent<RectTransform>(textInputObject);
                        float buttonWidth = (interactorRect.width - 5 * buttonsOffset) / 4;
                        float buttonHeight = (interactorRect.height - 2 * buttonsOffset);
                        textInputRectTransformComponent.pivot = Vector2.zero;
                        textInputRectTransformComponent.anchorMin = Vector2.zero;
                        textInputRectTransformComponent.anchorMax = Vector2.zero;
                        textInputRectTransformComponent.offsetMin = new Vector2(buttonsOffset, buttonsOffset);
                        textInputRectTransformComponent.offsetMax = textInputRectTransformComponent.offsetMin;
                        textInputRectTransformComponent.sizeDelta = new Vector2(buttonWidth * size + buttonsOffset * (size - 1), buttonHeight);
                        Image textImageComponent = Layout.AddComponent<Image>(textInputObject);
                        textImageComponent.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                        textImageComponent.type = Image.Type.Sliced;
                        textImageComponent.fillCenter = true;
                        GameObject childTextObject = new GameObject("Text");
                        childTextObject.transform.parent = textInputObject.transform;
                        Text childTextComponent = AddRectTransformAndTextComponents(childTextObject, buttonsOffset);
                        childTextComponent.supportRichText = false;
                        GameObject childPlaceholderObject = new GameObject("Placeholder");
                        childPlaceholderObject.transform.parent = textInputObject.transform;
                        Text childPlaceholderTextComponent = AddRectTransformAndTextComponents(childPlaceholderObject, buttonsOffset);
                        childPlaceholderTextComponent.color = inputPlaceholderColor;
                        childPlaceholderTextComponent.text = "Enter text...";
                        childPlaceholderTextComponent.fontStyle = FontStyle.BoldAndItalic;
                        InputField inputFieldComponent = Layout.AddComponent<InputField>(textInputObject);
                        inputFieldComponent.placeholder = childPlaceholderTextComponent;
                        inputFieldComponent.textComponent = childTextComponent;
                        inputFieldComponent.transition = Selectable.Transition.ColorTint;
                        inputFieldComponent.colors = inputSettings;
                        return inputFieldComponent;
                    }

                    private void Execute()
                    {
                        Rect canvasRect = selectedTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
                        float buttonsOffset = Values.Max(canvasRect.height, canvasRect.width) * 0.01f;

                        GameObject interactorObject = InteractorUtils.AddBaseInteractorLayout(selectedTransform.GetComponent<InteractiveInterface>(), textInteractorName, withBackground, backgroundTint);
                        Rect interactorRect = interactorObject.GetComponent<RectTransform>().rect;
                        RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                        Button continueButtonComponent = AddButton(withCancelButton ? 2 : 3, interactorRectTransformComponent, continueButton, buttonsOffset, interactorRect);
                        Button cancelButtonComponent = null;
                        if (withCancelButton)
                        {
                            cancelButtonComponent = AddButton(3, interactorRectTransformComponent, cancelButton, buttonsOffset, interactorRect);
                        }
                        InputField textInput = AddInputField(interactorRectTransformComponent, cancelButton, buttonsOffset, interactorRect, withCancelButton ? 2 : 3);
                        Layout.AddComponent<TextInteractor>(interactorObject, new Dictionary<string, object>()
                        {
                            { "continueButton", continueButtonComponent },
                            { "cancelButton", cancelButtonComponent },
                            { "textInput", textInput }
                        });
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Gab Tab > Interactive Interface > Create Text Interactor.
                ///   It creates a <see cref="TextInteractor"/>, with their inner buttons, in the scene.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Text Interactor", false, 11)]
                public static void CreateTextInteractor()
                {
                    CreateTextInteractorWindow window = ScriptableObject.CreateInstance<CreateTextInteractorWindow>();
                    window.maxSize = new Vector2(400, 176);
                    window.minSize = window.maxSize;
                    window.selectedTransform = Selection.activeTransform;
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item: GameObject > Gab Tab > Interactive Interface > Create Text Interactor.
                ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Text Interactor", true)]
                public static bool CanCreateTextInteractor()
                {
                    return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                }
            }
        }
    }
}
