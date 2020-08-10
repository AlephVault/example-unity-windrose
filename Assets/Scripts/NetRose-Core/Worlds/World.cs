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
        ///     Worlds account for two different sets of scenes that will be
        ///     loaded in different contexts: fixed scenes, and template scenes.
        ///   </para>
        ///   <para>
        ///     Fixed scenes are loaded on start. They are added additively.
        ///     Template scenes are also loaded on start, and additively, but
        ///     they will serve a different purpose: they will be cloned and
        ///     instantiated.
        ///   </para>
        /// </summary>
        public class World : NetworkBehaviour
        {
            /// <summary>
            ///   A list of "fixed" scenes, which may be referenced
            ///   later via its code. Those "fixed" scenes can be
            ///   referenced later (e.g. to teleport a character
            ///   from one place to another).
            /// </summary>
            [SerializeField]
            private SceneReferenceDictionary fixedScenes;

            /// <summary>
            ///   A list of "template" scenes. New scenes can be
            ///   "cloned" out of each of these templates.
            /// </summary>
            [SerializeField]
            private SceneReferenceDictionary templateScenes;

            async void Start()
            {
                // Preload all the fixed scenes.
                foreach(var pair in fixedScenes)
                {
                    await pair.Value.Preload();
                }

                // Preload all the template scenes.
                foreach(var pair in templateScenes)
                {
                    await pair.Value.Preload();
                }
            }

            /// <summary>
            ///   Returns a fixed scene by its key. Returns an invalid
            ///   scene (uninitialized) if no scene exists for the key
            ///   or the corresponding scene was not (pre)loaded. This
            ///   is an asynchronous task that must be waited for.
            /// </summary>
            /// <param name="key">The key to pick a scene by</param>
            /// <returns>A scene, which may be an invalid one if not found</returns>
            public async Task<Scene> GetSceneByKey(string key, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
            {
                SceneReference reference = null;
                if (fixedScenes.TryGetValue(key, out reference))
                {
                    await reference.Preload(physicsMode);
                    return reference.LoadedScene;
                }
                else
                {
                    return new Scene{};
                }
            }

            /// <summary>
            ///   Clones a template scene by its key. Returns an invalid
            ///   scene (uninitialized) if no scene exists for the key
            ///   or the corresponding scene was not (pre)loaded. This
            ///   is an asynchronous task that must be waited for.
            /// </summary>
            /// <param name="key">The key to pick a scene by</param>
            /// <param name="physicsMode">The physics mode to clone the scene with</param>
            /// <returns>A cloned scene, which may be an invalid one if not found</returns>
            public async Task<Scene> CloneSceneByKey(string key, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
            {
                SceneReference reference = null;
                if (fixedScenes.TryGetValue(key, out reference))
                {
                    // To get a scene, loadSceneAsync will be called and will return
                    // an AsyncOperation. Such operations are queued, thus never
                    // running into race conditions. This will imply that this code
                    // will be stable regarding the scene count.
                    Scene scene = new Scene{};
                    AsyncOperation operation = SceneManager.LoadSceneAsync(reference.Path, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
                    int index = SceneManager.sceneCount;
                    operation.completed += (op) =>
                    {
                        if (SceneManager.sceneCount != index)
                        {
                            scene = SceneManager.GetSceneAt(index);
                        }
                    };
                    while (!operation.isDone)
                    {
                        await Tasks.Blink();
                    }
                    return scene;
                }
                else
                {
                    return new Scene{};
                }
            }
        }
    }
}
