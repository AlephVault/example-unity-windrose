using AlephVault.Unity.Binary;
using AlephVault.Unity.Layout.Utils;
using AlephVault.Unity.Meetgard.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Server
    {
        public partial class NetworkServer : MonoBehaviour
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

            // Protocols will exist by their id (0-based)
            private IProtocolServerSide[] protocols = null;

            // Returns an object to serve as the receiver of specific
            // message data. This must be implemented with the protocol.
            private ISerializable NewMessageContainer(ushort protocolId, ushort messageTag)
            {
                if (protocolId >= protocols.Length)
                {
                    throw new UnexpectedMessageException($"Unexpected incoming message protocol/tag: ({protocolId}, {messageTag})");
                }
                ISerializable result = protocols[protocolId].NewMessageContainer(messageTag);
                if (result == null)
                {
                    throw new UnexpectedMessageException($"Unexpected outgoing message protocol/tag: ({protocolId}, {messageTag})");
                }
                else
                {
                    return result;
                }
            }

            // Returns the expected type for a message to be sent.
            private Type GetOutgoingMessageType(ushort protocolId, ushort messageTag)
            {
                if (protocolId >= protocols.Length)
                {
                    throw new UnexpectedMessageException($"Unexpected outgoing message protocol/tag: ({protocolId}, {messageTag})");
                }
                Type result = protocols[protocolId].GetOutgoingMessageType(messageTag);
                if (result == null)
                {
                    throw new UnexpectedMessageException($"Unexpected outgoing message protocol/tag: ({protocolId}, {messageTag})");
                }
                else
                {
                    return result;
                }
            }

            // Returns the index for a given protocol id.
            private ushort GetProtocolId(IProtocolServerSide protocol)
            {
                int index = Array.IndexOf(protocols, protocol);
                if (index == protocols.GetLowerBound(0) - 1)
                {
                    throw new UnknownProtocolException($"The given instance of {protocol.GetType().FullName} is not a component on this object");
                }
                else
                {
                    return (ushort)index;
                }
            }

            // Returns the message tag for the given protocol and message name.
            private ushort GetOutgoingMessageTag(ushort protocolId, string messageName)
            {
                if (protocolId >= protocols.Length)
                {
                    throw new UnexpectedMessageException($"Unexpected outgoing message protocol/name: ({protocolId}, {messageName})");
                }
                ushort? tag = protocols[protocolId].GetOutgoingMessageTag(messageName);
                if (tag == null)
                {
                    throw new UnexpectedMessageException($"Unexpected outgoing message protocol/name: ({protocolId}, {messageName})");
                }
                else
                {
                    return tag.Value;
                }
            }

            // Handles a received message. The received message will be
            // handled by the underlying protocol handler.
            private void HandleMessage(ulong clientId, ushort protocolId, ushort messageTag, ISerializable message)
            {
                // At this point, the protocolId exists. Also, the messageTag exists.
                // We get the client-side handler, and we invoke it.
                Action<ulong, ISerializable> handler = protocols[protocolId].GetIncomingMessageHandler(messageTag);
                if (handler != null)
                {
                    handler(clientId, message);
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
                ZeroProtocolServerSide zeroProtocol = GetComponent<ZeroProtocolServerSide>();
                if (zeroProtocol == null)
                {
                    Destroy(gameObject);
                    throw new MissingZeroProtocol("This NetworkServer does not have a ZeroProtocolServerSide protocol behaviour added - it must have one");
                }
                var protocolList = (from protocolServerSide in GetComponents<IProtocolServerSide>() select (Component)protocolServerSide).ToList();
                protocolList.Remove(zeroProtocol);
                Behaviours.SortByDependencies(protocolList.ToArray()).ToList();
                protocolList.Insert(0, zeroProtocol);
                protocols = (from protocolServerSide in protocolList select (IProtocolServerSide)protocolServerSide).ToArray();
            }

            // This function gets invoked when the network server
            // started. It invokes all of the OnServerStarted
            // handlers on each protocol.
            private void TriggerOnServerStarted()
            {
                foreach (IProtocolServerSide protocol in protocols)
                {
                    try
                    {
                        protocol.OnServerStarted();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("An exception was triggered. Ensure exceptions are captured and handled properly, " +
                                         "for this warning will not be available on deployed games");
                        Debug.LogException(e);
                    }
                }
            }

            // This function gets invoked when a network client
            // successfully connects to this server. It invokes
            // all of the OnConnected handlers on each protocol.
            private void TriggerOnConnected(ulong clientId)
            {
                foreach (IProtocolServerSide protocol in protocols)
                {
                    try
                    {
                        protocol.OnConnected(clientId);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("An exception was triggered. Ensure exceptions are captured and handled properly, " +
                                         "for this warning will not be available on deployed games");
                        Debug.LogException(e);
                    }
                }
            }

            // This function gets invoked when a network client
            // disconnects from this server, be it normally or
            // not. It invokes all of the OnDisconnected handlers
            // on each protocol.
            private void TriggerOnDisconnected(ulong clientId, System.Exception reason)
            {
                foreach (IProtocolServerSide protocol in protocols)
                {
                    try
                    {
                        protocol.OnDisconnected(clientId, reason);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("An exception was triggered. Ensure exceptions are captured and handled properly, " +
                                         "for this warning will not be available on deployed games");
                        Debug.LogException(e);
                    }
                }
            }

            // This function gets invoked when the network server
            // stopped. It invokes all of the OnServerStopped
            // handlers on each protocol.
            private void TriggerOnServerStopped(System.Exception e)
            {
                foreach (IProtocolServerSide protocol in protocols)
                {
                    try
                    {
                        protocol.OnServerStopped(e);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning("An exception was triggered. Ensure exceptions are captured and handled properly, " +
                                         "for this warning will not be available on deployed games");
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}
