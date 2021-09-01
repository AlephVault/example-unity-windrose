using AlephVault.Unity.Support.Generic.Authoring.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Entities.Objects;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Types
        {
            /// <summary>
            ///   Keeps a track of the networked objects that are
            ///   registered in this server as prefabs. Each one
            ///   will have a distinct name that will serve the
            ///   purpose of network synchronization / spawning.
            /// </summary>
            public class NetworkedMapObjectPrefabDictionary : Dictionary<string, NetworkedMapObject> {}
        }
    }
}
