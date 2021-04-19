using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using AlephVault.Unity.Support.Utils;
using AlephVault.Unity.MenuActions.Utils;

namespace GabTab
{
    namespace MenuActions
    {
        namespace InteractiveInterface
        {
            using Behaviours;

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
                        colors = MenuActionUtils.DefaultColors();
                    }
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
                    AlephVault.Unity.Layout.Utils.Behaviours.AddComponent<Hideable>(interactorObject);
                    Image interactorImage = AlephVault.Unity.Layout.Utils.Behaviours.AddComponent<Image>(interactorObject);

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

                    Hideable hideable = AlephVault.Unity.Layout.Utils.Behaviours.AddComponent<Hideable>(interactorObject);
                    hideable.Hidden = false;

                    return interactorObject;
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
                    settings.colors = MenuActionUtils.ColorsGUI(settings.colors);
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
                /// <param name="fontSize">Optional font size. Will default to half the height</param>
                /// <returns>The button being created, or <c>null</c> if arguments are negative or somehow inconsistent</returns>
                public static Button AddButtonAtPosition(RectTransform parent, int position, int expectedElements, float buttonsOffset, float buttonHeight, ButtonSettings settings, int fontSize = 0)
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

                    return MenuActionUtils.AddButton(parent, new Vector2(position * (buttonsOffset + buttonWidth) + buttonsOffset, buttonsOffset), new Vector2(buttonWidth, buttonHeight), settings.caption, settings.key, settings.textColor, settings.colors, fontSize);
                }
            }
        }
    }
}
