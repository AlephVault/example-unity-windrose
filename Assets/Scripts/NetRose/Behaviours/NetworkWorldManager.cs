using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NetRose.Types;

namespace NetRose
{
    namespace Behaviours
    {
        /// <summary>
        ///   <para>
        ///     This network manager, when on the online scene,
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
        ///   <para>
        ///     This behaviour will require an authenticator to be
        ///       added as component, but since this behaviour is
        ///       generic, a runtime type check will be done to
        ///       ensure the authenticator being used is of the
        ///       appropriate type (i.e. a "player source" one with
        ///       the appropriate types).
        ///   </para>
        /// </summary>
        [RequireComponent(typeof(NetworkAuthenticator))]
        public class NetworkWorldManager : NetworkManager
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
            ///   Triggered when trying to move a player object with inactive connection.
            /// </summary>
            public class InactiveConnectionException : Exception
            {
                public InactiveConnectionException() { }
                public InactiveConnectionException(string message) : base(message) { }
                public InactiveConnectionException(string message, System.Exception inner) : base(message, inner) { }
            }

            /// <summary>
            ///   Triggered when trying to create another networked scene layout instance.
            /// </summary>
            public class SingletonException : Exception
            {
                public SingletonException() { }
                public SingletonException(string message) : base(message) { }
                public SingletonException(string message, System.Exception inner) : base(message, inner) { }
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
            public bool isWorldReady { get; private set; }

            /// <summary>
            ///   The list of scenes to load. Those scenes can be either
            ///     template or singleton scenes. Template scenes will
            ///     usually not be included here, but all the singleton
            ///     scenes in a game typically will.
            /// </summary>
            [SerializeField]
            SceneConfigDictionary scenes = new SceneConfigDictionary();

            /// <summary>
            ///   This implementation of Awake will consider the authenticator
            ///     as a component and check its type to be a TODO player source
            ///     authenticator.
            /// </summary>
            public override void Awake()
            {
                authenticator = GetComponent<NetworkAuthenticator>();
                // TODO Check authenticator to be of a generic implementation of PlayerSourceAuthenticator<?, ?, ?>.
                // TODO or raise an exception.
                base.Awake();
            }

            /// <summary>
            ///   This hook tests for when the scene is changed to be the online
            ///     one, and in that case it will load all the required scenes
            ///     which are singleton.
            /// </summary>
            /// <param name="sceneName"></param>
            public override void OnServerSceneChanged(string sceneName)
            {
                if (sceneName == onlineScene)
                {
                    PreloadSingletonScenes();
                }
            }

            // Preloads all the singleton scenes.
            private async void PreloadSingletonScenes()
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
                catch(System.Exception)
                {
                    StopClient();
                    Destroy(gameObject);
                }
            }
        }
    }
}
