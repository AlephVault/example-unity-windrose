using System;
using MLAPI;
using MLAPI.Transports.Tasks;


namespace AlephVault.Unity.MMOUniverse
{
    namespace Authoring
    {
        namespace Behaviours
        {
            public abstract partial class MMOUniverseManager : NetworkManager
            {
                /**
                 * Being a subclass of NetworkManager, it will have
                 * a shared restriction on singleton: Only one
                 * NetworkManager will be exist, be it this subclass
                 * or another one.
                 * 
                 * This subclass will add more interactions to the
                 * and callbacks, and will have features for world
                 * loading and related stuff.
                 */

                protected enum NetworkMode
                {
                    Disconnected, Client, Host, Server
                }

                /// <summary>
                ///   Intended for subclasses, this event allows
                ///   framework-level universe start attention.
                /// </summary>
                protected event Action OnUniverseInternalStarted = null;

                /// <summary>
                ///   Intended for subclasses, this event allows
                ///   framework-level universe stop attention.
                /// </summary>
                protected event Action OnUniverseInternalStopping = null;

                /// <summary>
                ///   Intended for public uses, this event allows
                ///   game-level universe start attention.
                /// </summary>
                public event Action OnUniverseStarted = null;

                /// <summary>
                ///   Intended for public uses, this event allows
                ///   game-level universe stop attention.
                /// </summary>
                public event Action OnUniverseStopping = null;

                // Network Mode is a local constant that tells the
                // current connection mode: client, host, or server.
                // This is used just for internal tracking.
                protected NetworkMode currentMode = NetworkMode.Disconnected;

                private void StartUniverse()
                {
                    if (IsServer && IsClient)
                    {
                        currentMode = NetworkMode.Host;
                    }
                    else if (IsServer)
                    {
                        currentMode = NetworkMode.Server;
                    }
                    else if (IsClient)
                    {
                        currentMode = NetworkMode.Client;
                    }
                    OnUniverseInternalStarted?.Invoke();
                    OnUniverseStarted?.Invoke();
                }

                private void StopUniverse()
                {
                    OnUniverseStopping?.Invoke();
                    OnUniverseInternalStopping?.Invoke();
                    currentMode = NetworkMode.Disconnected;
                }

                public new SocketTasks StartClient()
                {
                    SocketTasks result = base.StartClient();
                    if (result.IsDone)
                    {
                        if (result.Success) StartUniverse();
                    }
                    // Question: should we handle !result.IsDone?
                    return result;
                }

                private void Start()
                {
                    // For starting in server mode and host mode, this
                    // assignment tracks server start as well.
                    OnServerStarted += StartUniverse;
                }
            }
        }
    }
}
