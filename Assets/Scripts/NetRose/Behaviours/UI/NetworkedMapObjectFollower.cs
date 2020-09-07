using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        namespace UI
        {
            using Entities.Objects;
            using UnityEngine.SceneManagement;

            /// <summary>
            ///   <para>
            ///     This behaviour tracks a target object across scenes.
            ///       The target object is followed continually and even
            ///       across world scenes. In the client, this will imply
            ///       also a camera being used to follow the player (which
            ///       has this behaviour and follows a target object).
            ///   </para>
            ///   <para>
            ///     This behaviour is intended to be used in player prefab
            ///       objects.
            ///   </para>
            /// </summary>
            public class NetworkedMapObjectFollower : NetworkBehaviour
            {
                // The underlying identity of the target object.
                [SyncVar]
                private NetworkIdentity target;

                /// <summary>
                ///   The camera being tracked. Such camera will
                ///     be the one tracking the target object, if
                ///     any. On client this camera must be explicitly
                ///     set to a camera.
                /// </summary>
                public new Camera camera = null;

                /// <summary>
				///   This event is triggered when the target is
				///     changed.
				/// </summary>
                public class NetworkedMapObjectChangedEvent : UnityEvent<NetworkedMapObject, NetworkedMapObject> {};

                /// <summary>
				///   This event is triggered when the target object
				///     is changed. Possible uses involve considering
                ///     the inventory of the target object.
				/// </summary>
                public readonly NetworkedMapObjectChangedEvent onTargetChanged = new NetworkedMapObjectChangedEvent();

                // The underlying identity. Used to change scenes.
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
                                NetworkManager.singleton.GetComponent<NetworkedWorld>().MovePlayer(identity, target.gameObject.scene);
                            }
                        }
                        // Both client and server will run this code on their own:
                        // Update position, update rotation.
                        // Clients will appropriately receive the message to run
                        // the scene change.
                        transform.position = target.transform.position;
                        transform.rotation = target.transform.rotation;
                    }
                    else if (gameObject.scene != SceneManager.GetActiveScene())
                    {
                        // If server side, move to the main world/"online" scene.
                        if (isServer)
                        {
                            NetworkManager.singleton.GetComponent<NetworkedWorld>().MovePlayer(identity, SceneManager.GetActiveScene());
                        }
                    }

                    // On clients, when the camera is set, it will update its position
                    // based on the follower's position as well. Since additive scenes
                    // are fully overlapped, the camera will have to exist in the world
                    // scene (this will NOT be checked/enforced but should be a rule of
                    // thumb for all games) and its (x, y) position will be updated, but
                    // not its z-position. Its rotation will also be updated.
                    if (isClient && camera)
                    {
                        if (!camera.orthographic) camera.orthographic = true;
                        camera.transform.position = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
                        camera.transform.rotation = transform.rotation;
                    } 
                }
            }
        }
    }
}
