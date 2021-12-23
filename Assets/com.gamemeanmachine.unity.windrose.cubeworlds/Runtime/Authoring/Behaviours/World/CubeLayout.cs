using System;
using System.Collections.Generic;
using AlephVault.Unity.Support.Utils;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.CubeWorlds.Authoring.Behaviours.Entities;
using GameMeanMachine.Unity.WindRose.CubeWorlds.Types;
using GameMeanMachine.Unity.WindRose.NeighbourTeleports.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEditor;
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
                ///   <para>
                ///     A cube layout relates to a lot of children maps satisfying
                ///     the following properties:
                ///   </para>
                ///   <para>
                ///     - Each map will be a <see cref="CubeFace"/>.
                ///     - There are faces belonging to the surface, assembling a
                ///       cube.
                ///     - There will be a face working as "basement entrance".
                ///       Typically, this face is the front one, but it actually
                ///       does not matter.
                ///     - Each basement level has a Front orientation. The distance
                ///       to the Front face cube and consecutive levels is
                ///       <see cref="delta"/>, although having their center aligned,
                ///       and it also has 2*delta less cells on each side (some sort
                ///       of a squared "cone" toward the center).
                ///   </para>
                /// </summary>
                public class CubeLayout : MonoBehaviour
                {
                    private struct LinkSetting
                    {
                        public FaceOrientation source;
                        public Direction sourceSide;
                        public FaceOrientation destination;
                        public Direction destinationSide;
                    }

                    private static LinkSetting[] linkSettings = {
                        // Front side
                        new LinkSetting
                        {
                            source = FaceOrientation.Front, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Left, destinationSide = Direction.RIGHT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Front, sourceSide = Direction.UP,
                            destination = FaceOrientation.Up, destinationSide = Direction.DOWN
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Front, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Right, destinationSide = Direction.LEFT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Front, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Down, destinationSide = Direction.DOWN
                        },
                        // Back side
                        new LinkSetting
                        {
                            source = FaceOrientation.Back, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Right, destinationSide = Direction.RIGHT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Back, sourceSide = Direction.UP,
                            destination = FaceOrientation.Up, destinationSide = Direction.UP
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Back, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Left, destinationSide = Direction.LEFT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Back, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Down, destinationSide = Direction.UP
                        },
                        // Left side
                        new LinkSetting
                        {
                            source = FaceOrientation.Left, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Back, destinationSide = Direction.RIGHT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Left, sourceSide = Direction.UP,
                            destination = FaceOrientation.Up, destinationSide = Direction.LEFT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Left, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Front, destinationSide = Direction.LEFT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Left, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Down, destinationSide = Direction.RIGHT
                        },
                        // Right side
                        new LinkSetting
                        {
                            source = FaceOrientation.Right, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Front, destinationSide = Direction.RIGHT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Right, sourceSide = Direction.UP,
                            destination = FaceOrientation.Up, destinationSide = Direction.RIGHT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Right, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Back, destinationSide = Direction.LEFT
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Right, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Down, destinationSide = Direction.LEFT
                        },
                        // Up side
                        new LinkSetting
                        {
                            source = FaceOrientation.Up, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Left, destinationSide = Direction.UP
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Up, sourceSide = Direction.UP,
                            destination = FaceOrientation.Back, destinationSide = Direction.UP
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Up, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Right, destinationSide = Direction.UP
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Up, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Front, destinationSide = Direction.UP
                        },
                        // Down side
                        new LinkSetting
                        {
                            source = FaceOrientation.Down, sourceSide = Direction.LEFT,
                            destination = FaceOrientation.Right, destinationSide = Direction.DOWN
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Down, sourceSide = Direction.UP,
                            destination = FaceOrientation.Back, destinationSide = Direction.DOWN
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Down, sourceSide = Direction.RIGHT,
                            destination = FaceOrientation.Left, destinationSide = Direction.DOWN
                        },
                        new LinkSetting
                        {
                            source = FaceOrientation.Down, sourceSide = Direction.DOWN,
                            destination = FaceOrientation.Front, destinationSide = Direction.DOWN
                        },
                    };
                    
                    /// <summary>
                    ///   <para>
                    ///     How many cells, on each direction, do each basement
                    ///     level (and surface) adds out of the previous level
                    ///     (or 0, for the bottommost).
                    ///   </para>
                    ///   <para>
                    ///     <see cref="delta"/> * 2 * (<see cref="basements"/> + 1)
                    ///     is the amount of cells in each of the surface maps.
                    ///   </para>
                    ///   <para>
                    ///     The default value is 5, and the minimum is 1.
                    ///   </para>
                    /// </summary>
                    [SerializeField]
                    private byte delta = 5;

                    /// <summary>
                    ///   <para>
                    ///     How many basement levels this cube layout has. This
                    ///     does not include the surface level.
                    ///   </para>
                    ///   <para>
                    ///     <see cref="delta"/> * 2 * (<see cref="basements"/> + 1)
                    ///     is the amount of cells in each of the surface maps.
                    ///   </para>
                    ///   <para>
                    ///     The default value is 1, and the minimum is 0.
                    ///   </para>
                    /// </summary>
                    [SerializeField]
                    private byte basements = 1;

                    /// <summary>
                    ///   <para>
                    ///     The expected cell size for the maps. Each inner map, to
                    ///     be detected, must have a cell size of (cellSize, cellSize, ?).
                    ///   </para>
                    ///   <para>
                    ///     The minimum cell size is <see cref="Mathf.Epsilon"/> and the
                    ///     default cell size is 1.
                    ///   </para>
                    /// </summary>
                    [SerializeField]
                    private float cellSize = 1f;

                    /// <summary>
                    ///   The color to use for basement backgrounds.
                    /// </summary>
                    [SerializeField]
                    private Color32 basementBackgroundColor = new Color(139f/255,69f/255,19f/255);

                    // This is the surface level faces.
                    private Dictionary<FaceOrientation, Map> surfaceFaces;

                    // This is the basement level faces.
                    private Map[] basementFaces;

                    // The width and height of surface size.
                    private ushort surfaceSize;

                    /// <summary>
                    ///   The face size of the cube, in game units.
                    /// </summary>
                    public float FaceSize()
                    {
                        return cellSize * surfaceSize;
                    }

                    // The expected size of a given basement.
                    private ushort BasementSize(byte level)
                    {
                        return (ushort)(2 * delta * (basements + 1 - level));
                    }

                    // Inverts the normal of a Quad.
                    private void InvertNormals(MeshFilter filter)
                    {
                        Vector3[] normals = filter.mesh.normals;
                        for (int i = 0; i < normals.Length; i++) normals[i] = -normals[i];
                        filter.mesh.normals = normals;

                        int[] triangles = filter.mesh.triangles;
                        for (int i = 0; i < triangles.Length; i += 3)
                        {
                            (triangles[i], triangles[i + 2]) = (triangles[i + 2], triangles[i]);
                        }
                        filter.mesh.triangles = triangles;
                    }

                    // Validates a map and aligns it.
                    private void ValidateAndAlign(
                        Map map, FaceType faceType,
                        FaceOrientation faceOrientation,
                        byte level
                    )
                    {
                        if (map.Width != map.Height || Mathf.Epsilon < map.CellSize.x - map.CellSize.y)
                        {
                            Debug.LogWarning($"The map is not square", map);
                        }
                        else if (Mathf.Epsilon < map.CellSize.x - cellSize)
                        {
                            Debug.LogWarning(
                                $"The map has an unexpected cell size (expected: ({cellSize}, {cellSize}, ?))",
                                map
                            );
                        }
                        else if (faceType == FaceType.Surface)
                        {
                            if (!Enum.IsDefined(typeof(FaceOrientation), faceOrientation))
                            {
                                Debug.LogWarning($"Unknown face orientation: {faceOrientation}", map);
                            }
                            if (surfaceFaces.ContainsKey(faceOrientation))
                            {
                                Debug.LogWarning(
                                    $"Orientation {faceOrientation} is already occupied by " +
                                    "another map", map
                                );
                            }
                            else if (map.Width != surfaceSize)
                            {
                                Debug.LogWarning(
                                    "(Height, Width) of a surface map must be " +
                                    $"({surfaceSize}, {surfaceSize})",
                                    map
                                );
                            }
                            else
                            {
                                // Store the face and adjust its position.
                                surfaceFaces[faceOrientation] = map;
                                Vector3 cubePivotPosition = faceOrientation.CubicAssemblyPosition() * 
                                                            ((basements + 1) * cellSize * delta);
                                map.transform.localPosition = cubePivotPosition;
                                // Also, create a Quad, with:
                                // - Same local cubic position.
                                // - Same local orientation.
                                // - Color: The given color here.
                                // - Scale: Vector3.one * (basements + 1) * cellSize * delta * 2.
                                // - Same parent (i.e. this).
                                // - Inverted normals.
                                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                quad.transform.parent = map.transform;
                                quad.transform.localPosition =
                                    (Vector3)(Vector2.one * (basements + 1) * cellSize * delta) +
                                    0.001f * Vector3.one;
                                quad.transform.localRotation = Quaternion.identity;
                                quad.transform.localScale = Vector3.one * ((basements + 1) * cellSize * delta - 0.001f) * 2;
                                quad.GetComponent<Renderer>().material.color = basementBackgroundColor;
                                InvertNormals(quad.GetComponent<MeshFilter>());
                            }
                        }
                        else if (faceType == FaceType.Basement)
                        {
                            ushort basementSize = BasementSize(level);
                            if (level == 0 || level > basements)
                            {
                                Debug.LogWarning($"Level {level} is 0 or above the number of basements", map);
                            }
                            if (basementFaces[level - 1] != null)
                            {
                                Debug.LogWarning($"Level {level} is already occupied by another map", map);
                            }
                            else if (map.Width != basementSize)
                            {
                                Debug.LogWarning(
                                    "(Height, Width) of a basement map must be " +
                                    $"({basementSize}, {basementSize})", map
                                );
                            }
                            else
                            {
                                // Store the basement and adjust its position.
                                basementFaces[level - 1] = map;
                                map.transform.localPosition = -Vector3.one * (basements + 1 - level) *
                                                              cellSize * delta;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Unknown face type: {faceType}", map);
                        }
                    }

                    // Joins the maps in the surface appropriately.
                    private void JoinSurface()
                    {
                        foreach (LinkSetting setting in linkSettings)
                        {
                            if (surfaceFaces.TryGetValue(setting.source, out Map sourceMap) &&
                                surfaceFaces.TryGetValue(setting.destination, out Map destinationMap))
                            {
                                NeighbourTeleportObjectsManagementStrategy sourceOMS =
                                    sourceMap.ObjectsLayer.GetComponent<NeighbourTeleportObjectsManagementStrategy>();
                                NeighbourTeleportObjectsManagementStrategy destinationOMS =
                                    destinationMap.ObjectsLayer.GetComponent<NeighbourTeleportObjectsManagementStrategy>();
                                Debug.Log($"Attempting links: {sourceOMS}:{setting.sourceSide} -> {destinationOMS}:{setting.destinationSide}");
                                if (sourceOMS && destinationOMS)
                                {
                                    sourceOMS.Unlink(setting.sourceSide, false);
                                    sourceOMS.Link(
                                        setting.sourceSide, destinationOMS, 
                                        setting.destinationSide, false
                                    );
                                }
                            }
                        }
                    }

                    private void Awake()
                    {
                        surfaceSize = (ushort)(2 * delta * (basements + 1));
                        delta = Values.Max<byte>(delta, 1);
                        cellSize = Values.Max(cellSize, Mathf.Epsilon);
                        basementFaces = new Map[basements];
                        surfaceFaces = new Dictionary<FaceOrientation, Map>();
                    }

                    private void Start()
                    {
                        int childCount = transform.childCount;
                        for (int i = 0; i < childCount; i++)
                        {
                            Transform child = transform.GetChild(i);
                            Map map = child.GetComponent<Map>();

                            if (!map) continue;
                            
                            NeighbourTeleportObjectsManagementStrategy neighbourTeleportStrategy =
                                map.ObjectsLayer.GetComponent<NeighbourTeleportObjectsManagementStrategy>();
                            CubeFace cubeFace = neighbourTeleportStrategy.GetComponent<CubeFace>();
                            
                            if (cubeFace != null && neighbourTeleportStrategy != null)
                            {
                                FaceType faceType = cubeFace.FaceType;
                                FaceOrientation faceOrientation = cubeFace.FaceOrientation;
                                byte faceLevel = cubeFace.FaceLevel;
                                ValidateAndAlign(map, faceType, faceOrientation, faceLevel);
                            }
                        }
                        JoinSurface();
                        foreach (CubeFollowed cubeFollowed in GetComponentsInChildren<CubeFollowed>())
                        {
                            cubeFollowed.RefreshWatcherStatus();
                        }
                    }
                }
            }
        }
    }
}
