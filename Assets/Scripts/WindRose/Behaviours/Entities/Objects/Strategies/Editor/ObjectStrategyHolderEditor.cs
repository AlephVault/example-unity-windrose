using System.Linq;
using UnityEngine;
using UnityEditor;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace Strategies
            {
                [CustomEditor(typeof(ObjectStrategyHolder))]
                [CanEditMultipleObjects]
                public class ObjectStrategyHolderEditor : Editor
                {
                    SerializedProperty strategy;

                    protected virtual void OnEnable()
                    {
                        strategy = serializedObject.FindProperty("objectStrategy");
                    }

                    public override void OnInspectorGUI()
                    {
                        serializedObject.Update();

                        ObjectStrategyHolder underlyingObject = (serializedObject.targetObject as ObjectStrategyHolder);
                        ObjectStrategy[] strategies = underlyingObject.GetComponents<ObjectStrategy>();
                        GUIContent[] strategyNames = (from strategy in strategies select new GUIContent(strategy.GetType().Name)).ToArray();

                        int index = ArrayUtility.IndexOf(strategies, strategy.objectReferenceValue as ObjectStrategy);
                        index = EditorGUILayout.Popup(new GUIContent("Main Strategy"), index, strategyNames);
                        strategy.objectReferenceValue = index >= 0 ? strategies[index] : null;

                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
