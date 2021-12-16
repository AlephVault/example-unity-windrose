using GameMeanMachine.Unity.WindRose.NeighbourTeleports.Authoring.Behaviours.World.Layers.Objects.ObjectsManagementStrategies;
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
                /// <summary>
                ///   Depends on neighbour teleport strategy and also behaves
                ///   as a cube face (so it reorients appropriately - basement
                ///   layers always reorient to the front side). It requires
                ///   to be squared and creates 6 internal pivots.
                /// </summary>
                [RequireComponent(typeof(NeighbourTeleportObjectsManagementStrategy))]
                public class CubeFace : MonoBehaviour
                {
                    /// <summary>
                    ///   The face type for this map. Either a surface
                    ///   map, or a basement map.
                    /// </summary>
                    [SerializeField]
                    private FaceType faceType;

                    /// <summary>
                    ///   See <see cref="faceType"/>.
                    /// </summary>
                    public FaceType FaceType => faceType;

                    /// <summary>
                    ///   The face orientation for this map. Only meaningful
                    ///   when <see cref="faceType"/> is Surface.
                    /// </summary>
                    [SerializeField]
                    private FaceOrientation faceOrientation;

                    /// <summary>
                    ///   See <see cref="faceOrientation"/>.
                    /// </summary>
                    public FaceOrientation FaceOrientation => faceOrientation;

                    /// <summary>
                    ///   The basement level for this map. Only meaningful
                    ///   when <see cref="faceType"/> is Basement.
                    /// </summary>
                    [SerializeField]
                    private byte faceLevel;

                    /// <summary>
                    ///   See <see cref="faceLevel"/>.
                    /// </summary>
                    public byte FaceLevel => faceLevel;
                    
                    // Sets the local rotation of the map.
                    protected void Awake()
                    {
                        // Set the rotation of the object to the appropriate
                        // orientation given the face. For basement layers,
                        // the rotation is always front.
                        transform.localRotation = faceType == FaceType.Surface ?
                            FaceOrientation.Rotation() :
                            FaceOrientation.Front.Rotation();
                    }
                }
            }
        }
    }
}
