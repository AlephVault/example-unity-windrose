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
                        interactorImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
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
                ///   Generates the UI to change a specific button setting.
                /// </summary>
                /// <param name="index">The button index (intended to be used on a multi-button setting)</param>
                /// <param name="settings">The button settings object being affected</param>
                /// <param name="style">Style to apply to this object's UI group</param>
                public static void ButtonsSettingsGUI(int index, ButtonSettings settings, GUIStyle style)
                {
                    settings.key = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Button key", settings.key), "button-" + index);
                    EditorGUILayout.BeginVertical(style);
                    settings.caption = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Caption", settings.caption), "Button " + index);
                    if (settings.caption == "") settings.caption = "Button " + index;
                    settings.colors = ColorsGUI(settings.colors);
                    settings.textColor = EditorGUILayout.ColorField("Text color", settings.textColor);
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
