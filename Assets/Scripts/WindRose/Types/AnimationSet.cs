using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Support.Utils;

namespace WindRose
{
    namespace Types
    {
        [CreateAssetMenu(fileName = "NewAnimationSet", menuName = "Wind Rose/Objects/Animation Set", order = 201)]
        public class AnimationSet : ScriptableObject
        {
            [SerializeField]
            private AnimationSpec up;
            [SerializeField]
            private AnimationSpec down;
            [SerializeField]
            private AnimationSpec left;
            [SerializeField]
            private AnimationSpec right;

#if UNITY_EDITOR
            [MenuItem("Assets/Create/Wind Rose/Objects/Animation Set (with specs)")]
            public static void CreateInstanceWithChildSpecs()
            {
                AnimationSet instance = ScriptableObject.CreateInstance<AnimationSet>();
                AnimationSpec instanceUp = ScriptableObject.CreateInstance<AnimationSpec>();
                AnimationSpec instanceDown = ScriptableObject.CreateInstance<AnimationSpec>();
                AnimationSpec instanceLeft = ScriptableObject.CreateInstance<AnimationSpec>();
                AnimationSpec instanceRight = ScriptableObject.CreateInstance<AnimationSpec>();
                Layout.SetObjectFieldValues(instance, new Dictionary<string, object>() {
                    { "up", instanceUp },
                    { "down", instanceDown },
                    { "left", instanceLeft },
                    { "right", instanceRight },
                });
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (path == "")
                {
                    path = "Assets";
                }
                if (!Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                AssetDatabase.CreateAsset(instanceUp, Path.Combine(path, "AnimationUpSpec.asset"));
                AssetDatabase.CreateAsset(instanceDown, Path.Combine(path, "AnimationDownSpec.asset"));
                AssetDatabase.CreateAsset(instanceLeft, Path.Combine(path, "AnimationLeftSpec.asset"));
                AssetDatabase.CreateAsset(instanceRight, Path.Combine(path, "AnimationRightSpec.asset"));
                AssetDatabase.CreateAsset(instance, Path.Combine(path, "AnimationSet.asset"));
            }
#endif

            public AnimationSpec GetForDirection(Direction direction)
            {
                switch(direction)
                {
                    case Direction.UP:
                        return up;
                    case Direction.DOWN:
                        return down;
                    case Direction.LEFT:
                        return left;
                    case Direction.RIGHT:
                        return right;
                    default:
                        // No default will run here,
                        //   but just for code completeness
                        return down;
                }
            }
        }
    }
}