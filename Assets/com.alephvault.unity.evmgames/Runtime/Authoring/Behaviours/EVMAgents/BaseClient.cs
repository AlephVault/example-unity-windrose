using UnityEngine;

namespace AlephVault.Unity.EVMGames
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace EVMAgents
            {
                /// <summary>
                ///   A base client has all the features to be used as a direct
                ///   client or a sponsored one. A sponsored client performs
                ///   both signatures and direct calls (e.g. not just signing
                ///   a transaction but, instead, sending it. Clients will use
                ///   Wallet Connect when notusing WebGL, and will use the
                ///   injected web3
                /// </summary>
                public partial class BaseClient : MonoBehaviour
                {
                }
            }
        }
    }
}