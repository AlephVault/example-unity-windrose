using UnityEngine;
using Mirror;
using NetRose.Behaviours.Sessions.Messages;
using NetRose.Behaviours.UI;
using NetRose.Behaviours.Entities.Objects;
using NetworkedSamples.Behaviours;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   Standard (aware) players are behaviours intended to
            ///     be part of player objects and have behaviours
            ///     and methods to be aware of the current connection's
            ///     session. This logic, however, is only server-side,
            ///     while for clients it will always hold a dummy
            ///     behaviour. Players will exist in the context of
            ///     a Network Manager being active (either as client,
            ///     server, or host mode), and this player object
            ///     will recognize such condition as well and,
            ///     depending on the networking mode, will execute
            ///     or not its logic: being aware of the session and
            ///     appropriately implementing their logic (partially,
            ///     and mostly abstract). Several methods may be
            ///     implemented / overriden, but those involving the
            ///     character inflation and disposal are required to
            ///     be implemented since they are the core of this
            ///     behaviour.
            /// </summary>
            /// <typeparam name="AccountID">The type of the id of an account</typeparam>
            /// <typeparam name="AccountData">The type of the data of an account</typeparam>
            /// <typeparam name="CharacterID">The type of the id of a character</typeparam>
            /// <typeparam name="CharacterPreviewData">The type of the partial preview data of a character</typeparam>
            /// <typeparam name="CharacterFullData">The type of the full data of a character</typeparam>
            /// <typeparam name="CCMsg">The desired subtype of <see cref="ChooseCharacter{CharacterID, CharacterPreviewData}"/> to handle</typeparam>
            /// <typeparam name="UCMsg">The desired subtype of <see cref="UsingCharacter{CharacterID, CharacterFullData}"/> to handle</typeparam>
            /// <typeparam name="ICMsg">The desired subtype of <see cref="InvalidCharacterID{CharacterID}"/> to handle</typeparam>
            /// <typeparam name="NCMsg">The desired subtype of <see cref="CharacterDoesNotExist{CharacterID}"/> to handle</typeparam>
            [RequireComponent(typeof(NetworkedMapObjectFollower))]
            public abstract class StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> : MonoBehaviour
                where CCMsg : ChooseCharacter<CharacterID, CharacterPreviewData>, new()
                where UCMsg : UsingCharacter<CharacterID, CharacterFullData>, new()
                where ICMsg : InvalidCharacterID<CharacterID>, new()
                where NCMsg : CharacterDoesNotExist<CharacterID>, new()
            {
                /// <summary>
                ///   This inner listener ensures that listened callbacks are appropriately
                ///     forwarded to the corresponding protected methods in the player class.
                /// </summary>
                private class InnerListener : SessionListener<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>
                {
                    private StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> target;

                    public InnerListener(StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> player)
                    {
                        target = player;
                    }

                    public void AccountData()
                    {
                        target.OnAccountDataSet();
                    }

                    public void CustomDataCleared()
                    {
                        target.OnCustomDataCleared();
                    }

                    public void CustomDataRemoved(string key)
                    {
                        target.OnCustomDataRemoved(key);
                    }

                    public void CustomDataSet(string key, object value)
                    {
                        target.OnCustomDataSet(key, value);
                    }

                    public void Started(Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> session)
                    {
                        target.OnAttachedToSession();
                    }

                    public void Terminated()
                    {
                        target.OnDetachedFromSession();
                    }

                    public void UsingCharacter()
                    {
                        target.OnUsingCharacter();
                    }

                    public void UsingNoCharacter()
                    {
                        target.OnUsingNoCharacter();
                    }
                }

                private InnerListener listener;
                private NetworkIdentity identity;

                /// <summary>
                ///   A reference to the map object follower behaviour
                ///     to be used in children classes.
                /// </summary>
                protected NetworkedMapObjectFollower follower;

                private Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> session;

                /// <summary>
                ///   A reference to the current character instance,
                ///     if any character is already loaded, to be used
                ///     by children player behaviours.
                /// </summary>
                protected NetworkedMapObject currentCharacter;

                // Registers a callback for the local player event so these
                // objects know whether they are the local player or not,
                // and react accordingly to different settings.
                static StandardPlayer()
                {
                    ClientScene.onLocalPlayerChanged += OnLocalPlayerChanged;
                }

                // This change handler triggers two callbacks for these objects
                // being set into (and removed from) being the local player.
                private static void OnLocalPlayerChanged(NetworkIdentity oldPlayer, NetworkIdentity newPlayer)
                {
                    StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> cmp = null;
                    if (oldPlayer && (cmp = oldPlayer.GetComponent<StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>>())) cmp.OnStoppedBeingLocalPlayer();
                    if (newPlayer && (cmp = newPlayer.GetComponent<StandardPlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>>())) cmp.OnStartedBeingLocalPlayer();
                }

                protected virtual void Awake()
                {
                    identity = GetComponent<NetworkIdentity>();
                    listener = new InnerListener(this);
                    follower = GetComponent<NetworkedMapObjectFollower>();
                }

                void Start()
                {
                    // Getting the manager, which must be active and an instance
                    // of network world manager, and have a session manager. This
                    // whole prep is only safe to be done in the Start() method
                    // since otherwise it could collide in priority with the
                    // Awake() method in the NetworkManager and not be yet ready.

                    NetworkWorldManager manager = NetworkManager.singleton.GetComponent<NetworkWorldManager>();
                    if (manager == null)
                    {
                        Destroy(gameObject);
                        throw new Exception("The NetworkManager singleton must be of type NetworkWorldManager");
                    }
                    SessionManager<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> sessionManager = manager.GetComponent<SessionManager<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>>();
                    if (sessionManager == null)
                    {
                        Destroy(gameObject);
                        throw new System.Exception("The NetworkManager singleton must be of type SessionManager of compatible generic types");
                    }

                    switch (manager.mode)
                    {
                        case NetworkManagerMode.Offline:
                            Destroy(gameObject);
                            throw new System.Exception("The network manager singleton must be online for this component to work");
                        case NetworkManagerMode.ClientOnly:
                            if (!identity.isClient)
                            {
                                Destroy(gameObject);
                                throw new System.Exception("The current player object must be a live client/server identity");
                            }
                            break;
                        default:
                            // In both host and server, a session will be checked
                            // for existence. If the session does not exist, then
                            // an error will be raised. Otherwise the session will
                            // be kept and the lifecycle will start.
                            session = sessionManager.GetSession(identity.connectionToClient);
                            if (session == null)
                            {
                                Destroy(gameObject);
                                throw new System.Exception("There current player object needs an active session in the underlying connection to work");
                            }
                            break;
                    }

                    // And THEN the startup code is to be run.

                    switch (NetworkManager.singleton.GetComponent<NetworkWorldManager>().mode)
                    {
                        case NetworkManagerMode.ServerOnly:
                        case NetworkManagerMode.Host:
                            session.AddListener(listener);
                            break;
                    }
                }

                void OnDestroy()
                {
                    // The mode will not change across the life of this object.
                    // If it was client, server or host on start, will keep the
                    // same mode on destroy. Otherwise this object would have
                    // been destroyed during the mode change. So we have the
                    // guarantee that we're closing with the same mode than we
                    // opened with.

                    switch (NetworkManager.singleton.GetComponent<NetworkWorldManager>().mode)
                    {
                        case NetworkManagerMode.ServerOnly:
                        case NetworkManagerMode.Host:
                            session.RemoveListener(listener);
                            break;
                    }
                }

                /// <summary>
                ///   Gets a camera to be used by this player's follower.
                /// </summary>
                protected virtual Camera GetCamera()
                {
                    return Camera.main;
                }

                /// <summary>
                ///   Callback for when the object becomes the local player.
                /// </summary>
                protected virtual void OnStartedBeingLocalPlayer()
                {
                    follower.camera = GetCamera();
                }

                /// <summary>
                ///   Callback for when the objects stops being the local player.
                /// </summary>
                protected virtual void OnStoppedBeingLocalPlayer()
                {
                    follower.camera = null;
                }

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-added to
                ///     listen the underlying session.
                /// </summary>
                protected virtual void OnAttachedToSession() {}

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-removed
                ///     from listening the underlying session.
                /// </summary>
                protected virtual void OnDetachedFromSession() {}

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-told that
                ///     the account data was first-obtained or refreshed
                ///     in the session.
                /// </summary>
                protected virtual void OnAccountDataSet() {}

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-told that
                ///     a key in the session custom data was set.
                /// </summary>
                /// <param name="key">The key</param>
                /// <param name="value">Its value</param>
                protected virtual void OnCustomDataSet(string key, object value) {}

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-told that
                ///     a key in the session custom data was removed.
                /// </summary>
                /// <param name="key">The key</param>
                protected virtual void OnCustomDataRemoved(string key) {}

                /// <summary>
                ///   This method, if overriden, may be used to reflect
                ///     what happens when this player is just-told that
                ///     the session custom data was cleared.
                /// </summary>
                /// <param name="key">The key</param>
                protected virtual void OnCustomDataCleared() {}

                /// <summary>
                ///   This method must be overriden to instantiate a
                ///     character object (a networked map object) based
                ///     on the current character data in the session.
                /// </summary>
                /// <returns>A new instance of a networked map object for the current character data</returns>
                protected abstract NetworkedMapObject InstantiateCharacter();

                /// <summary>
                ///   This method must be overriden to dispose the current
                ///     character object. This may include back-refreshing
                ///     its data to the session or even a persistent storage.
                /// </summary>
                protected abstract void DisposeCharacter(NetworkedMapObject character);

                protected void OnUsingCharacter()
                {
                    // Instantiates the character and it becomes actively
                    // tracked by the follower behaviour.
                    currentCharacter = InstantiateCharacter();
                    follower.Target = currentCharacter;
                }

                protected void OnUsingNoCharacter()
                {
                    // Disposes the character (this may involve back-saving
                    // the data to the session and perhaps even persisting
                    // it to an external storage) and clears it from the
                    // internal variable and follower target.
                    DisposeCharacter(currentCharacter);
                    currentCharacter = null;
                    follower.Target = null;
                }
            }
        }
    }
}