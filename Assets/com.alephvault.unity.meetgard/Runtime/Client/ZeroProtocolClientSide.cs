
using AlephVault.Unity.Meetgard.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        /// <summary>
        ///   Client-side implementation for the "zero" protocol.
        /// </summary>
        public class ZeroProtocolClientSide : ProtocolClientSide<ZeroProtocolDefinition>
        {
            protected override void SetIncomingMessageHandlers()
            {
                // TODO implement this!!! And also implement the ZeroProtocolDefinition itself.
                throw new NotImplementedException();
            }
        }
    }
}