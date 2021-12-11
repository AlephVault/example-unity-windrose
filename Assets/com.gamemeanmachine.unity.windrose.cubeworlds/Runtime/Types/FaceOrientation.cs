using System;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.CubeWorlds
{
    namespace Types
    {
        /// <summary>
        ///   The orientation of faces in cubes, applied to maps.
        /// </summary>
        public enum FaceOrientation
        {
            Front, Back, Down, Up, Left, Right
        }

        /// <summary>
        ///   Adds some methods to rotate maps in the specified orientation,
        ///   and to assemble a cube.
        /// </summary>
        public static class FaceOrientationMethods
        {
            /// <summary>
            ///   Determines the euler rotation (i.e. in (x, y, z) planes) of a
            ///   given orientation.
            /// </summary>
            /// <param name="orientation">The orientation to get the euler rotation for</param>
            /// <returns>The euler rotation</returns>
            /// <exception cref="ArgumentException">When the orientation is not among the expected values</exception>
            public static Quaternion Rotation(this FaceOrientation orientation)
            {
                switch (orientation)
                {
                    case FaceOrientation.Front:
                        return Quaternion.Euler(0, 0, 0);
                    case FaceOrientation.Back:
                        return Quaternion.Euler(0, 180, 0);
                    case FaceOrientation.Down:
                        return Quaternion.Euler(-90, 0, 0);
                    case FaceOrientation.Up:
                        return Quaternion.Euler(90, 0, 0);
                    case FaceOrientation.Left:
                        return Quaternion.Euler(0, 90, 0);
                    case FaceOrientation.Right:
                        return Quaternion.Euler(0, -90, 0);
                    default:
                        throw new ArgumentException($"Unexpected face orientation value: {orientation}");
                }
            }

            /// <summary>
            ///   Determines the cube-pivot (i.e. the corresponding pivot vertex, out of the in-cube 8 ones)
            ///   position of a given orientation.
            /// </summary>
            /// <param name="orientation">The orientation to get the pivot position for</param>
            /// <returns>The pivot position</returns>
            /// <exception cref="ArgumentException">When the orientation is not among the expected values</exception>
            public static Vector3 CubicAssemblyPosition(this FaceOrientation orientation)
            {
                switch (orientation)
                {
                    case FaceOrientation.Front:
                        return new Vector3(-1, -1, -1);
                    case FaceOrientation.Back:
                        return new Vector3(1, -1, 1);
                    case FaceOrientation.Down:
                    case FaceOrientation.Left:
                        return new Vector3(-1, -1, 1);
                    case FaceOrientation.Up:
                        return new Vector3(-1, 1, -1);
                    case FaceOrientation.Right:
                        return new Vector3(1, -1, -1);
                    default:
                        throw new ArgumentException($"Unexpected face orientation value: {orientation}");
                }
            }
        }
    }
}
