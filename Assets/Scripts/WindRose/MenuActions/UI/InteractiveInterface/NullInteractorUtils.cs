using UnityEditor;
using UnityEngine;

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

                            titleContent = new GUIContent("Wind Rose - Creating a new Null Interactor");

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
                            Layout.AddComponent<NullInteractor>(nullInteractorObject);
                            Close();
                        }
                    }

                    /// <summary>
                    ///   This method is used in the menu action: GameObject > Wind Rose > UI > HUD > Interactive Interface > Create Null Interactor.
                    ///   It creates a <see cref="NullInteractor"/> in the scene.
                    /// </summary>
                    [MenuItem("GameObject/Wind Rose/UI/HUD/Interactive Interface/Create Null Interactor", false, 11)]
                    public static void CreateInteractiveInterface()
                    {
                        CreateNullInteractorWindow window = ScriptableObject.CreateInstance<CreateNullInteractorWindow>();
                        window.maxSize = new Vector2(522, 63);
                        window.minSize = window.maxSize;
                        window.selectedTransform = Selection.activeTransform;
                        window.ShowUtility();
                    }

                    /// <summary>
                    ///   Validates the menu item: GameObject > Wind Rose > UI > HUD > Interactive Interface > Create Null Interactor.
                    ///   It enables such menu option when an <see cref="InteractiveInterface"/> is selected in the scene hierarchy.
                    /// </summary>
                    [MenuItem("GameObject/Wind Rose/UI/HUD/Interactive Interface/Create Null Interactor", true)]
                    public static bool CanCreateInteractiveInterface()
                    {
                        return Selection.activeTransform != null && Selection.activeTransform.GetComponent<InteractiveInterface>();
                    }
                }
            }
        }
    }
}