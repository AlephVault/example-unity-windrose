using System.Collections;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace CommandExchange
            {
                namespace Talk
                {
                    [RequireComponent(typeof(Oriented))]
                    [RequireComponent(typeof(CommandReceiver))]
                    class TalkReceiver : MonoBehaviour
                    {
                        /**
                         * This behaviour understands commands sent with .Talk() and attends them
                         *   appropriately by triggering a new event named OnTalkCommandReceived.
                         * 
                         * Such callback receives an argument: The gameObject (it could be anything!)
                         *   who wanted to start a chat interaction.
                         * 
                         * Since you can only talk to actual positinable objects, we will also require
                         *   the object being orientable. When the object is being talked to, it will
                         *   look towards the direction of the sender (if the sender is also oriented,
                         *   which should be the case if the talk command was initiated by a TalkSender).
                         */

                        Oriented oriented;
                        private void Start()
                        {
                            oriented = GetComponent<Oriented>();
                        }

                        void OnCommandReceived(CommandReceiver.CommandStatus status)
                        {
                            if (status.Stage == CommandReceiver.CommandStage.ENTER && status.Command.name == "WR:Talk")
                            {
                                StartCoroutine(StartTalk(status.Command.sender));
                            }
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
                            SendMessage("OnTalkCommandReceived", sender);
                        }
                    }
                }
            }
        }
    }
}