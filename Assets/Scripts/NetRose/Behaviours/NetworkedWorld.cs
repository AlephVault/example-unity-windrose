using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetRose.Types;
using Mirror;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        ///   <para>
        ///     This single behavior, when on the online scene,
        ///       will also keep a track of several additive
        ///       scenes (the main scene will not have usage
        ///       on its own, save for client-side background
        ///       and UI elements).
        ///   </para>
        ///   <para>
        ///     It has several features involving the underlying
        ///       scenes, like loading and unloading additive
        ///       scenes and moving players across those scenes.
        ///   </para>
        ///   <para>
        ///     When going offline, all the scenes will be unloaded
        ///       before moving to the offline scene. When going
        ///       online, all the scenes will be first loaded and
        ///       THEN the queued players will be added to the
        ///       scene to complete their usual lifecycle.
        ///   </para>
        /// </summary>
        public class NetworkedWorld : MonoBehaviour
        {
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
            ///   Tells when the world is ready to be manipulated or not. This stands for the
            ///     server being online and also the base scenes being fully loaded in scene's
            ///     memory.
            /// </summary>
            public bool isWorldReady { get; internal set; }

            // This is the list of players being added. When a player is added while the world
            // is not ready (this will most likely happen while the scenes are being loaded)
            // the connection will be queued instead of immediately being added. When the world
            // is ready, this list will empty and all the players in it will be immediately
            // added to the game.
            internal HashSet<NetworkConnection> pendingPlayers = new HashSet<NetworkConnection>();

            /// <summary>
            ///   The list of scenes to load. Those scenes can be either
            ///     template or singleton scenes. Template scenes will
            ///     usually not be included here, but all the singleton
            ///     scenes in a game typically will.
            /// </summary>
            [SerializeField]
            SceneConfigDictionary scenes = new SceneConfigDictionary();

            public void Awake()
            {
                isWorldReady = false;
            }

            // Preloads all the singleton scenes.
            private async Task PreloadSingletonScenes(System.Action stopAll)
            {
                try
                {
                    foreach (KeyValuePair<string, SceneConfig> pair in scenes)
                    {
                        if (pair.Value.LoadMode == SceneLoadMode.Singleton)
                        {
                            await pair.Value.Load();
                        }
                    }
                }
                catch (System.Exception)
                {
                    stopAll();
                }
            }

            /// <summary>
            ///   Initializes the world and runs an initializer on each
            ///     client connection.
            /// </summary>
            /// <param name="addPending">The callback to run for each pending connection</param>
            internal async void InitWorld(System.Action<NetworkConnection> addPending, System.Action stopAll)
            {
                await PreloadSingletonScenes(stopAll);
                isWorldReady = true;
                foreach (NetworkConnection connection in pendingPlayers)
                {
                    addPending(connection);
                }
            }

            // Unloads all the singleton scenes. This is only useful to clear all
            // the references, for the scenes per se will be deleted anyway on
            // active scene change (i.e. after this event).
            internal async void UnloadSingletonScenes(System.Action stopAll)
            {
                try
                {
                    foreach (KeyValuePair<string, SceneConfig> pair in scenes)
                    {
                        await pair.Value.Unload();
                    }
                }
                catch (System.Exception)
                {
                    stopAll();
                }
            }

            /// <summary>
            ///   Loads a scene, identified by its key. If the key does not
            ///     exist, an invalid scene will be returned. Depending on the
            ///     scene mode, an existing scene instance will be returned or
            ///     a new scene instance will be created. This is an asynchronous
            ///     task that must be waited for. A reference to the loaded
            ///     <see cref="Scene"/> is then returned, either existing or
            ///     instantiated.
            /// </summary>
            /// <param name="sceneKey">The scene key to load</param>
            /// <returns>The loaded scene</returns>
            public async Task<Scene> Load(string sceneKey)
            {
                if (!NetworkServer.active) throw new OnlyAvailableInServer("Cannot invoke Load method in a non-server context");

                if (!isWorldReady) throw new WorldNotReady("Cannot invoke MovePlayer method when the world scenes are not ready");

                SceneConfig config;
                Scene scene = new Scene { };
                if (scenes.TryGetValue(sceneKey, out config))
                {
                    return await config.Load();
                }
                else
                {
                    return scene;
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
            ///   Moves a player across different scenes. The target
            ///     scene must be already loaded, and the source scene
            ///     will be. Either scene must be a valid and additively
            ///     loaded scene, or the main scene (which contains this
            ///     world object).
            /// </summary>
            /// <param name="identity">The player object to move</param>
            /// <param name="newScene">The target scene to move the object to</param>
            public void MovePlayer(NetworkIdentity identity, Scene newScene)
            {
                if (!NetworkServer.active) throw new OnlyAvailableInServer("Cannot invoke MovePlayer method in a non-server context");

                if (!isWorldReady) throw new WorldNotReady("Cannot invoke MovePlayer method when the world scenes are not ready");

                if (newScene.IsValid())
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
                else if (!newScene.isLoaded)
                {
                    throw new SceneNotLoadedException("Cannot move a network identity, across scenes, to a target scene that is not loaded");
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
                    // The player will be, in the end, refreshed into the parent scene.
                }
                else if (identity.gameObject.scene == gameObject.scene)
                {
                    // The player must be moved to the new scene.
                    SceneManager.MoveGameObjectToScene(identity.gameObject, newScene);
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
                    // The new scene must be loaded into the client.
                    identity.connectionToClient.Send(new SceneMessage { sceneName = newScene.name, sceneOperation = SceneOperation.LoadAdditive });
                    // The player will be, in the end, refreshed into the new scene.
                }
            }
        }
    }
}
