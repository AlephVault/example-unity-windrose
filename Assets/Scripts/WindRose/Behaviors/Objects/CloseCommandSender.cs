using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(Oriented))]
            class CloseCommandSender : MonoBehaviour
            {
                /**
                 * This behaviour has convenience methods to cast a command. Casting a command
                 *   actually means actively interacting with an object that can receive such
                 *   command (e.g. talk, hit, use/press).
                 * 
                 * Ideally, each object having a CloseCommandSender behaviour will have one
                 *   distinct Command object related to himself. However this is not enforced
                 *   (and could be efficient for some kind of games to use a single command;
                 *   yet it is still up to you and how you design your commands, since you
                 *   could implement the EXIT status on a command receiver and so using a
                 *   single command object for the whole game could be counterproductive.
                 * 
                 * You can cast a command indefinitely, or cast it just for one instant.
                 * You can specify arbitrary command and arguments.
                 *   
                 * THIS IS JUST A CONVENIENCE BEHAVIOUR AND BY NO MEANS SHOULD THIS ONE
                 *   CONSIDERED AS EXTENSIVE IN IMPLEMENTATION. Good enough to close combat
                 *   or implementing a talk mechanism with NPCs.
                 */

                [SerializeField]
                private Misc.Command command;
                private Oriented oriented;
                private Positionable positionable;

                private void Start()
                {
                    oriented = GetComponent<Oriented>();
                    positionable = GetComponent<Positionable>();
                    Release();
                }
                
                private void FixCommandPosition()
                {
                    float x, y;
                    switch(oriented.orientation)
                    {
                        case Types.Direction.DOWN:
                            x = positionable.transform.position.x + (positionable.Width / 2f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            y = positionable.transform.position.y - (positionable.Height + 0.5f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            break;
                        case Types.Direction.UP:
                            x = positionable.transform.position.x + (positionable.Width / 2f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            y = positionable.transform.position.y + 0.5f * Map.GAME_UNITS_PER_TILE_UNITS;
                            break;
                        case Types.Direction.LEFT:
                            y = positionable.transform.position.y - (positionable.Height / 2f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            x = positionable.transform.position.x - 0.5f * Map.GAME_UNITS_PER_TILE_UNITS;
                            break;
                        case Types.Direction.RIGHT:
                            y = positionable.transform.position.y - (positionable.Height / 2f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            x = positionable.transform.position.x + (positionable.Width + 0.5f) * Map.GAME_UNITS_PER_TILE_UNITS;
                            break;
                        default:
                            x = command.transform.position.x;
                            y = command.transform.position.y;
                            break;
                    }
                    command.transform.position = new Vector3(x, y, command.transform.position.z);
                }

                private void SetCommandData(string commandName, params object[] arguments)
                {
                    command.sender = gameObject;
                    command.name = commandName;
                    command.arguments = arguments;
                }

                public void Cast(string commandName, bool instantaneous = true, params object[] arguments)
                {
                    FixCommandPosition();
                    SetCommandData(commandName, arguments);
                    command.gameObject.SetActive(false);
                    if (instantaneous)
                    {
                        StartCoroutine(InstantRelease());
                    }
                }

                private IEnumerator InstantRelease()
                {
                    yield return new WaitForSeconds(0f);
                    Release();
                }

                public void Release()
                {
                    command.gameObject.SetActive(false);
                }
            }
        }
    }
}
