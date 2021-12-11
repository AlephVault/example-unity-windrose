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
                ///   as a cube face (so it reorients appropriately). It also
                ///   serves to <see cref="CubePivot"/>, which takes 6 cube
                ///   faces and assembles them into a cube.
                /// </summary>
                [RequireComponent(typeof(NeighbourTeleportObjectsManagementStrategy))]
                public class CubeFace : MonoBehaviour
                {
                    /// <summary>
                    ///   The face orientation for this map.
                    /// </summary>
                    [SerializeField]
                    private FaceOrientation faceOrientation;

                    /// <summary>
                    ///   See <see cref="faceOrientation"/>.
                    /// </summary>
                    public FaceOrientation FaceOrientation => faceOrientation;

                    // Sets the local rotation of the map.
                    protected void Awake()
                    {
                        transform.localRotation = FaceOrientation.Rotation();
                    }
                }
            }
        }
    }
}
