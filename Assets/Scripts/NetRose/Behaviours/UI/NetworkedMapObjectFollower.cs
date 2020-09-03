using UnityEngine.Events;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace UI
        {
            using Entities.Objects;

            /// <summary>
            ///   This behaviour tracks a target object across scenes.
            ///     The target object is followed continually and even
            ///     across world scenes.
            /// </summary>
            public class NetworkedObjectFollower : NetworkBehaviour
            {
                [SyncVar]
                private NetworkIdentity target;

                /// <summary>
				///   This event is triggered when the target is
				///     changed.
				/// </summary>
                public class NetworkedMapObjectChangedEvent : UnityEvent<NetworkedMapObject, NetworkedMapObject> {};

                /// <summary>
				///   This event is triggered when the target object
				///     is changed.
				/// </summary>
                public readonly NetworkedMapObjectChangedEvent onTargetChanged = new NetworkedMapObjectChangedEvent();

                // The underlying identity.
                private NetworkIdentity identity;

                private void Awake()
                {
                    identity = GetComponent<NetworkIdentity>();
                }

                /// <summary>
                ///   The object being tracked.
                /// </summary>
                public NetworkedMapObject Target
                {
                    get
                    {
                        return target.GetComponent<NetworkedMapObject>();
                    }
                    set
                    {
                        NetworkedMapObject oldTarget = target ? target.GetComponent<NetworkedMapObject>() : null;
                        target = value.GetComponent<NetworkIdentity>();
                        onTargetChanged.Invoke(oldTarget, value);
                    }
                }

                // Updating the object involves following the related
                // map object's position, rotation and scene.
                private void Update()
                {
                    if (target)
                    {
                        // If server side, move to the object's scene.
                        if (gameObject.scene != target.gameObject.scene)
                        {
                            if (isServer)
                            {
                                NetworkedSceneLayout.Instance.MovePlayer(identity, target.gameObject.scene);
                            }
                        }
                        // Both client and server will run this code on their own:
                        // Update position, update rotation.
                        // Clients will appropriately receive the message to run
                        // the scene change.
                        transform.position = target.transform.position;
                        transform.rotation = target.transform.rotation;
                    }
                    else if (gameObject.scene != NetworkedSceneLayout.Instance.gameObject.scene)
                    {
                        // If server side, move to the main world/"online" scene.
                        if (isServer)
                        {
                            NetworkedSceneLayout.Instance.MovePlayer(identity, NetworkedSceneLayout.Instance.gameObject.scene);
                        }
                    }
                }
            }
        }
    }
}
