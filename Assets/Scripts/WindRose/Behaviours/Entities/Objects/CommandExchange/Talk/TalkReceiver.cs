using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GMM.Utils;

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
							GetComponent<CommandReceiver>().ListenCommand(TalkSender.COMMAND, (string commandName, object[] arguments, GameObject sender) => {
								StartTalk(sender);
                            });
                        }

                        private async void StartTalk(GameObject sender)
                        {
                            Oriented senderOriented = sender.GetComponent<Oriented>();
                            if (senderOriented)
                            {
                                switch (senderOriented.Orientation)
                                {
                                    case Types.Direction.DOWN:
                                        oriented.Orientation = Types.Direction.UP;
                                        break;
                                    case Types.Direction.UP:
                                        oriented.Orientation = Types.Direction.DOWN;
                                        break;
                                    case Types.Direction.LEFT:
                                        oriented.Orientation = Types.Direction.RIGHT;
                                        break;
                                    case Types.Direction.RIGHT:
                                        oriented.Orientation = Types.Direction.LEFT;
                                        break;
                                }
                                await Tasks.Blink();
                            }
                            onTalkReceived.Invoke(sender);
                        }
                    }
                }
            }
        }
    }
}