using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using NetRose.Types;
using NetRose.Behaviours.Auth;
using NetRose.Behaviours.UI;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        ///   <para>
        ///     Works with a <see cref="NetworkedWorld"/> behaviour
        ///       that will manage the additive scenes that make
        ///       all the different maps of the game when on the
        ///       online scene.
        ///   </para>
        ///   <para>
        ///     A runtime type check will be done to ensure the
        ///       authenticator being used is of the appropriate
        ///       type (i.e. a "standard" one with the appropriate
        ///       types). Such behaviour can be extracted from the
        ///       same object this behaviour belongs to, or from
        ///       the <see cref="NetworkManager.authenticator"/>
        ///       property.
        ///   </para>
        /// </summary>
        /// <typeparam name="AuthMessage">The type of the auth message to send to the server to perform login</typeparam>
        /// <typeparam name="AccountID">The type of the id for the player's account</typeparam>
        /// <typeparam name="CharacterID">The type of the id for the player's characters</typeparam>
        /// <typeparam name="PreviewCharacterData">The type of the preview data for the player's characters</typeparam>
        /// <typeparam name="FullCharacterData">The type of the full data for the player's characters</typeparam>
        [RequireComponent(typeof(NetworkedWorld))]
        public class NetworkWorldManager<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData> : NetworkManager where AuthMessage : IMessageBase, new()
        {
            /// <summary>
            ///   Triggered when detecting that a bad type of <see cref="NetworkAuthenticator"/>
            ///     is added as a requirement to this component, or when no instance is added.
            /// </summary>
            public class InvalidAuthenticator : Exception
            {
                public InvalidAuthenticator() { }
                public InvalidAuthenticator(string message) : base(message) { }
                public InvalidAuthenticator(string message, System.Exception inner) : base(message, inner) { }
            }

            // The related networked world manager.
            private NetworkedWorld networkedWorld;

            /// <summary>
            ///   This implementation of Awake will consider the authenticator
            ///     as a component and check its type to be a TODO player source
            ///     authenticator.
            /// </summary>
            public override void Awake()
            {
                networkedWorld = GetComponent<NetworkedWorld>();
                transport.OnServerDisconnected.AddListener(OnClientDisconnectedFromServer);
                // Try to extract the authenticator from the current property,
                // or from the same object (as an attached behaviour).
                if (authenticator == null)
                {
                    authenticator = GetComponent<NetworkAuthenticator>();
                }
                // If the authenticator is still null, then this is an error.
                // Also, if not set to an expected type (Standard with the
                // same type parameters) an error is to be triggered.
                if (authenticator == null)
                {
                    throw new InvalidAuthenticator("No authenticator is set/attached to this network behaviour");
                }
                else if (!(authenticator is StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>))
                {
                    throw new InvalidAuthenticator("An incompatible authenticator was assigned. A standard authenticator with the same type parameters must be used");
                }
                else if (playerPrefab == null || playerPrefab.GetComponent<StandardPlayer<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>>() == null)
                {
                    throw new InvalidAuthenticator("An incompatible player prefab was assigned. A prefab with a standard player behaviour with the same type parameters must be used");
                }
                base.Awake();
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                transport.OnServerDisconnected.RemoveListener(OnClientDisconnectedFromServer);
            }

            private void StopAll()
            {
                // Stop client first, so the server cleans up the client.
                if (NetworkClient.isConnected)
                {
                    StopClient();
                }
                // Stop server after stopping client (in particular needed
                // to do in this order for the host game type).
                if (NetworkServer.active)
                {
                    StopServer();
                }
                Destroy(gameObject);
            }

            // When a client disconnects from the transport, then all the
            // entries involving this connection will be removed from the
            // pending players list.
            private void OnClientDisconnectedFromServer(int connectionId)
            {
                networkedWorld.pendingPlayers.RemoveWhere((NetworkConnection connection) =>
                {
                    return connection.connectionId == connectionId;
                });
            }

            /// <summary>
            ///   This hook tests for when the scene is changed to be the online
            ///     one, and in that case it will load all the required scenes
            ///     which are singleton.
            /// </summary>
            /// <param name="sceneName">The new scene (either the online or offline scene)</param>
            public override void OnServerSceneChanged(string sceneName)
            {
                if (sceneName == onlineScene)
                {
                    networkedWorld.InitWorld(base.OnServerAddPlayer, StopAll);
                }
            }

            /// <summary>
            ///   This hook tests for when the scene is being changed (not yet
            ///     changed) to the offline one, and in that case all the loaded
            ///     scenes must be unloaded.
            /// </summary>
            /// <param name="newSceneName">The new scene (either the online or offline scene)</param>
            public override void OnServerChangeScene(string newSceneName)
            {
                networkedWorld.isWorldReady = false;
                if (newSceneName == onlineScene)
                {
                    networkedWorld.UnloadSingletonScenes(StopAll);
                }
            }

            /// <summary>
            ///   The player will be added immediately if the world is ready, but
            ///     if not ready, it will be queued until it is ready.
            /// </summary>
            /// <param name="conn">The connection for which the player must be added</param>
            public override void OnServerAddPlayer(NetworkConnection conn)
            {
                if (networkedWorld.isWorldReady)
                {
                    base.OnServerAddPlayer(conn);
                }
                else
                {
                    networkedWorld.pendingPlayers.Add(conn);
                }
            }

            /// <summary>
            ///   Tells whether the accounts support having several characters or
            ///     each account is its only character.
            /// </summary>
            public bool AllowsMultipleCharactersPerAccount()
            {
                return (authenticator as StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>).AllowsMultipleCharactersPerAccount();
            }

            /// <summary>
            ///   Lists the characters for a given connection. For single-character
            ///     games, the key will be <c>default(CharacterID)</c>, while for
            ///     multi-character (per account) games, no key will be such default
            ///     value. On error, this method will raise <see cref="AccountException"/>.
            ///     On missing authentication data, this method will also raise the
            ///     same type of error. This task is asynchronous and must be waited for.
            /// </summary>
            /// <param name="connection">The connection of the account to get the characters from</param>
            /// <returns>A dictionary with the preview of available characters</returns>
            public async Task<Dictionary<CharacterID, PreviewCharacterData>> ListCharacters(NetworkConnection connection)
            {
                return await (authenticator as StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>).ListCharacters(connection);
            }

            /// <summary>
            ///   Gets the full data for a character in an account, give both the connection
            ///     and the character id as well. For single-character games, the only valid
            ///     character id is <c>default(CharacterID)</c>. For multi-character games,
            ///     no id will be <c>default(CharacterID)</c>. On invalid character id error,
            ///     an <see cref="AccountException"/> will be risen. Custom conditions will
            ///     also trigger an <see cref="AccountException"/> as needed. On missing
            ///     authentication data, this method will also raise the same type of error.
            ///     This task is asynchronous and must be waited for.
            /// </summary>
            /// <param name="accountID">The connection to get a character's full data from</param>
            /// <param name="characterID">The id of the character for which the data is being retrieved</param>
            /// <returns>The full data of the chosen character</returns>
            public async Task<FullCharacterData> Load(NetworkConnection connection, CharacterID characterID)
            {
                return await (authenticator as StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>).Load(connection, characterID);
            }
        }
    }
}
