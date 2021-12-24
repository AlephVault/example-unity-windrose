using System;
using UnityEngine;


namespace GameMeanMachine.Unity.WindRose.CubeWorlds
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Watch
            {
                /// <summary>
                ///   A watcher object - it handles rotation and camera
                ///   settings (size, perspective, distance).
                /// </summary>
                public class CubeWatcher : MonoBehaviour
                {
                    /// <summary>
                    ///   The near clipping plane.
                    /// </summary>
                    public const float NearPlane = 0.3f;
                    
                    /// <summary>
                    ///   The camera this component is tied to.
                    /// </summary>
                    public Camera Camera { get; private set; }

                    // Fetches the camera to use as a child.
                    private Camera FindCamera()
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            Camera cameraComponent = transform.GetChild(i).GetComponent<Camera>();
                            if (cameraComponent) return cameraComponent;
                        }
                        Debug.LogWarning("This watcher has no child camera!", this);
                        return null;
                    }
                    
                    /// <summary>
                    ///   The distance of the camera to the center of the watcher.
                    /// </summary>
                    public float Distance
                    {
                        set
                        {
                            if (Camera)
                            {
                                Transform ct = Camera.transform;
                                ct.localPosition = new Vector3(
                                    ct.localPosition.x,
                                    ct.localPosition.y, 
                                    -Mathf.Max(Mathf.Epsilon, value)
                                );
                            }
                        }
                        get => -Camera.transform.localPosition.z;
                    }
                    
                    /// <summary>
                    ///   The (x, y) position of the camera in its internal plane.
                    /// </summary>
                    public Vector2 CameraPosition
                    {
                        set
                        {
                            if (Camera)
                            {
                                float size = Size;
                                Transform ct = Camera.transform;
                                ct.localPosition = new Vector3(value.x, value.y, ct.localPosition.z);
                                Size = size;
                            }                        
                        }
                        get => Camera ? (Vector2)Camera.transform.localPosition : Vector2.zero;
                    }
                    
                    /// <summary>
                    ///   Whether the camera is orthographic (true) or perspective (false).
                    /// </summary>
                    public bool IsOrthographic
                    {
                        set
                        {
                            if (Camera)
                            {
                                float size = Size;
                                Camera.orthographic = value;
                                Size = size;
                            }
                        }
                        get => Camera && Camera.orthographic;
                    }

                    /// <summary>
                    ///   The vertical size of the camera (from the center). This is the
                    ///   orthographic size when the camera is orthographic, otherwise
                    ///   this is distance*tan(fieldOfView).
                    /// </summary>
                    public float Size
                    {
                        set
                        {
                            value = Mathf.Max(Mathf.Epsilon, value);
                            if (Camera)
                            {
                                if (Camera.orthographic)
                                {
                                    Camera.orthographicSize = value;
                                }
                                else
                                {
                                    Camera.fieldOfView = 2 * Mathf.Atan2(value, -Camera.transform.localPosition.z) * Mathf.Rad2Deg;
                                }
                            }
                        }
                        get
                        {
                            if (Camera)
                            {
                                if (Camera.orthographic)
                                {
                                    return Camera.orthographicSize;
                                }
                                return -Camera.transform.localPosition.z * Mathf.Tan(Camera.fieldOfView / 2 * Mathf.Deg2Rad);
                            }
                            return 0f;
                        }
                    }

                    /// <summary>
                    ///   The clipping size of the camera. Typically, the far clipping plane.
                    ///   On set, it will alter also the near clipping plane to an epsilon.
                    /// </summary>
                    public float ClipDistance
                    {
                        set
                        {
                            value = Mathf.Max(value, NearPlane + Mathf.Epsilon);
                            if (Camera)
                            {
                                Camera.farClipPlane = value;
                                Camera.nearClipPlane = NearPlane;
                            }
                        }
                        get => Camera ? Camera.farClipPlane : 0f;
                    }

                    private void Awake()
                    {
                        Camera = FindCamera();
                    }
                }
            }
        }
    }
}
