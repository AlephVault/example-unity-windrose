﻿using System;
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

            /// <summary>
            ///   Loads the scene with the given path.
            ///   This is an asynchronous task that must be waited for.
            /// </summary>
            /// <returns>Whether the scene was loaded or not</returns>
            public async Task<Scene> Load(LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
            {
                if (string.IsNullOrEmpty(_path))
                {
                    return new Scene{};
                }
                else
                {
                    // To get a scene, loadSceneAsync will be called and will return
                    // an AsyncOperation. Such operations are queued, thus never
                    // running into race conditions. This will imply that this code
                    // will be stable regarding the scene count.
                    Scene scene = new Scene{};
                    AsyncOperation operation = SceneManager.LoadSceneAsync(_path, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
                    int index = SceneManager.sceneCount;
                    operation.completed += (op) =>
                    {
                        if (SceneManager.sceneCount != index)
                        {
                            scene = SceneManager.GetSceneAt(index);
                        }
                    };
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                    return scene;
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