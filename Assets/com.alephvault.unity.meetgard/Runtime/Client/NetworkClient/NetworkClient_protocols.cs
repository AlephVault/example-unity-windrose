using System;
using UnityEngine;
using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Types;
using AlephVault.Unity.Layout.Utils;
using System.Linq;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
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

            // Handles a received message. The received message will be
            // handled by the underlying protocol handler.
            private void HandleMessage(ushort protocolId, ushort messageTag, ISerializable message)
            {
                // At this point, the protocolId exists. Also, the messageTag exists.
                // We get the client-side handler, and we invoke it.
                Action<NetworkClient, ISerializable> handler = protocols[protocolId].GetHandler(messageTag);
                if (handler != null)
                {
                    handler(this, message);
                }
                else
                {
                    Debug.LogWarning($"Message ({protocolId}, {messageTag}) does not have any handler!");
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
                var protocolList = (from protocolClientSide in GetComponents<IProtocolClientSide>() select (Component)protocolClientSide).ToList();
                protocolList.Remove(zeroProtocol);
                Behaviours.SortByDependencies(protocolList.ToArray()).ToList();
                protocolList.Insert(0, zeroProtocol);
                protocols = (from protocolClientSide in protocolList select (IProtocolClientSide)protocolClientSide).ToArray();
            }
        }
    }
}
