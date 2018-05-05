using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Misc
        {
            [RequireComponent(typeof(CircleCollider2D))]
            class Command : MonoBehaviour
            {
                /**
                 * A command is just a small circle collider which can hold data
                 *   referring the "command" being sent. Such "command" is as meaningful
                 *   as desired and interpreted by the receiver that errrr... "receives"
                 *   it.
                 * 
                 * The collider will have a small radius and act as just a trigger (i.e.
                 *   not as a regular collision).
                 * 
                 * The receiver will be a live trigger (e.g. a character or solid object).
                 * 
                 * ANY OBJECT OR MEDIUM CAN CAST A COMMAND. There is no restriction, but
                 *   just the suggestion of moving the data accordingly. However, the ideal
                 *   scenario is that commands are cast by a controlled environment (e.g.
                 *   a CloseCommandSender, which requires both odd width and height (in
                 *   map tiles) and casts a command appropriately in that direction.
                 */
                public Positionable sender;
                public string name;
                public object[] arguments;

                public void Start()
                {
                    CircleCollider2D collider = GetComponent<CircleCollider2D>();
                    collider.radius = 0.25f * Map.GAME_UNITS_PER_TILE_UNITS;
                    collider.isTrigger = true;
                }
            }
        }
    }
}
