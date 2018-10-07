using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Teleport
            {
                [RequireComponent(typeof(TriggerPlatform))]
                public class TeleportTarget : MonoBehaviour
                {
                    /**
                     * A teleport target is a placeholder to allow
                     *   teleportation. It will be a trigger platform
                     *   and will have more logic when implementing
                     *   the remote teleporters (i.e. teleporting
                     *   across different scenes).
                     *   
                     * Right now the teleport target will only know
                     *   how must the object be oriented when ending
                     *   its teleportation here, if an orientation is
                     *   given.
                     */
                    public bool ForceOrientation = true;
                    public Types.Direction NewOrientation = Types.Direction.DOWN;
                }
            }
        }
    }
}
