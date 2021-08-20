using AlephVault.Unity.Binary;
using System;
using System.Linq;
using System.Collections.Generic;
using AlephVault.Unity.Meetgard.Types;

namespace AlephVault.Unity.Meetgard
{
    namespace Protocols
    {
        /// <summary>
        ///   <para>
        ///     The "zero" protocol is the first one to be accounted
        ///     and it is the one that synchronizes the endpoints
        ///     to make a version handshake, among other things.
        ///   </para>
        /// </summary>
        public class ZeroProtocolDefinition : ProtocolDefinition
        {
            protected override void DefineMessages()
            {
                DefineServerMessage<Nothing>("LetsAgree");
                DefineServerMessage<Nothing>("Timeout");
                DefineClientMessage<Version>("MyVersion");
                DefineServerMessage<Nothing>("VersionMatch");
                DefineServerMessage<Nothing>("VersionMismatch");
                DefineServerMessage<Nothing>("NotReady");
                DefineServerMessage<Nothing>("AlreadyDone");
            }
        }
    }
}
