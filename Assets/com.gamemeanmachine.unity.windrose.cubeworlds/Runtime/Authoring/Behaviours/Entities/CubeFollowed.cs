using System.Collections;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.CubeWorlds.Types;
using GameMeanMachine.Unity.WindRose.NeighbourTeleports.Authoring.Behaviours.Entities.Objects.Strategies;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.CubeWorlds
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace World
            {
                /// <summary>
                ///   Attached to neighbour-teleportable strategies, this
                ///   behavior will track the object appropriately by
                ///   using a chosen <see cref="CubeWatcher"/>. Typically,
                ///   this behaviour is added only to a client-local object.
                /// </summary>
                [RequireComponent(typeof(NeighbourTeleportObjectStrategy))]
                public class CubeFollowed : MonoBehaviour
                {
                    /// <summary>
                    ///   The default distance for this watcher, when the
                    ///   new map does not belong to a CubeLayout or does
                    ///   not have a CubeFace.
                    /// </summary>
                    public float DefaultDistance = 1f;

                    /// <summary>
                    ///   The default camera size for this watcher, when the
                    ///   new map does not belong to a CubeLayout or does not
                    ///   have a CubeFace.
                    /// </summary>
                    public float CameraSize = 12f;
                    
                    // The related map object.
                    private MapObject mapObject;

                    // The previous map this object was attached to.
                    private Map previousMap;

                    // The current rotation coroutine.
                    private Coroutine currentRotation;
                    
                    private void Awake()
                    {
                        mapObject = GetComponent<MapObject>();
                        mapObject.onAttached.AddListener(OnAttached);
                    }

                    private void OnDestroy()
                    {
                        mapObject.onAttached.RemoveListener(OnAttached);
                    }

                    private void OnAttached(Map newMap)
                    {
                        // Destroy any current movement.
                        if (currentRotation != null)
                        {
                            StopCoroutine(currentRotation);
                            currentRotation = null;
                        }
                        
                        CubeFace previousFace = previousMap ? previousMap.ObjectsLayer.GetComponent<CubeFace>() : null;
                        CubeFace newFace = newMap ? newMap.ObjectsLayer.GetComponent<CubeFace>() : null;
                        Transform previousMapParent = previousMap.transform.parent;
                        CubeLayout previousLayout = previousMap && previousMapParent ? previousMapParent.GetComponent<CubeLayout>() : null;
                        Transform newMapParent = newMap.transform.parent;
                        CubeLayout newLayout = newMap && newMapParent ? newMapParent.GetComponent<CubeLayout>() : null;

                        // First, the previous map must not be null / destroyed.
                        // Also, they must be different maps.
                        if (!previousMap || newMap == previousMap)
                        {
                            InstantFixCamera(newMap, newFace, newLayout);
                            return;
                        }

                        // Next, both the previous and new map must be CubeFace,
                        // within the same CubeLayout.
                        if (previousFace == null || newFace == null)
                        {
                            InstantFixCamera(newMap, newFace, newLayout);
                            return;
                        }

                        // Next, both faces must belong to the same parent cube.
                        if (newMap.transform.parent == null || previousMap.transform.parent == null)
                        {
                            InstantFixCamera(newMap, newFace, newLayout);
                            return;
                        }
                        if (previousLayout == null || newLayout == null || previousLayout != newLayout)
                        {
                            InstantFixCamera(newMap, newFace, newLayout);
                            return;
                        }
                        
                        // Next, both faces must be SURFACE.
                        if (previousFace.FaceType != FaceType.Surface || newFace.FaceType != FaceType.Surface)
                        {
                            InstantFixCamera(newMap, newFace, newLayout);
                            return;
                        }
                        
                        // Now, all the conditions are satisfied: CubeFaces inside
                        // the same CubeLayout, both different and both surface. The
                        // next thing to do is perform an animation.
                        StartCoroutine(CubeRotatingMovement());
                    }

                    private void InstantFixCamera(Map map, CubeFace cubeFace, CubeLayout cubeLayout)
                    {
                        if (!Watcher) return;
                        Transform mapTransform = map.transform;
                        Transform watcherTransform = Watcher.transform;

                        // 1. The parent of the entire watcher must be the CubeLayout.
                        //    Also, the local position and rotation must be the same.
                        watcherTransform.parent = mapTransform.parent;
                        watcherTransform.localPosition = mapTransform.localPosition;
                        watcherTransform.localRotation = mapTransform.localRotation;
                        
                        // 2. Fix the distance of the inner camera in the watcher.
                        if (cubeLayout)
                        {
                            Watcher.Distance = cubeLayout.FaceSize() / 2;
                        }
                        else if (cubeFace)
                        {
                            Watcher.Distance = map.Width * map.CellSize.x;
                        }
                        else
                        {
                            Watcher.Distance = DefaultDistance;
                        }
                        
                        // 3. Set the mode appropriately: Orthographic or Perspective.
                        Watcher.IsOrthographic = cubeFace != null;
                        
                        // 4. Set the camera size.
                        Watcher.Size = CameraSize;

                        // 5. Set the clip distance.
                        if (cubeLayout)
                        {
                            Watcher.ClipDistance = 2 * cubeLayout.FaceSize();
                        }
                        else if (cubeFace)
                        {
                            Watcher.ClipDistance = map.Width * map.CellSize.x;
                        }
                        else
                        {
                            Watcher.Distance = DefaultDistance + Mathf.Epsilon;
                        }

                        // 6. Set the inner camera's (x, y) to the object's position plus
                        //    the considered offset.
                        Watcher.CameraPosition = (Vector2)transform.localPosition + Offset;
                    }

                    private IEnumerator CubeRotatingMovement()
                    {
                        try
                        {
                            // First, fix the start position and rotation.
                            // Do it hardly, with no wait or smooth.
                            // TODO.
                            // Second, start the smoothed movement.
                            // TODO.

                            // Then remove this return statement.
                            yield return null;
                        }
                        finally
                        {
                            currentRotation = null;
                        }
                    }

                    private void Update()
                    {
                        if (currentRotation == null && Watcher)
                        {
                            // Adjust the camera position.
                            Watcher.CameraPosition = (Vector2)transform.localPosition + Offset;
                        }
                    }

                    /// <summary>
                    ///   The target watcher to force a follow.
                    /// </summary>
                    public CubeWatcher Watcher;
                    
                    /// <summary>
                    ///   <para>
                    ///     An offset, by default (1/2, 1/2), to apply to the watcher's camera.
                    ///     Typically, set once and unchanged. Still, open for free-change.
                    ///   </para>
                    ///   <para>
                    ///     typically, the offset becomes (k, k) where k = object's width / 2.
                    ///   </para>
                    /// </summary>
                    public Vector2 Offset = Vector2.one * 0.5f;
                }
            }
        }
    }
}

