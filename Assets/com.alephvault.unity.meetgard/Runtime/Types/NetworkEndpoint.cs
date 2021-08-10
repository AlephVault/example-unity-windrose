using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        /// <summary>
        ///   <para>
        ///     A network endpoint is an endpoint which can send
        ///     and receive data, and have their own concepts
        ///     of connected/active and events for data arrival.
        ///   </para>
        ///   <para>
        ///     There are two types of network endpoints: standard
        ///     (remote) ones, and host (local) ones. While the
        ///     inmense majority of the endpoints are remote, one
        ///     local endpoint may exist in the server (and WILL
        ///     exist in the server for host/symmetric games).
        ///   </para>
        ///   <para>
        ///     Implementation details: A network endpoint must
        ///     notify, somehow, about the following events:
        ///     connected, disconnected, and message arrival.
        ///   </para>
        /// </summary>
        public abstract class NetworkEndpoint : MonoBehaviour
        {
            /// <summary>
            ///   Tells whether the endpoint is active (i.e.
            ///   running some sort of life-cycle).
            /// </summary>
            public abstract bool IsActive { get; }

            /// <summary>
            ///   Tells whether the endpoint is connected (i.e.
            ///   its socket is connected).
            /// </summary>
            public abstract bool IsConnected { get; }

            /// <summary>
            ///   Closes the connection.
            /// </summary>
            public abstract void Close();

            /// <summary>
            ///   Performs, asynchronously, a data send using
            ///   that metadata and input.
            /// </summary>
            /// <param name="protocolID">The id of protocol for this message</param>
            /// <param name="messageTag">The tag of the message being sent</param>
            /// <param name="input">The input stream</param>
            public abstract Task Send(ushort protocolId, ushort messageTag, Stream input);
        }
    }
}
