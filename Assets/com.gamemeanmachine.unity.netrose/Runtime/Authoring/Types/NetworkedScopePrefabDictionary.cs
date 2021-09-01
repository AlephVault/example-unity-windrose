using AlephVault.Unity.Support.Generic.Authoring.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.World;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   Keeps a track of the networked scopes that are
            ///   registered in this server as prefabs. Each one
            ///   will have a distinct name that will serve the
            ///   purpose of network synchronization.
            /// </summary>
            public class NetworkedScopePrefabDictionary : Dictionary<string, NetworkedScope> {}
        }
    }
}
