using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace CommandExchange
            {
                [RequireComponent(typeof(CommandReceiver))]
                class TalkReceiver : MonoBehaviour
                {
                    void OnCommandReceived(CommandReceiver.CommandStatus status)
                    {
                        if (status.Stage == CommandReceiver.CommandStage.ENTER && status.Command.name == "WR:Talk")
                        {
                            SendMessage("OnTalkCommandReceived", status.Command.gameObject);
                        }
                    }
                }
            }
        }
    }
}