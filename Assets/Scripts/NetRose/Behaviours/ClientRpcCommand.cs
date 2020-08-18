using System;
using System.Threading.Tasks;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        ///   These RPC commands are created when receiving a ClientRpc-tagged
        ///     call. The contract of an RPC command contains the Invoke
        ///     method that will execute the logic and know when the queue is
        ///     full / it should "accelerate" (update after a huge lag).
        /// </summary>
        public abstract class ClientRpcCommand
        {
            /// <summary>
            ///   Logic to invoke when the command is enqueued.
            /// </summary>
            public virtual void OnEnqueued() {}

            /// <summary>
            ///   Logic to invoke as per command execution.
            /// </summary>
            /// <param name="mustAccelerate">A function that tells whether the execution of the command must accelerate or run normal</param>
            /// <returns></returns>
            public abstract Task Invoke(Func<bool> mustAccelerate);

            /// <summary>
            ///   Logic to invoke when the command is dequeued.
            /// </summary>
            public virtual void OnDequeued() {}
        }
    }
}
