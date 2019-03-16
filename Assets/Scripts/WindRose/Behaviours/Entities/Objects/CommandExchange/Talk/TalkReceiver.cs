using System;
using System.Collections;
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
                namespace Talk
                {
                    /// <summary>
                    ///   This class is usually attached to NPCs. It will receive the
                    ///     <see cref="TalkSender.COMMAND"/> sent by an <see cref="TalkSender"/>
                    ///     and trigger the <see cref="onTalkReceived"/> event.
                    /// </summary>
                    /// <remarks>
                    ///   When receiving the command, this object will look towards the opposite
                    ///     direction the sender object is looking to, and then trigger the
                    ///     <see cref="onTalkReceived"/> event.
                    /// </remarks>
                    [RequireComponent(typeof(Oriented))]
                    [RequireComponent(typeof(CommandReceiver))]
                    class TalkReceiver : MonoBehaviour
                    {
                        Oriented oriented;

                        [Serializable]
                        public class UnityTalkReceivedEvent : UnityEvent<GameObject> { }

                        /// <summary>
                        ///   This event triggers when a <see cref="TalkSender.COMMAND"/> is received.
                        /// </summary>
                        /// <remarks>
                        ///   Only ONE UI-interaction-triggering handler should be added to this event.
                        /// </remarks>
                        public readonly UnityTalkReceivedEvent onTalkReceived = new UnityTalkReceivedEvent();

                        private void Start()
                        {
                            oriented = GetComponent<Oriented>();
                            GetComponent<CommandReceiver>().onCommandReceiver.AddListener(delegate (CommandReceiver.CommandStatus status)
                            {
                                if (status.Stage == CommandReceiver.CommandStage.ENTER && status.Command.name == TalkSender.COMMAND)
                                {
                                    StartCoroutine(StartTalk(status.Command.sender));
                                }
                            });
                        }

                        IEnumerator StartTalk(GameObject sender)
                        {
                            Oriented senderOriented = sender.GetComponent<Oriented>();
                            if (senderOriented)
                            {
                                switch (senderOriented.orientation)
                                {
                                    case Types.Direction.DOWN:
                                        oriented.orientation = Types.Direction.UP;
                                        break;
                                    case Types.Direction.UP:
                                        oriented.orientation = Types.Direction.DOWN;
                                        break;
                                    case Types.Direction.LEFT:
                                        oriented.orientation = Types.Direction.RIGHT;
                                        break;
                                    case Types.Direction.RIGHT:
                                        oriented.orientation = Types.Direction.LEFT;
                                        break;
                                }
                                yield return new WaitForSeconds(0f);
                            }
                            onTalkReceived.Invoke(sender);
                        }
                    }
                }
            }
        }
    }
}