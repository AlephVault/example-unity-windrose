using UnityEngine;
using Mirror;

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
            public override void OnStartClient()
            {
                // The object is started from the server and into the
                // client. If this object is running in "client only"
                // mode, the queue will be created.
            }

            // TODO Add queue stuff & methods here.
            // TODO Also add a class in the same package standing for
            // TODO   the RPC queue.
        }
    }
}
