using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace CommandExchange
            {
                /// <summary>
                ///   <para>
                ///     Receives a <see cref="Misc.Command"/> sent from somewhere. Since receiving
                ///       a command involves detecting collisions, this component also requires a
                ///       <see cref="TriggerLive"/> which does most of the work for us.
                ///   </para>
                ///   <para>
                ///     Other components depending on this one may be interested in adding listeners
                ///       to <see cref="onCommandReceiver"/> handler.
                ///   </para>
                /// </summary>
                [RequireComponent(typeof(TriggerLive))]
                class CommandReceiver : MonoBehaviour, Pausable.IPausable
                {
                    /// <summary>
                    ///   The stage of the command arrival (just entering,
                    ///     just leaving, or present).
                    /// </summary>
                    public enum CommandStage
                    {
                        ENTER, EXIT, STAY
                    }

                    /// <summary>
                    ///   A command status holds the details of the command and its stage.
                    /// </summary>
                    public class CommandStatus
                    {
                        /// <summary>
                        ///   The stage this event is processed in (command just entering,
                        ///     command just leaving, or command present).
                        /// </summary>
                        public readonly CommandStage Stage;
                        /// <summary>
                        ///   The <see cref="Misc.Command"/> being processed.
                        /// </summary>
                        public readonly Misc.Command Command;
                        public CommandStatus(Misc.Command command, CommandStage stage)
                        {
                            Stage = stage;
                            Command = command;
                        }
                    }

                    [Serializable]
                    public class UnityCommandReceivedEvent : UnityEvent<CommandStatus> { }

                    /// <summary>
                    ///   This event is triggered when the object receives a command from somewhere.
                    ///   Other behaviours should add listeners to this event and handle the event
                    ///     being received.
                    /// </summary>
                    public readonly UnityCommandReceivedEvent onCommandReceiver = new UnityCommandReceivedEvent();

                    private Misc.Command GetCommand(Collider2D collider)
                    {
                        return collider.gameObject.GetComponent<Misc.Command>();
                    }

                    private void SendCommandStatusFromCollision(Collider2D collider, CommandStage stage)
                    {
                        Misc.Command command = GetCommand(collider);

                        if (enabled && command != null)
                        {
                            if (command.sender != null && command.sender.gameObject != gameObject)
                            {
                                onCommandReceiver.Invoke(new CommandStatus(command, stage));
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

                    /// <summary>
                    ///   Pauses the object - this means the object will not attend commands.
                    /// </summary>
                    public void Pause(bool fullFreeze)
                    {
                        enabled = false;
                    }

                    /// <summary>
                    ///   Resumes the object - it will attend commands again.
                    /// </summary>
                    public void Resume()
                    {
                        enabled = true;
                    }
                }
            }
        }
    }
}
