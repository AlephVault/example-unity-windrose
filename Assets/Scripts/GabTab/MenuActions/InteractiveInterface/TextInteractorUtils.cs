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
                    private Color inputTint = new Color(15 / 16f, 15 / 16f, 15 / 16f);
                    private Color inputContentColor = Color.black;
                    private Color inputPlaceholderColor = new Color(7 / 16f, 7 / 16f, 7 / 16f);

                    private void AllButtonsSettingsGUI(GUIStyle style)
                    {
                        withCancelButton = EditorGUILayout.ToggleLeft("Add a 'Cancel' button", withCancelButton);
                        InteractorUtils.ButtonsSettingsGUI(0, continueButton, style);
                        if (withCancelButton)
                        {
                            InteractorUtils.ButtonsSettingsGUI(1, cancelButton, style);
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
                        inputTint = EditorGUILayout.ColorField("Input tint", inputTint);
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
                        return buttonComponent;
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
                        textImageComponent.color = inputTint;
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
                        return inputFieldComponent;
                    }

                    private void Execute()
                    {
                        Rect canvasRect = selectedTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
                        float buttonsOffset = Values.Max(canvasRect.height, canvasRect.width) * 0.01f;

                        GameObject interactorObject = InteractorUtils.AddBaseInteractorLayout(selectedTransform.GetComponent<InteractiveInterface>(), textInteractorName, withBackground, backgroundTint);
                        Rect interactorRect = interactorObject.GetComponent<RectTransform>().rect;
                        RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                        Button continueButtonComponent = AddButton(0, interactorRectTransformComponent, continueButton, buttonsOffset, interactorRect);
                        Button cancelButtonComponent = null;
                        if (withCancelButton)
                        {
                            cancelButtonComponent = AddButton(1, interactorRectTransformComponent, cancelButton, buttonsOffset, interactorRect);
                        }
                        InputField textInput = AddInputField(interactorRectTransformComponent, cancelButton, buttonsOffset, interactorRect, withCancelButton ? 2 : 3);
                        TextInteractor buttonsInteractorComponent = Layout.AddComponent<TextInteractor>(interactorObject, new Dictionary<string, object>()
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
                public static void CreateInteractiveInterface()
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
                public static bool CanCreateInteractiveInterface()
                {
                    return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                }
            }
        }
    }
}
