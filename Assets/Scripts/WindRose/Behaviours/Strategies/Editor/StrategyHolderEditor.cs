using System.Linq;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Strategies
        {
            [CustomEditor(typeof(StrategyHolder))]
            [CanEditMultipleObjects]
            public class StrategyHolderEditor : Editor
            {
                SerializedProperty strategy;

                protected virtual void OnEnable()
                {
                    strategy = serializedObject.FindProperty("strategy");
                }

                public override void OnInspectorGUI()
                {
                    serializedObject.Update();

                    StrategyHolder underlyingObject = (serializedObject.targetObject as StrategyHolder);
                    Strategy[] strategies = underlyingObject.GetComponents<Strategy>();
                    GUIContent[] strategyNames = (from strategy in strategies select new GUIContent(strategy.GetType().Name)).ToArray();

                    int index = ArrayUtility.IndexOf(strategies, strategy.objectReferenceValue as Strategy);
                    index = EditorGUILayout.Popup(new GUIContent("Main Strategy"), index, strategyNames);
                    strategy.objectReferenceValue = index >= 0 ? strategies[index] : null;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
