
using AlephVault.Unity.Meetgard.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        /// <summary>
        ///   Server-side implementation for the "zero" protocol.
        /// </summary>
        public class ZeroProtocolServerSide : ProtocolServerSide<ZeroProtocolDefinition>
        {
            /// <summary>
            ///   A value telling the version of the current protocol
            ///   set in this network server. This must be changed as
            ///   per deployment, since certain game changes are meant
            ///   to be not retro-compatible and thus the version must
            ///   be marked as mismatching.
            /// </summary>
            [SerializeField]
            private Protocols.Version Version;

            protected override void SetIncomingMessageHandlers()
            {
                throw new NotImplementedException();
            }
        }
    }
}