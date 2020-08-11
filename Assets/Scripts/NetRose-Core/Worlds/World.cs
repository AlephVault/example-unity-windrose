using System.Collections.Generic;
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
            ///   when this component starts.
            /// </summary>
            [SerializeField]
            private bool autoPreload = false;

            // The tracked instance.
            private static World instance = null;

            private async void Start()
            {
                if (instance != null && instance != this)
                {
                    // TODO error, and destroy.
                }
                else
                {
                    instance = this;
                }

                if (autoPreload)
                {
                    await Preload();
                }
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

            
            private bool IsActiveConnection(NetworkIdentity identity)
            {
                // This method only makes sense in server-side.
                return identity.connectionToClient.isReady;
            }

            public void MovePlayer(NetworkIdentity identity, Scene newScene)
            {
                if (isServer)
                {
                    if (newScene == null)
                    {
                        newScene = gameObject.scene;
                    }

                    if (!IsPlayer(identity))
                    {
                        // TODO error
                    }
                    else if (!IsActiveConnection(identity))
                    {
                        // TODO other error
                    }
                    else if (!newScene.isLoaded)
                    {
                        // TODO other error
                    }
                    else if (newScene == identity.gameObject.scene)
                    {
                        // Nothing to do here, except perhaps for a notify / log
                    }
                    else if (newScene == gameObject.scene)
                    {
                        // TODO case: the object moving from whatever its scene is,
                        // TODO       to the main scene.
                    }
                    else if (identity.gameObject.scene == gameObject.scene)
                    {
                        // TODO case: the object moving from the main scene to another
                        //            scene (which is loaded, additively).
                    }
                    else
                    {
                        // TODO case: the object moving from a non-main scene to another
                        //            non-main scene.
                    }
                }
            }
        }
    }
}
