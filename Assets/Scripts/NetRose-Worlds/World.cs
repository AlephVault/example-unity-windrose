using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using GMM.Types;

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

            void Start()
            {
                // Preload all the fixed scenes.
                foreach(var pair in fixedScenes)
                {
                    pair.Value.Preload();
                }

                // Preload all the template scenes.
                foreach(var pair in templateScenes)
                {
                    pair.Value.Preload();
                }
            }

            /// <summary>
            ///   Returns a fixed scene by its key. Returns an invalid
            ///   scene (uninitialized) if no scene exists for the key
            ///   or the corresponding scene was not (pre)loaded.
            /// </summary>
            /// <param name="key">The key to pick a scene by</param>
            /// <returns>A scene, which may be an invalid one if not found</returns>
            public Scene GetSceneByKey(string key)
            {
                SceneReference reference = null;
                if (fixedScenes.TryGetValue(key, out reference))
                {
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
            ///   or the corresponding scene was not (pre)loaded.
            /// </summary>
            /// <param name="key">The key to pick a scene by</param>
            /// <returns>A cloned scene, which may be an invalid one if not found</returns>
            public Scene CloneSceneByKey(string key, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
            {
                SceneReference reference = null;
                if (fixedScenes.TryGetValue(key, out reference) && reference.LoadedScene.IsValid())
                {
                    return SceneManager.LoadScene(reference.LoadedScene.name, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
                }
                else
                {
                    return new Scene{};
                }
            }
        }
    }
}
