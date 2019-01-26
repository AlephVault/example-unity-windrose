using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Support.Utils;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Animations
        {
            using Types;

            /// <summary>
            ///   An animation set consist of 4 animation specs: one for each direction.
            ///   Intended for animations in orientable or moving objects.
            /// </summary>
            [CreateAssetMenu(fileName = "NewAnimationSet", menuName = "Wind Rose/Objects/Animation Set", order = 201)]
            public class AnimationSet : ScriptableObject
            {
                /// <summary>
                ///   Animation spec for the UP direction.
                /// </summary>
                [SerializeField]
                private AnimationSpec up;

                /// <summary>
                ///   Animation spec for the DOWN direction.
                /// </summary>
                [SerializeField]
                private AnimationSpec down;

                /// <summary>
                ///   Animation spec for the LEFT direction.
                /// </summary>
                [SerializeField]
                private AnimationSpec left;

                /// <summary>
                ///   Animation spec for the RIGHT direction.
                /// </summary>
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

                /// <summary>
                ///   Gets the animation spec for a given direction. This method is used internally from
                ///     other classes (e.g. orientable).
                /// </summary>
                /// <param name="direction">The desired direction</param>
                /// <returns>The animation to render</returns>
                public AnimationSpec GetForDirection(Direction direction)
                {
                    switch (direction)
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
}