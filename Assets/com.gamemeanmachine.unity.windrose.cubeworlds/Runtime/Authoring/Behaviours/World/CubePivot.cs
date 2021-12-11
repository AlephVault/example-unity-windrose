using System;
using System.Collections.Generic;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.CubeWorlds.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using GameMeanMachine.Unity.WindRose.CubeWorlds.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.CubeWorlds
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace World
            {
                using AlephVault.Unity.Layout.Utils;
                
                /// <summary>
                ///   Works as a pivot to assemble one or more concentric cubes.
                ///   The involved cube faces will be considered, positioned, and
                ///   rotated according to their size (and also expected to be
                ///   cubic).
                /// </summary>
                public class CubePivot : MonoBehaviour
                {
                    /// <summary>
                    ///   <para>
                    ///     Specifies the faces this pivot will track & modify.
                    ///     Non-square maps (both in size and cell size) will
                    ///     be excluded from this list on startup.
                    ///   </para>
                    ///   <para>
                    ///     AVOID having two faces with the same orientation
                    ///     and size
                    ///   </para>
                    /// </summary>
                    [SerializeField]
                    private List<CubeFace> faces;

                    // A track over a square cube face.
                    private class CubeFaceTracking
                    {
                        public CubeFace Face;
                        public GameObject FacePivot;
                    }

                    // The list of tracks over square cube faces.
                    private List<CubeFaceTracking> trackedFaces;

                    // Keeps all the square maps in the tracked faces list.
                    private void Awake()
                    {
                        InitFaceTrackingEntries();
                    }

                    // Initializes the face tracking entries from the desired cube faces.
                    // It discards the non-square faces.
                    private void InitFaceTrackingEntries()
                    {
                        trackedFaces = new List<CubeFaceTracking>();
                        foreach (CubeFace face in faces)
                        {
                            Map map = Behaviours.RequireComponentInParent<Map>(face);
                            if (map.Width == map.Height && Mathf.Epsilon > Mathf.Abs(map.CellSize.x - map.CellSize.y))
                            {
                                // Force the face to be a sibling of this assembler pivot.
                                // This means that this pivot should either have no parent
                                // or have a Scope (WindRose) parent.
                                face.transform.parent = transform.parent;
                                // Getting the half-size of the map, and the cube-pivot-position.
                                float halfSize = map.CellSize.x * map.Width / 2;
                                // Create a pivot. Set the initial position and rotation appropriately.
                                GameObject facePivot = new GameObject($"{map} pivot");
                                facePivot.transform.parent = transform;
                                facePivot.transform.localRotation = face.FaceOrientation.Rotation();
                                facePivot.transform.localPosition = halfSize * face.FaceOrientation.CubicAssemblyPosition();
                                // Add the pivot to the tracked.
                                trackedFaces.Add(new CubeFaceTracking
                                {
                                    Face = face, FacePivot = facePivot
                                });
                            }
                            else
                            {
                                Debug.LogWarning($"The map {map} is not square - it will NOT be considered " +
                                                 $"as a cube face", gameObject);
                            }
                        }
                    }

                    
                    private void ConnectFaces()
                    {
                        
                    }
                    
                    /// <summary>
                    ///   Forces the transform of the related faces into specific
                    ///   settings, to follow what happens in the whole pivot.
                    ///   This method is a good candidate to be invoked on Update,
                    ///   if desired, in an additional behaviour.
                    /// </summary>
                    public void ForcePivotTransforms()
                    {
                        foreach (CubeFaceTracking tracked in trackedFaces)
                        {
                            Transform faceTransform = tracked.Face.transform;
                            Transform facePivotTransform = tracked.FacePivot.transform;
                            // Force it to be a sibling.
                            faceTransform.parent = transform.parent;
                            // Copy the ABSOLUTE position and rotation from the pivot
                            // to the face. Also, set the face's scale to this object's
                            // scale (pivots will have a default scale of Vector.one).
                            // Ideally, LOCAL position and rotation of the pivot will
                            // never change.
                            faceTransform.position = facePivotTransform.position;
                            faceTransform.rotation = facePivotTransform.rotation;
                            faceTransform.localScale = transform.localScale;
                        }
                    }
                }
            }
        }
    }
}