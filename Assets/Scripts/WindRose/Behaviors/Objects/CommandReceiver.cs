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

                private Misc.Command GetCommand(Collision2D collision)
                {
                    return collision.collider.gameObject.GetComponent<Misc.Command>();
                }

                private void SendCommandStatusFromCollision(Collision2D collision, CommandStage stage)
                {
                    Misc.Command command = GetCommand(collision);
                    if (command != null)
                    {
                        SendMessage("OnCommandReceived", new CommandStatus(command, stage), SendMessageOptions.DontRequireReceiver);
                    }
                }

                private void OnCollisionEnter2D(Collision2D collision)
                {
                    SendCommandStatusFromCollision(collision, CommandStage.ENTER);
                }

                private void OnCollisionExit2D(Collision2D collision)
                {
                    SendCommandStatusFromCollision(collision, CommandStage.EXIT);
                }

                private void OnCollisionStay2D(Collision2D collision)
                {
                    SendCommandStatusFromCollision(collision, CommandStage.STAY);
                }
            }
        }
    }
}
