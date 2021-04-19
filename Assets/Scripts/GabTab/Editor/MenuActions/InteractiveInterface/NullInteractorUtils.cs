using UnityEditor;
using UnityEngine;

namespace GabTab
{
    namespace MenuActions
    {
        namespace InteractiveInterface
        {
            using AlephVault.Unity.MenuActions.Utils;
            using Behaviours;
            using Behaviours.Interactors;

            /// <summary>
            ///   Menu actions to create a null interactor inside an <see cref="InteractiveInterface"/>.
            /// </summary>
            public static class NullInteractorUtils
            {
                private class CreateNullInteractorWindow : EditorWindow
                {
                    private string nullInteractorName = "New Null Interactor";
                    public Transform selectedTransform = null;

                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

                        titleContent = new GUIContent("Gab Tab - Creating a new Null Interactor");

                        EditorGUILayout.LabelField("This wizard will create an interactor that only prompts the text, expecting no user action.", longLabelStyle);

                        nullInteractorName = MenuActionUtils.EnsureNonEmpty(EditorGUILayout.TextField("Object name", nullInteractorName), "New Null Interactor");

                        if (GUILayout.Button("Create Null Interactor"))
                        {
                            Execute();
                        }
                    }

                    private void Execute()
                    {
                        GameObject nullInteractorObject = new GameObject(nullInteractorName);
                        nullInteractorObject.transform.parent = selectedTransform;
                        AlephVault.Unity.Layout.Utils.Behaviours.AddComponent<NullInteractor>(nullInteractorObject);
                        Undo.RegisterCreatedObjectUndo(nullInteractorObject, "Create Null Interactor");
                        Close();
                    }
                }

                /// <summary>
                ///   This method is used in the menu action: GameObject > Gab Tab > Interactive Interface > Create Null Interactor.
                ///   It creates a <see cref="NullInteractor"/> in the scene.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Null Interactor", false, 11)]
                public static void CreateNullInteractor()
                {
                    CreateNullInteractorWindow window = ScriptableObject.CreateInstance<CreateNullInteractorWindow>();
                    window.maxSize = new Vector2(522, 63);
                    window.minSize = window.maxSize;
                    window.selectedTransform = Selection.activeTransform;
                    window.ShowUtility();
                }

                /// <summary>
                ///   Validates the menu item: GameObject > Gab Tab > Interactive Interface > Create Null Interactor.
                ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                /// </summary>
                [MenuItem("GameObject/Gab Tab/Interactive Interface/Create Null Interactor", true)]
                public static bool CanCreateNullInteractor()
                {
                    return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                }
            }
        }
    }
}