using System;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
        using AlephVault.Unity.Binary;

        public partial class NetworkClient : MonoBehaviour
        {
            /// <summary>
            ///   <para>
            ///     This event is triggered on successful connection.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action OnConnected = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered when a new message arrives.
            ///     PLEASE NOTE: ONLY ONE HANDLER SHOULD HANDLE THE INCOMING MESSAGE, AND IT
            ///     SHOULD EXHAUST THE BUFFER COMPLETELY.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<ushort, ushort, ISerializable> OnMessage = null;

            /// <summary>
            ///   <para>
            ///     This event is triggered on disconnection. An Exception argument tells
            ///     whether the disconnection was graceful or not: by having a null
            ///     value, it was a graceful termination.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<System.Exception> OnDisconnected = null;

            // Triggers the OnConnected event. This occurs inside an asynchronous
            // context, already.
            private void TriggerOnConnected()
            {
                OnConnected?.Invoke();
            }

            // Triggers the OnMessage event. This occurs inside an asynchronous
            // context, already.
            private void TriggerOnMessage(ushort protocolId, ushort messageTag, ISerializable content)
            {
                OnMessage?.Invoke(protocolId, messageTag, content);
            }

            // Triggers the OnDisconnected event. This occurs inside an asynchronous
            // context, already. Before triggering, it releases the current value in
            // the endpoint variable.
            private void TriggerOnDisconnected(System.Exception e)
            {
                endpoint = null;
                OnDisconnected?.Invoke(e);
            }
        }
    }
}
