using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Mirror;
using NetRose.Types;
using AlephVault.Unity.Support.Utils;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        /// 
        /// </summary>
        public class NetworkWorldManager : NetworkManager
        {
            /// <summary>
            ///   Triggered when trying to load a singleton scene in
            ///     the <see cref="Load"/> method.
            /// </summary>
            public class CannotLoadSingletonScenes : Exception
            {
                public CannotLoadSingletonScenes() { }
                public CannotLoadSingletonScenes(string message) : base(message) { }
                public CannotLoadSingletonScenes(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to load an invalid scene.
            ///     Either the scene is invalid (not added at
            ///     build time) or the scene key is invalid.
            /// </summary>
            public class InvalidScene : Exception
            {
                public InvalidScene() { }
                public InvalidScene(string message) : base(message) { }
                public InvalidScene(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to unload a scene which still
            ///     has players watching it.
            /// </summary>
            public class SceneNotEmpty : Exception
            {
                public SceneNotEmpty() { }
                public SceneNotEmpty(string message) : base(message) { }
                public SceneNotEmpty(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to run a method involving scenes / players
            ///     manipulations while the world is not ready (i.e. offline or
            ///     still loading the base scenes).
            /// </summary>
            public class WorldNotReady : Exception
            {
                public WorldNotReady() { }
                public WorldNotReady(string message) : base(message) { }
                public WorldNotReady(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to run a method involving scenes / players
            ///     manipulations in a non-server context.
            /// </summary>
            public class OnlyAvailableInServer : Exception
            {
                public OnlyAvailableInServer() { }
                public OnlyAvailableInServer(string message) : base(message) { }
                public OnlyAvailableInServer(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to move a player object with inactive connection.
            /// </summary>
            public class InactiveConnectionException : Exception
            {
                public InactiveConnectionException() { }
                public InactiveConnectionException(string message) : base(message) { }
                public InactiveConnectionException(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to move a non-player across the scene layout.
            /// </summary>
            public class NoPlayerException : Exception
            {
                public NoPlayerException() { }
                public NoPlayerException(string message) : base(message) { }
                public NoPlayerException(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to move a player object to a scene that is not loaded.
            /// </summary>
            public class SceneNotLoadedException : Exception
            {
                public SceneNotLoadedException() { }
                public SceneNotLoadedException(string message) : base(message) { }
                public SceneNotLoadedException(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   The list of scenes to load. Those scenes can be either
            ///     template or singleton scenes. When loaded, those scenes
            ///     will be identified by their key (if singleton) or by
            ///     a GUID (if template).
            /// </summary>
            [SerializeField]
            private SceneConfigDictionary scenes = new SceneConfigDictionary();

            // These are the singleton scenes that are successfully loaded.
            // They are identified by their key (the same keys in the
            // scenes configuration).
            private Dictionary<string, Scene> singletonLoadedScenes = new Dictionary<string, Scene>();

            // These are the template-instantiated scenes that are successfully
            // loaded. They are identified by a GUID which is generated when one
            // of these scenes is successfully loaded.
            private Dictionary<string, Scene> templateLoadedScenes = new Dictionary<string, Scene>();

            /// <summary>
            ///   Tells when the world is ready to be manipulated or not. This stands for the
            ///     server being online and also the base scenes being fully loaded in scene's
            ///     memory (i.e. those scenes that are "singleton" were successfully loaded).
            /// </summary>
            public bool isWorldReady { get; private set; }

            // This is the list of players being added. When a player is added while the world
            // is not ready (this will most likely happen while the scenes are being loaded)
            // the connection will be queued instead of immediately being added. When the world
            // is ready, this list will empty and all the players in it will be immediately
            // added to the game.
            private HashSet<NetworkConnection> pendingPlayers = new HashSet<NetworkConnection>();

            // Tracks the position of each player as identity => scene. The scene is a valid
            // scene among the loaded ones and the online scene.
            private Dictionary<NetworkIdentity, Scene> playerScenes = new Dictionary<NetworkIdentity, Scene>();

            // Tracks all the players in a scene as scene => set of identity.
            private Dictionary<Scene, HashSet<NetworkIdentity>> scenePlayers = new Dictionary<Scene, HashSet<NetworkIdentity>>();

            // Preloads all the singleton scenes.
            private async Task PreloadScenes()
            {
                foreach (KeyValuePair<string, SceneConfig> pair in scenes)
                {
                    if (pair.Value.LoadMode == SceneLoadMode.Singleton)
                    {
                        singletonLoadedScenes[pair.Key] = await pair.Value.Load();
                    }
                }
            }

            // Initializes all the world (this loads all the singleton scenes
            // and also dequeues all the queued players).
            private async void InitWorld()
            {
                try
                {
                    await PreloadScenes();
                    isWorldReady = true;
                    foreach (NetworkConnection connection in pendingPlayers)
                    {
                        base.OnServerAddPlayer(connection);
                        RefreshPlayerTrack(connection.identity);
                    }
                }
                catch (System.Exception)
                {
                    StopAll();
                }
            }

            // Unloads all the singleton and template scenes.
            private async Task UnloadScenes()
            {
                foreach (KeyValuePair<string, Scene> pair in singletonLoadedScenes)
                {
                    AsyncOperation operation = SceneManager.UnloadSceneAsync(pair.Value);
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                }
                singletonLoadedScenes.Clear();
                foreach (KeyValuePair<string, Scene> pair in templateLoadedScenes)
                {
                    AsyncOperation operation = SceneManager.UnloadSceneAsync(pair.Value);
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                }
                templateLoadedScenes.Clear();
            }

            // Stops all the world by making it not ready, moving all the
            // players to the active scene, and then unloading all the scenes.
            private async void EndWorld()
            {
                try
                {
                    isWorldReady = false;
                    await UnloadScenes();
                    // TODO remove players
                }
                catch (System.Exception)
                {
                    StopAll();
                }
            }

            // Stops the network client, the network server, and destroys
            // this instance of network manager.
            private void StopAll()
            {
                if (mode == NetworkManagerMode.ServerOnly)
                {
                    StopServer();
                }
                else if (mode == NetworkManagerMode.Host)
                {
                    StopHost();
                }
                else if (mode == NetworkManagerMode.ClientOnly)
                {
                    StopClient();
                }
                Destroy(gameObject);
            }

            // Refreshes the player's current track in the internal
            // tracking variables.
            private void RefreshPlayerTrack(NetworkIdentity player)
            {
                Scene scene = player.gameObject.scene;
                if (scene == gameObject.scene || ContainsScene(scene))
                {
                    Scene oldScene;
                    HashSet<NetworkIdentity> players;

                    // If the player is in a scene, it will also exist
                    // among the scenePlayers[scene], and must be popped
                    // out.
                    if (playerScenes.TryGetValue(player, out oldScene))
                    {
                        scenePlayers[oldScene].Remove(player);
                    }

                    // The new scene is then set to the player.
                    playerScenes[player] = scene;

                    // And finally the player is added to the new scene.
                    // This implies creating the players set, for that
                    // new scene, if absent and then add the player to
                    // that set.
                    if (!scenePlayers.TryGetValue(scene, out players))
                    {
                        players = new HashSet<NetworkIdentity>();
                        scenePlayers[scene] = players;
                    }
                    players.Add(player);
                }
            }

            // Clears the player's current track from the internal
            // variables.
            private void ClearPlayerTrack(NetworkIdentity player)
            {
                Scene oldScene;

                if (playerScenes.TryGetValue(player, out oldScene))
                {
                    scenePlayers[oldScene].Remove(player);
                    playerScenes.Remove(player);
                }
            }

            /* ************************* Public methods will start here *********************** */

            /// <summary>
            ///   Loads a scene, identified by its key. If the key does not
            ///     exist, an invalid scene will be returned. Singleton scenes
            ///     are preloaded and thus cannot be loaded in this way.
            ///     This is an asynchronous task that must be waited for.
            ///     A GUID reference is then returned, instead of returning
            ///     the actual scene.
            /// </summary>
            /// <param name="sceneKey">The scene key to load</param>
            /// <returns>The loaded scene's guid</returns>
            public async Task<string> Load(string sceneKey)
            {
                if (!NetworkServer.active) throw new OnlyAvailableInServer("Cannot invoke Load method in a non-server context");

                if (!isWorldReady) throw new WorldNotReady("Cannot invoke Load method when the world scenes are not ready");

                SceneConfig config;
                if (scenes.TryGetValue(sceneKey, out config))
                {
                    if (config.LoadMode == SceneLoadMode.Singleton)
                    {
                        throw new CannotLoadSingletonScenes("Cannot load a singleton scene - they are already preloaded on world startup");
                    }
                    string guid = System.Guid.NewGuid().ToString();
                    templateLoadedScenes[guid] = await config.Load();
                    return guid;
                }
                else
                {
                    throw new InvalidScene("Invalid scene key: " + sceneKey);
                }
            }

            /// <summary>
            ///   Unloads a template loaded scene, identified by its guid.
            ///     If there is no loaded template scene under that guid
            ///     or the scene has players on it, then an error will
            ///     occur. This is an asynchronous task that must be waited
            ///     for (in contrast to <see cref="Load"/> method, waiting
            ///     this task yields no result).
            /// </summary>
            /// <param name="guid">The guid of the scene to unload</param>
            public async Task Unload(string guid)
            {
                if (!NetworkServer.active) throw new OnlyAvailableInServer("Cannot invoke Unload method in a non-server context");

                if (!isWorldReady) throw new WorldNotReady("Cannot invoke Unload method when the world scenes are not ready");

                Scene scene;
                if (templateLoadedScenes.TryGetValue(guid, out scene))
                {
                    HashSet<NetworkIdentity> players;
                    if (scenePlayers.TryGetValue(scene, out players) && players.Count > 0)
                    {
                        throw new SceneNotEmpty("Cannot unload the scene while there are players watching it");
                    }

                    AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                    templateLoadedScenes.Remove(guid);
                    scenePlayers.Remove(scene);
                }
                else
                {
                    throw new InvalidScene("Cannot unload a template scene by a non-existing key: " + guid);
                }
            }

            // Server-side: Checks whether the object is a player
            // for the current connection.
            private bool IsPlayer(NetworkIdentity identity)
            {
                return identity.connectionToClient.identity == identity;
            }

            // Server-side: Checks whether the object is networked
            // and active.
            private bool IsActiveConnection(NetworkIdentity identity)
            {
                return identity.connectionToClient.isReady;
            }

            /// <summary>
            ///   Moves a player across different scenes. The target scene
            ///     must already be loaded, and it is identified by a string
            ///     key (which is the scene key or guid). Fore more help, see
            ///     <see cref="MovePlayer(NetworkIdentity, Scene)"/>.
            /// </summary>
            /// <param name="identity">The player object to move</param>
            /// <param name="newScene">The key or guid of the target scene to move the object to</param>
            public void MovePlayer(NetworkIdentity identity, string newScene)
            {
                if (newScene == null)
                {
                    MovePlayer(identity, gameObject.scene);
                }
                else
                {
                    Scene scene;
                    if (templateLoadedScenes.TryGetValue(newScene, out scene) || templateLoadedScenes.TryGetValue(newScene, out scene))
                    {
                        MovePlayer(identity, scene);
                    }
                    else
                    {
                        throw new InvalidScene("The target scene key does not belong to a singleton nor template loaded scene: " + newScene);
                    }
                }
            }

            /// <summary>
            ///   Moves a player across different scenes. The target
            ///     scene must be already loaded, and the source scene
            ///     will be. Either scene must be a valid and additively
            ///     loaded scene, or the main scene (which contains this
            ///     world object). The case for the new scene is that
            ///     it must exist among the values of either singleton
            ///     or template loaded scenes.
            /// </summary>
            /// <param name="identity">The player object to move</param>
            /// <param name="newScene">The target scene to move the object to</param>
            public void MovePlayer(NetworkIdentity identity, Scene newScene)
            {
                if (!NetworkServer.active) throw new OnlyAvailableInServer("Cannot invoke MovePlayer method in a non-server context");

                if (!isWorldReady) throw new WorldNotReady("Cannot invoke MovePlayer method when the world scenes are not ready");

                if (!newScene.IsValid())
                {
                    newScene = gameObject.scene;
                }

                if (!IsPlayer(identity))
                {
                    throw new NoPlayerException("Cannot move a network identity, across scenes, not being a client player object");
                }
                else if (!IsActiveConnection(identity))
                {
                    throw new InactiveConnectionException("Cannot move a network identity, across scenes, with an inactive connection");
                }
                else if (!ContainsScene(newScene) && newScene != gameObject.scene)
                {
                    throw new SceneNotLoadedException("Cannot move a network identity, across scenes, to a target scene that does not belong to this world manager");
                }
                else if (newScene == identity.gameObject.scene)
                {
                    // Nothing to do here, except perhaps for a notify / log
                }
                else if (newScene == gameObject.scene)
                {
                    // The current additive scene must be unloaded from the client.
                    identity.connectionToClient.Send(new SceneMessage { sceneName = identity.gameObject.scene.name, sceneOperation = SceneOperation.UnloadAdditive });
                    // The player must be moved to the parent scene.
                    SceneManager.MoveGameObjectToScene(identity.gameObject, newScene);
                    RefreshPlayerTrack(identity);
                    // The player will be, in the end, refreshed into the parent scene.
                }
                else if (identity.gameObject.scene == gameObject.scene)
                {
                    // The player must be moved to the new scene.
                    SceneManager.MoveGameObjectToScene(identity.gameObject, newScene);
                    RefreshPlayerTrack(identity);
                    // The new scene must be loaded into the client.
                    identity.connectionToClient.Send(new SceneMessage { sceneName = newScene.name, sceneOperation = SceneOperation.LoadAdditive });
                    // The player will be, in the end, refreshed into the new scene.
                }
                else
                {
                    // The current additive scene must be unloaded from the client.
                    identity.connectionToClient.Send(new SceneMessage { sceneName = identity.gameObject.scene.name, sceneOperation = SceneOperation.UnloadAdditive });
                    // The player must be moved to the new scene.
                    SceneManager.MoveGameObjectToScene(identity.gameObject, newScene);
                    RefreshPlayerTrack(identity);
                    // The new scene must be loaded into the client.
                    identity.connectionToClient.Send(new SceneMessage { sceneName = newScene.name, sceneOperation = SceneOperation.LoadAdditive });
                    // The player will be, in the end, refreshed into the new scene.
                }
            }

            /// <summary>
            ///   Checks whether a scene belongs to this world or not.
            /// </summary>
            /// <param name="scene">The scene to check</param>
            /// <returns>Whether is among the template loaded or the singleton loaded scenes</returns>
            public bool ContainsScene(Scene scene)
            {
                return singletonLoadedScenes.ContainsValue(scene) || templateLoadedScenes.ContainsValue(scene);
            }

            /// <summary>
            ///   Event class to forward what happens to a connection.
            /// </summary>
            public class ConnectionEvent : UnityEvent<NetworkConnection> {};

            /// <summary>
            ///   This event triggers when this manager starts in server
            ///     mode (which includes host mode).
            /// </summary>
            public readonly UnityEvent onServerStart = new UnityEvent();

            /// <summary>
            ///   This event triggers when this manager stops in server
            ///     mode (which includes host mode).
            /// </summary>
            public readonly UnityEvent onServerStop = new UnityEvent();

            /// <summary>
            ///   This event triggers when this manager starts in client
            ///     mode (which includes host mode).
            /// </summary>
            public readonly UnityEvent onClientStart = new UnityEvent();

            /// <summary>
            ///   This event triggers when this manager stops in client
            ///     mode (which includes host mode).
            /// </summary>
            public readonly UnityEvent onClientStop = new UnityEvent();

            /// <summary>
            ///   This event triggers when a new connection is established.
            ///     This happens right after authentication, if any
            ///     authenticator is set to this network manager, and
            ///     before the client sends the message to create the
            ///     player (which creates the player and refreshes the
            ///     occupancy state for the first time).
            /// </summary>
            public readonly ConnectionEvent onConnected = new ConnectionEvent();

            /// <summary>
            ///   This event triggers when a connection is terminated.
            ///     The occupancy state is cleared, the player identity
            ///     is destroyed, and finally this event is triggered.
            /// </summary>
            public readonly ConnectionEvent onDisconnected = new ConnectionEvent();

            /* ************************* Events will start here *********************** */

            public override void Awake()
            {
                base.Awake();
                isWorldReady = false;
            }

            /// <summary>
            ///   Invokes the <see cref="onServerStart"/> event to notify
            ///     that this network manager started as server.
            /// </summary>
            public override void OnStartServer()
            {
                onServerStart.Invoke();
            }

            /// <summary>
            ///   Invokes the <see cref="onServerStop"/> event to notify
            ///     that this network manager stopped as server.
            /// </summary>
            public override void OnStopServer()
            {
                onServerStop.Invoke();
            }

            /// <summary>
            ///   Invokes the <see cref="onClientStart"/> event to notify
            ///     that this network manager started as client.
            /// </summary>
            public override void OnStartClient()
            {
                onClientStart.Invoke();
            }

            /// <summary>
            ///   Invokes the <see cref="onClientStop"/> event to notify
            ///     that this network manager stopped as client.
            /// </summary>
            public override void OnStopClient()
            {
                onClientStop.Invoke();
            }

            /// <summary>
            ///   Invokes the <see cref="onConnected"/> event to notify
            ///     the initialization of the connection.
            /// </summary>
            /// <param name="conn">The connection being started</param>
            public override void OnServerConnect(NetworkConnection conn)
            {
                onConnected.Invoke(conn);
            }

            /// <summary>
            ///   Removes the connection also from the pending players set.
            ///     Also the player must be popped out from the tracked state.
            ///     Then the player will be destroyed, and implementations
            ///     must attend OnStopServer to perform the related cleanups.
            ///     In the end, it invokes the <see cref="onDisconnected"/>
            ///     event to notify the termination of the connection.
            /// </summary>
            /// <param name="conn">The connection being terminated</param>
            public override void OnServerDisconnect(NetworkConnection conn)
            {
                if (conn.identity) ClearPlayerTrack(conn.identity);
                pendingPlayers.Remove(conn);
                base.OnServerDisconnect(conn);
                onDisconnected.Invoke(conn);
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
                    InitWorld();
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
                if (newSceneName == onlineScene)
                {
                    EndWorld();
                }
            }

            /// <summary>
            ///   The player will be added immediately if the world is ready, but
            ///     if not ready, it will be queued until it is ready.
            /// </summary>
            /// <param name="conn">The connection for which the player must be added</param>
            public override void OnServerAddPlayer(NetworkConnection conn)
            {
                if (isWorldReady)
                {
                    base.OnServerAddPlayer(conn);
                    RefreshPlayerTrack(conn.identity);
                }
                else
                {
                    pendingPlayers.Add(conn);
                }
            }
        }
    }
}
