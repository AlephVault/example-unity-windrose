using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            [RequireComponent(typeof(TriggerLive))]
            class CommandReceiver : MonoBehaviour
            {
                /**
                 * Receives a collision from a Command, which is a special object containing
                 *   arbitrary data about the command to be captured. Only object being
                 *   subclasses of TriggerLive can receive commands.
                 * 
                 * This object needs to detect the collision of the command, and so it will
                 *   need somehow to handle collisions.
                 */

                public enum CommandStage
                {
                    ENTER, EXIT, STAY
                }

                public class CommandStatus
                {
                    public readonly CommandStage Stage;
                    public readonly Misc.Command Command;
                    public CommandStatus(Misc.Command command, CommandStage stage)
                    {
                        Stage = stage;
                        Command = command;
                    }
                }

                private Misc.Command GetCommand(Collider2D collider)
                {
                    return collider.gameObject.GetComponent<Misc.Command>();
                }

                private void SendCommandStatusFromCollision(Collider2D collider, CommandStage stage)
                {
                    Misc.Command command = GetCommand(collider);

                    if (command != null)
                    {
                        if (command.sender != null && command.sender.gameObject != gameObject)
                        {
                            SendMessage("OnCommandReceived", new CommandStatus(command, stage), SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }

                private void OnTriggerEnter2D(Collider2D collider)
                {
                    SendCommandStatusFromCollision(collider, CommandStage.ENTER);
                }

                private void OnTriggerExit2D(Collider2D collider)
                {
                    SendCommandStatusFromCollision(collider, CommandStage.EXIT);
                }

                private void OnTriggerStay2D(Collider2D collider)
                {
                    SendCommandStatusFromCollision(collider, CommandStage.STAY);
                }

                void Pause(bool fullFreeze)
                {
                    enabled = false;
                }

                void Resume()
                {
                    enabled = true;
                }
            }
        }
    }
}
