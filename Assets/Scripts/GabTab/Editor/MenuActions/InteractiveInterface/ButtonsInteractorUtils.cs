using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using AlephVault.Unity.Support.Utils;

namespace GabTab
{
    namespace MenuActions
    {
        namespace InteractiveInterface
        {
            using AlephVault.Unity.Layout.Utils;
            using AlephVault.Unity.MenuActions.Utils;
            using GabTab.Behaviours;
            using GabTab.Behaviours.Interactors;

            /// <summary>
            ///   Menu actions to create button sets (1, 2 or 3 buttons) to be used
            ///     inside an <see cref="InteractiveInterface"/>.
            /// </summary>
            public static class ButtonsInteractorUtils
            {
                private class CreateButtonsInteractorWindow : EditorWindow
                {
                    private string buttonsInteractorName = "New Buttons Interactor";
                    private InteractorUtils.ButtonSettings[] buttonsSettings = new InteractorUtils.ButtonSettings[] {
                        new InteractorUtils.ButtonSettings("button-1", "Button 1"),
                        new InteractorUtils.ButtonSettings("button-2", "Button 2"),
                        new InteractorUtils.ButtonSettings("button-3", "Button 3")
                    };
                    private int buttonsCount = 1;
                    private bool withBackground = false;
                    private Color backgroundTint = Color.white;
                    public Transform selectedTransform = null;

                    private void AllButtonsSettingsGUI(GUIStyle style)
                    {
                        buttonsCount = EditorGUILayout.IntSlider("Buttons #", buttonsCount, 1, 3);
                        for(int i = 0; i < buttonsCount; i++)
                        {
                            InteractorUtils.ButtonSettingsGUI(i, buttonsSettings[i], style);
                        }
                    }

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();
                        GUIStyle indentedStyle = MenuActionUtils.GetIndentedStyle();

                        titleContent = new GUIContent("Gab Tab - Creating a new Buttons Interactor");

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

                    private void AddButton(ButtonsInteractor.ButtonKeyDictionary buttons, int index, RectTransform parent, InteractorUtils.ButtonSettings settings, float buttonsOffset, Rect interactorRect)
                    {
                        Button button = InteractorUtils.AddButtonAtPosition(parent, 3 - index, 4, buttonsOffset, interactorRect.height - 2 * buttonsOffset, settings);
                        if (button)
                        {
                            buttons.Add(button, settings.key);
                        }
                    }

                    private void Execute()
                    {
                        Rect canvasRect = selectedTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
                        float buttonsOffset = Values.Max(canvasRect.height, canvasRect.width) * 0.01f;

                        GameObject interactorObject = InteractorUtils.AddBaseInteractorLayout(selectedTransform.GetComponent<InteractiveInterface>(), buttonsInteractorName, withBackground, backgroundTint);
                        Rect interactorRect = interactorObject.GetComponent<RectTransform>().rect;
                        ButtonsInteractor.ButtonKeyDictionary buttons = new ButtonsInteractor.ButtonKeyDictionary();
                        RectTransform interactorRectTransformComponent = interactorObject.GetComponent<RectTransform>();
                        Behaviours.AddComponent<ButtonsInteractor>(interactorObject, new Dictionary<string, object>()
                        {
                            { "buttons", buttons }
                        });
                        for (int index = 0; index < 3; index++)
                        {
                            if (buttonsCount > index) AddButton(buttons, index, interactorRectTransformComponent, buttonsSettings[index], buttonsOffset, interactorRect);
                        }
                        Undo.RegisterCreatedObjectUndo(interactorObject, "Create Buttons Interactor");
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Gab Tab > Interactive Interface > Create Buttons Interactor.
                ///   It creates a <see cref="ButtonsInteractor"/>, with their inner buttons, in the scene.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Buttons Interactor", false, 11)]
                public static void CreateButtonsInteractor()
                {
                    CreateButtonsInteractorWindow window = ScriptableObject.CreateInstance<CreateButtonsInteractorWindow>();
                    window.maxSize = new Vector2(400, 176);
                    window.minSize = window.maxSize;
                    window.selectedTransform = Selection.activeTransform;
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item: GameObject > Gab Tab > Interactive Interface > Create Buttons Interactor.
                ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Buttons Interactor", true)]
                public static bool CanCreateButtonsInteractor()
                {
                    return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                }
            }
        }
    }
}
