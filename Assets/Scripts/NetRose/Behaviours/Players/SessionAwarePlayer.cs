﻿using UnityEngine;
using Mirror;
using NetRose.Behaviours.Sessions.Messages;
using UnityEngine.Events;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   Session-aware players are behaviours intended to
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
            ///     forwarding their events.
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
            [RequireComponent(typeof(NetworkIdentity))]
            public class SessionAwarePlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> : MonoBehaviour
                where CCMsg : ChooseCharacter<CharacterID, CharacterPreviewData>, new()
                where UCMsg : UsingCharacter<CharacterID, CharacterFullData>, new()
                where ICMsg : InvalidCharacterID<CharacterID>, new()
                where NCMsg : CharacterDoesNotExist<CharacterID>, new()
            {
                /// <summary>
                ///   This inner listener ensures that listened callbacks are appropriately
                ///     forwarded to the corresponding events.
                /// </summary>
                private class InnerListener : SessionListener<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg>
                {
                    private SessionAwarePlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> target;

                    public InnerListener(SessionAwarePlayer<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> player)
                    {
                        target = player;
                    }

                    public void AccountData()
                    {
                        target.onServerSessionEvent.Invoke("account-data:set", "", null);
                    }

                    public void CustomDataCleared()
                    {
                        target.onServerSessionEvent.Invoke("custom-data:cleared", "", null);
                    }

                    public void CustomDataRemoved(string key)
                    {
                        target.onServerSessionEvent.Invoke("custom-data:removed", key, null);
                    }

                    public void CustomDataSet(string key, object value)
                    {
                        target.onServerSessionEvent.Invoke("custom-data:set", key, value);
                    }

                    public void Started(Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> session)
                    {
                        target.onServerSessionEvent.Invoke("started", "", null);
                    }

                    public void Terminated()
                    {
                        target.onServerSessionEvent.Invoke("terminated", "", null);
                    }

                    public void UsingCharacter()
                    {
                        target.onServerSessionEvent.Invoke("character:set", "", null);
                    }

                    public void UsingNoCharacter()
                    {
                        target.onServerSessionEvent.Invoke("character:cleared", "", null);
                    }
                }

                private InnerListener listener;

                /// <summary>
                ///   This event carries information of the concrete change in the session.
                ///   This involves an event key, and extra fields like custom data key and
                ///     custom data value for certain events.
                /// </summary>
                public class SessionEvent : UnityEvent<string, string, object> {}

                /// <summary>
                ///   This event is triggered on any session event on this object. While
                ///     most events involve actual data change in the session, two of them
                ///     involve adding/removing this object as listener instead: "started"
                ///     and "terminated". For those, the session will still exist around
                ///     them.
                /// </summary>
                public readonly SessionEvent onServerSessionEvent = new SessionEvent();

                private NetworkIdentity identity;
                private Session<AccountID, AccountData, CharacterID, CharacterPreviewData, CharacterFullData, CCMsg, UCMsg, ICMsg, NCMsg> session;

                void Awake()
                {
                    identity = GetComponent<NetworkIdentity>();
                    listener = new InnerListener(this);
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
            }
        }
    }
}