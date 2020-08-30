using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Objects
            {
                using WindRose.Behaviours.Entities.Objects;

                /// <summary>
                ///   Networked trigger holders disable all the colliders of a behaviour,
                ///     if the game is not server-side.
                /// </summary>
                [RequireComponent(typeof(TriggerHolder))]
                public class NetworkedTriggerHolder : NetworkBehaviour
                {
                    public void Start()
                    {
                        if (!isServer)
                        {
                            foreach(Collider collider in GetComponents<Collider>())
                            {
                                collider.enabled = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
