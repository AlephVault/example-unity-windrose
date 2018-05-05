using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace CommandExchange
            {
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