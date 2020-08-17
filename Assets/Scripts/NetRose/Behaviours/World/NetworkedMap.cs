using UnityEngine;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace World
        {
            using WindRose.Behaviours.World;
            using WindRose.Behaviours.World.Layers.Objects;

            /// <summary>
            ///   Networked maps will force maps' <see cref="ObjectsManagementStrategyHolder"/>
            ///     in objects layer to mark their <see cref="ObjectsManagementStrategyHolder.Bypass"/>
            ///     to true (if server or host), or false (otherwise).
            /// </summary>
            [RequireComponent(typeof(Map))]
            public class NetworkedMap : BaseBehaviour
            {
                private void Start()
                {
                    Map map = GetComponent<Map>();
                    if (!map) return;

                    map.ObjectsLayer.StrategyHolder.Bypass = !isServer;
                }
            }
        }
    }
}