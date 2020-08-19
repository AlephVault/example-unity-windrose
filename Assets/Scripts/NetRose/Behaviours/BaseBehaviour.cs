using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using GMM.Utils;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        ///   <para>
        ///     This behaviour acts like a "base" behaviour for both
        ///       the maps and their entities. Subclasses of this
        ///       behaviour will get protected access to the internal
        ///       queue of received ClientRpc-based updates, and will
        ///       populate that queue, and process it, accordingly.
        ///   </para>
        ///   <para>
        ///     Subclasses of this component will have lots of methods
        ///       marked with [ClientRpc] and ways to trigger them.
        ///       Those methods will most likely only process code if
        ///       !isServer, because that code reflects the whole
        ///       (WindRose, BackPack, ...) events and not synchronize
        ///       the transforms or other data via [SyncVar].
        ///   </para>
        /// </summary>
        [RequireComponent(typeof(NetworkSceneChecker))]
        public abstract class BaseBehaviour : NetworkBehaviour
        {
            /// <summary>
            ///   This class is a dependent behaviour. It depends on
            ///   <see cref="BaseBehaviour" /> and also has access to
            ///   its queue.
            /// </summary>
            [RequireComponent(typeof(BaseBehaviour))]
            public abstract class RelatedBehaviour : NetworkBehaviour
            {
                private BaseBehaviour baseBehaviour;

                private void Awake()
                {
                    baseBehaviour = GetComponent<BaseBehaviour>();
                }

                /// <summary>
                ///   Forwards a call to add a command to the queue.
                /// </summary>
                /// <param name="forceAccelerate">If true, it accelerates regardless of the size</param>
                protected void AddToQueue(ClientRpcCommand command, bool forceAccelerate = false)
                {
                    baseBehaviour.AddToQueue(command, forceAccelerate);
                }
            }

            /// <summary>
            ///   Triggered when trying to add an RPC command when the queue is not
            ///     initialized.
            /// </summary>
            public class QueueNotInitializedException : Exception
            {
                public QueueNotInitializedException() { }
                public QueueNotInitializedException(string message) : base(message) { }
                public QueueNotInitializedException(string message, Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   The limit of the movements queue for this object. At minimum, this value
            ///     is <see cref="MIN_QUEUE_LIMIT" />, and when the movement queue passes
            ///     this maximum size, all the movement actions will be forced, not waited.
            /// </summary>
            [SerializeField]
            private uint queueLimit = MIN_QUEUE_LIMIT;

            // This is the minimum size the queue can have.
            private const uint MIN_QUEUE_LIMIT = 3;

            /// <summary>
            ///   The queue to use in client side.
            /// </summary>
            protected Queue<ClientRpcCommand> queue;

            // Tells whether the queue is in accelerated mode.
            // This will happen when adding a new element to
            // the queue when the current queue size is >=
            // {queueLimit}, and will stop when the current
            // queue size reaches 0.
            private bool mustAccelerateQueueProcessing = false;

            private void Awake()
            {
                queueLimit = (queueLimit < MIN_QUEUE_LIMIT) ? MIN_QUEUE_LIMIT : queueLimit;
            }

            /// <summary>
            ///   Starts the queue processing routine when
            ///     spawned in client side.
            /// </summary>
            public override void OnStartClient()
            {
                // The object is started from the server and into the
                // client. If this object is running in "client only"
                // mode, the queue will be created.
                if (!isServer)
                {
                    queue = new Queue<ClientRpcCommand>();
                    RunQueue();
                }
            }

            // A function to dynamically evaluate whether all the queue
            // must be run in accelerated mode or not.
            private bool MustAccelerateQueueProcessing()
            {
                return mustAccelerateQueueProcessing;
            }

            // Runs the whole queue, asynchronously. This is a loop
            // that runs while the object is alive, and processes
            // the whole commands queue.
            private async void RunQueue()
            {
                while(gameObject && NetworkClient.isConnected)
                {
                    if (queue.Count == 0) mustAccelerateQueueProcessing = false;
                    while (queue.Count == 0) await Tasks.Blink();
                    await queue.Peek().Invoke(MustAccelerateQueueProcessing);
                    queue.Dequeue().OnDequeued();
                }
            }

            /// <summary>
            ///   Adds an element to the queue. If the new item is
            ///     added while the queue size is on its limit or
            ///     above that limit, the queue will run in an
            ///     accelerated mode. The [en]queued command, after
            ///     its addition, will invoke its "on enqueued"
            ///     callback.
            /// </summary>
            /// <param name="forceAccelerate">If true, it accelerates regardless of the size</param>
            protected void AddToQueue(ClientRpcCommand command, bool forceAccelerate = false)
            {
                if (command == null) throw new NullReferenceException("Cannot queue a null RPC command");
                if (queue == null) throw new QueueNotInitializedException("Cannot queue an RPC command because the queue is not yet initialized");
                if (forceAccelerate || queue.Count >= queueLimit) mustAccelerateQueueProcessing = true;
                queue.Enqueue(command);
                command.OnEnqueued();
            }
        }
    }
}
