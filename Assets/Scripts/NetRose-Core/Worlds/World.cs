﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using GMM.Types;
using GMM.Utils;

namespace NetRose
{
    namespace Worlds
    {
        /// <summary>
        ///   <para>
        ///     World will define their layout of scenes. Each scene will
        ///     have a distinct key and they will be either singleton or
        ///     template scenes when trying to load.
        ///   </para>
        ///   <para>
        ///     It has several features involving the underlying scenes,
        ///     and moving players across scenes.
        ///   </para>
        ///   <para>
        ///     This object, on itself, is a singleton.
        ///   </para>
        /// </summary>
        public class World : NetworkBehaviour
        {
            /// <summary>
            ///   The list of scenes to load. Those scenes can be either
            ///   template or singleton scenes.
            /// </summary>
            [SerializeField]
            SceneConfigDictionary scenes = new SceneConfigDictionary();

            /// <summary>
            ///   Set this property to true to cause a preload to be triggered
            ///   when this component starts in the server.
            /// </summary>
            [SerializeField]
            private bool autoPreload = false;

            // The tracked instance.
            private static World instance = null;

            private void Start()
            {
                // This object must exist into a normally-loaded scene.
                // With this, it is guaranteed that only one instance
                // will be used.
                if (instance != null && instance != this)
                {
                    Destroy(this);
                    throw new SingletonException("A world instance already exists");
                }
                else
                {
                    instance = this;
                }
            }

            // When this object's spawned identity started on a server,
            // it must preload all the singleton sub-scenes.
            public override void OnStartServer()
            {
                if (autoPreload && isServer)
                {
                    CastPreload();
                }
            }

            private async void CastPreload()
            {
                await Preload();
            }

            private void OnDestroy()
            {
                if (instance == this) instance = null;
            }

            /// <summary>
            ///   Preloads all the singleton scenes. This is an asynchronous
            ///   task that must be waited for.
            /// </summary>
            public async Task Preload()
            {
                foreach(KeyValuePair<string, SceneConfig> pair in scenes)
                {
                    if (pair.Value.LoadMode == SceneLoadMode.Singleton)
                    {
                        await pair.Value.Load();
                    }
                }
            }

            /// <summary>
            ///   Loads a scene, identified by its key. If the key does not
            ///   exist, an invalid scene will be returned. Depending on the
            ///   scene mode, an existing scene instance will be returned or
            ///   a new scene instance will be created. This is an asynchronous
            ///   task that must be waited for. A reference to the loaded
            ///   <see cref="Scene"/> is then returned, either existing or
            ///   instantiated.
            /// </summary>
            /// <param name="sceneKey">The scene key to load</param>
            /// <returns>The loaded scene</returns>
            public async Task<Scene> Load(string sceneKey)
            {
                SceneConfig config;
                Scene scene = new Scene{};
                if (scenes.TryGetValue(sceneKey, out config))
                {
                    return await config.Load();
                }
                else
                {
                    return scene;
                }
            }

            private bool IsPlayer(NetworkIdentity identity)
            {
                return identity.connectionToClient.identity == identity;
            }


            // This method only makes sense in server-side.
            private bool IsActiveConnection(NetworkIdentity identity)
            {
                return identity.connectionToClient.isReady;
            }

            /// <summary>
            ///   Moves a player across different scenes. The target
            ///   scene must be already loaded, and the source scene
            ///   will be. Either scene must be a valid and additively
            ///   loaded scene, or the main scene (which contains this
            ///   world object).
            /// </summary>
            /// <param name="identity">The player object to move</param>
            /// <param name="newScene">The target scene to move the object to</param>
            public void MovePlayer(NetworkIdentity identity, Scene newScene)
            {
                if (isServer) return;

                if (newScene == null)
                {
                    newScene = gameObject.scene;
                }

                // TODO: Add assumption that each scene may have a WindRose
                //       Map inside, and that the object being moved is a
                //       WindRose Object. With this in mind, attempt the
                //       attachments and detachments, notifying the events
                //       in the meantime.

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
