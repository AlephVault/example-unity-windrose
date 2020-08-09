using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using GMM;
#if UNITY_EDITOR
using UnityEditor;
#endif
using GMM.Utils;

// Taken from https://gist.github.com/JohannesMP/ec7d3f0bcf167dab3d0d3bb480e0e07b

namespace GMM
{
    namespace Types
    {
        /// <summary>
        ///   Holds a reference to a scene. When retrieving it, the Path property
        ///   will hold the actual Scene.
        /// </summary>
        [Serializable]
        public sealed class SceneReference : ISerializationCallbackReceiver
        {
#if UNITY_EDITOR
            /// <summary>
            ///   The inner asset of the scene.
            /// </summary>
            [SerializeField]
            private SceneAsset _asset; // hidden by the drawer
#endif

            /// <summary>
            ///   The path of the scene.
            /// </summary>
            [SerializeField]
            private string _path; // hidden by the drawer

            /// <summary>
            ///   Returns the underlying path of the scene.
            /// </summary>
            public string Path => _path;

            // The inner scene.
            private Scene loadedScene;

            /// <summary>
            ///   Returns the underlying scene. The scene must
            ///   be successfully loaded before trying to get
            ///   the reference, or it will be null.
            /// </summary>
            public Scene LoadedScene => loadedScene;

            /// <summary>
            ///   Preloads the scene with the given path.
            ///   This process is asynchronous and should be waited for.
            /// </summary>
            /// <returns>Whether the scene was loaded or not</returns>
            public async Task<bool> Preload()
            {
                if (loadedScene != null)
                {
                    return true;
                }
                else if (string.IsNullOrEmpty(_path))
                {
                    return false;
                }
                else
                {
                    AsyncOperation operation = SceneManager.LoadSceneAsync(_path, LoadSceneMode.Additive);
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                    Scene scene = SceneManager.GetSceneByPath(_path);
                    if (scene != null && scene.IsValid())
                    {
                        loadedScene = scene;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            #region ISerializationCallbackReceiver Members

            public void OnAfterDeserialize()
            {
#if UNITY_EDITOR
                EditorApplication.delayCall += () => { _path = _asset == null ? string.Empty : AssetDatabase.GetAssetPath(_asset); };
#endif
            }

            public void OnBeforeSerialize()
            {
            }

            #endregion

            #region Nested type: SceneReferencePropertyDrawer

#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(SceneReference))]
            internal sealed class SceneReferencePropertyDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    var relative = property.FindPropertyRelative(nameof(_asset));

                    var content = EditorGUI.BeginProperty(position, label, relative);

                    EditorGUI.BeginChangeCheck();

                    var source = relative.objectReferenceValue;
                    var target = EditorGUI.ObjectField(position, content, source, typeof(SceneAsset), false);

                    if (EditorGUI.EndChangeCheck())
                        relative.objectReferenceValue = target;

                    EditorGUI.EndProperty();
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            }
#endif

            #endregion
        }
    }
}
