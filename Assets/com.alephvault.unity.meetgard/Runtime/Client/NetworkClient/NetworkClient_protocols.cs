using AlephVault.Unity.Support.Utils;
using System;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        using AlephVault.Unity.Binary;
        using AlephVault.Unity.Meetgard.Types;
        using System.Collections.Generic;
        using System.Linq;
        using System.Net;
        using System.Net.Sockets;
        using System.Threading.Tasks;

        public partial class NetworkClient : MonoBehaviour
        {
            /// <summary>
            ///   A value telling the version of the current protocol
            ///   set in this network client. This must be changed as
            ///   per deployment, since certain game changes are meant
            ///   to be not retro-compatible and thus the version must
            ///   be marked as mismatching.
            /// </summary>
            [SerializeField]
            private Protocols.Version Version;

            // Protocols will exist by their id (0-based)
            private IProtocolClientSide[] protocols = null;

            // Returns an object to serve as the receiver of specific
            // message data. This must be implemented with the protocol.
            private ISerializable NewMessageContainer(ushort protocolId, ushort messageTag)
            {
                if (protocolId >= protocols.Length)
                {
                    throw new UnexpectedMessageException($"Unexpected message header: ({protocolId}, {messageTag})");
                }
                ISerializable result = protocols[protocolId].NewMessageContainer(messageTag);
                if (result == null)
                {
                    throw new UnexpectedMessageException($"Unexpected message header: ({protocolId}, {messageTag})");
                }
                else
                {
                    return result;
                }
            }

            // Enumerates all of the protocols in this connection.
            // This method will be invoked on Awake, to prepare
            // the list of protocols.
            private void SetupClientProtocols()
            {
                // The first thing is to detect the Zero protocol manually.
                // This is done because, otherwise, adding RequireComponent
                // would force the Zero protocol into a circular dependency
                // in the editor.
                ZeroProtocolClientSide zeroProtocol = GetComponent<ZeroProtocolClientSide>();
                if (zeroProtocol == null)
                {
                    Destroy(gameObject);
                    throw new MissingZeroProtocol("This NetworkClient does not have a ZeroProtocolClientSide protocol behaviour added - it must have one");
                }
                var protocolList = GetComponents<IProtocolClientSide>().ToList();
                protocolList.Remove(zeroProtocol);
                protocolList.Sort((a, b) => {
                    var fa = a.GetType().FullName;
                    var fb = a.GetType().FullName;

                    if (fa == fb)
                    {
                        return 0;
                    }
                    else
                    {
                        return fa.CompareTo(fb);
                    }
                });
                protocolList.Insert(0, zeroProtocol);
                protocols = protocolList.ToArray();
            }
        }
    }
}
