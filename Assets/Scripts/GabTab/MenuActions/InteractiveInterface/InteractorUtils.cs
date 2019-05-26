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

            /// <summary>
            ///   Utility class holding features common to almost all interactors.
            /// </summary>
            static class InteractorUtils
            {
                /// <summary>
                ///   This structure holds the data for the buttons.
                /// </summary>
                public class ButtonSettings
                {
                    public string key = "";
                    public string caption = "Button";
                    public ColorBlock colors;
                    public Color textColor = Color.black;

                    public ButtonSettings(string key, string caption)
                    {
                        this.key = key;
                        this.caption = caption;
                        colors = DefaultColors();
                    }
                }

                /// <summary>
                ///   Returns the default colors to be used in any color transition.
                ///   These colors are just a suggestion and can be changed.
                /// </summary>
                /// <returns>A <see cref="ColorBlock"/> with default colors.</returns>
                public static ColorBlock DefaultColors()
                {
                    ColorBlock colors = new ColorBlock();
                    colors.normalColor = new Color32(255, 255, 255, 255);
                    colors.highlightedColor = new Color32(245, 245, 245, 255);
                    colors.pressedColor = new Color32(200, 200, 200, 255);
                    colors.disabledColor = new Color32(200, 200, 200, 255);
                    colors.fadeDuration = 0.1f;
                    colors.colorMultiplier = 1f;
                    return colors;
                }

                /// <summary>
                ///   Creates a background that uses to be common to most interactors.
                ///   The background may be visible or invisible, but it is always
                ///     present in at least specific kind of interactors (like text
                ///     or buttons).
                /// </summary>
                /// <param name="parent">The interface the new object will be attached to</param>
                /// <param name="objectName">The name of the new object</param>
                /// <param name="addBackground">Whether the background will be set, or empty</param>
                /// <param name="backgroundTint">The tint to apply in the background</param>
                public static GameObject AddBaseInteractorLayout(InteractiveInterface parent, string objectName, bool addBackground, Color backgroundTint)
                {
                    GameObject interactorObject = new GameObject(objectName);
                    interactorObject.transform.parent = parent.transform;
                    Layout.AddComponent<Hideable>(interactorObject);
                    Image interactorImage = Layout.AddComponent<Image>(interactorObject);

                    if (addBackground)
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

                    RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                    Rect canvasRect = parent.transform.parent.GetComponent<RectTransform>().rect;
                    float interactorOffset = Values.Max(canvasRect.height, canvasRect.width) * 0.01f;
                    interactorRectTransformComponent.pivot = new Vector2(0.5f, 0);
                    interactorRectTransformComponent.anchorMin = Vector2.zero;
                    interactorRectTransformComponent.anchorMax = new Vector2(1f, 0.3f);
                    interactorRectTransformComponent.offsetMin = Vector2.one * interactorOffset;
                    interactorRectTransformComponent.offsetMax = new Vector2(-interactorOffset, 0);

                    Hideable hideable = Layout.AddComponent<Hideable>(interactorObject);
                    hideable.Hidden = false;

                    return interactorObject;
                }

                /// <summary>
                ///   Generates the UI to change a specific color set.
                /// </summary>
                /// <param name="source">The input colors</param>
                /// <returns>The new colors</returns>
                public static ColorBlock ColorsGUI(ColorBlock source)
                {
                    ColorBlock colors = new ColorBlock();
                    colors.normalColor = EditorGUILayout.ColorField("Normal color", source.normalColor);
                    colors.highlightedColor = EditorGUILayout.ColorField("Highlighted color", source.highlightedColor);
                    colors.pressedColor = EditorGUILayout.ColorField("Pressed color", source.pressedColor);
                    colors.disabledColor = EditorGUILayout.ColorField("Disabled color", source.disabledColor);
                    colors.fadeDuration = source.fadeDuration;
                    colors.colorMultiplier = source.colorMultiplier;
                    return colors;
                }

                /// <summary>
                ///   Generates the UI to change a specific indexed button setting.
                /// </summary>
                /// <param name="index">The button index (intended to be used on a multi-button setting)</param>
                /// <param name="settings">The button settings object being affected</param>
                /// <param name="style">Style to apply to this object's UI group</param>
                public static void ButtonSettingsGUI(int index, ButtonSettings settings, GUIStyle style)
                {
                    ButtonSettingsGUI(settings, "button-" + index, "Button " + index, style);
                }

                /// <summary>
                ///   Generates the UI to change a specific button setting.
                /// </summary>
                /// <param name="settings">The button settings object being affected</param>
                /// <param name="defaultKey">The default key for the button, if blank</param>
                /// <param name="defaultCaption">The default caption for the button, if blank</param>
                /// <param name="style">Style to apply to this object's UI group</param>
                public static void ButtonSettingsGUI(ButtonSettings settings, string defaultKey, string defaultCaption, GUIStyle style)
                {
                    EditorGUILayout.BeginVertical();
                    settings.key = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Button key", settings.key), defaultKey);
                    EditorGUILayout.BeginVertical(style);
                    settings.caption = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Caption", settings.caption), defaultCaption);
                    settings.colors = ColorsGUI(settings.colors);
                    settings.textColor = EditorGUILayout.ColorField("Text color", settings.textColor);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                /// <summary>
                ///   Adds a button at the bottom of the specified parent, and at a specific position.
                ///   The position is calculated given an offset between buttons (and boundaries) and
                ///     the specified number of expected elements.
                /// </summary>
                /// <param name="parent">The parent under which the button will be located</param>
                /// <param name="position">The 0-based position of the button</param>
                /// <param name="expectedElements">The number of expected button positions</param>
                /// <param name="buttonsOffset">The buttons' offset/margin</param>
                /// <param name="buttonHeight">The buttons' height</param>
                /// <param name="settings">The current button's settings</param>
                /// <returns>The button being created, or <c>null</c> if arguments are negative or somehow inconsistent</returns>
                public static Button AddButtonAtPosition(RectTransform parent, int position, int expectedElements, float buttonsOffset, float buttonHeight, ButtonSettings settings)
                {
                    if (position < 0 || expectedElements < 1 || position >= expectedElements || buttonHeight <= 0 || buttonsOffset < 0)
                    {
                        return null;
                    }
                    float buttonWidth = (parent.rect.width - (expectedElements + 1) * buttonsOffset) / expectedElements;
                    if (buttonWidth < 0)
                    {
                        return null;
                    }

                    return AddButton(parent, new Vector2(position * (buttonsOffset + buttonWidth) + buttonsOffset, buttonsOffset), new Vector2(buttonWidth, buttonHeight), settings);
                }

                /// <summary>
                ///   Creates a button, inside a parent, using specific coordinates (based on bottom-left) and size.
                /// </summary>
                /// <param name="parent">The parent under which the button will be located</param>
                /// <param name="position">The button's from-top-bottom position</param>
                /// <param name="size">The button's size</param>
                /// <param name="settings">The button's settings</param>
                /// <returns>The button being created, or <c>null</c> if arguments are negative or somehow inconsistent</returns>
                public static Button AddButton(RectTransform parent, Vector2 position, Vector2 size, ButtonSettings settings)
                {
                    if (size.x <= 0 || size.y <= 0 || position.x < 0 || position.y < 0)
                    {
                        return null;
                    }
                    GameObject buttonObject = new GameObject(settings.key);
                    buttonObject.transform.parent = parent;
                    RectTransform rectTransformComponent = Layout.AddComponent<RectTransform>(buttonObject);
                    rectTransformComponent.pivot = Vector2.zero;
                    rectTransformComponent.anchorMin = Vector2.zero;
                    rectTransformComponent.anchorMax = Vector2.zero;
                    rectTransformComponent.offsetMin = position;
                    rectTransformComponent.offsetMax = position;
                    rectTransformComponent.sizeDelta = size;
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
                    textComponent.fontSize = (int)(size.y / 2);
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
            }
        }
    }
}
