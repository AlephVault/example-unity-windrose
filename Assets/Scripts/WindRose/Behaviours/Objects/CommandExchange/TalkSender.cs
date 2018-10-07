using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace CommandExchange
            {
                /**
                 * This behaviour offers a .Talk() method to start a usual chat
                 *   interaction with an NPC.
                 */
                [RequireComponent(typeof(CloseCommandSender))]
                class TalkSender : MonoBehaviour
                {
                    private CloseCommandSender sender;
                    private void Start()
                    {
                        sender = GetComponent<CloseCommandSender>();
                    }

                    public void Talk()
                    {
                        sender.Cast("WR:Talk");
                    }
                }
            }
        }
    }
}