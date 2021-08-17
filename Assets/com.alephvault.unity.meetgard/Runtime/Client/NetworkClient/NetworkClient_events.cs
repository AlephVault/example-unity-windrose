using System;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Client
    {
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
            ///     This event is triggered on disconnection. An Exception argument tells
            ///     whether the disconnection was graceful or not: by having a null
            ///     value, it was a graceful termination.
            ///   </para>
            ///   <para>
            ///     This event is triggered in an asynchronous context.
            ///   </para>
            /// </summary>
            public event Action<Exception> OnDisconnected = null;

            // Triggers the OnConnected event. This occurs inside an asynchronous
            // context, already.
            private void TriggerOnConnected()
            {
                OnConnected?.Invoke();
            }

            // Triggers the OnDisconnected event. This occurs inside an asynchronous
            // context, already. Before triggering, it releases the current value in
            // the endpoint variable.
            private void TriggerOnDisconnected(Exception e)
            {
                endpoint = null;
                OnDisconnected?.Invoke(e);
            }
        }
    }
}
