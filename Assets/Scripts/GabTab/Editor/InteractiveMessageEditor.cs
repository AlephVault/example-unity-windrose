using UnityEditor;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

namespace GabTab
{
    namespace Behaviours
    {
        [CustomEditor(typeof(InteractiveMessage), true)]
        [CanEditMultipleObjects]
        public class ScrollRectEditor : Editor
        {
            SerializedProperty m_Content;

            protected virtual void OnEnable()
            {
                m_Content = serializedObject.FindProperty("messageContent");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(m_Content);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
