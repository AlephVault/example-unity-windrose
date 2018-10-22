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
                    [RequireComponent(typeof(CloseCommandSender))]
                    class TalkSender : MonoBehaviour
                    {
                        /**
                         * This behaviour offers a .Talk() method to start a usual chat
                         *   interaction with an NPC.
                         */

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
}