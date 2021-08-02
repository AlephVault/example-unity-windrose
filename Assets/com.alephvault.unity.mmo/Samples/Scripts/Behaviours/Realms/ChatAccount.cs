using AlephVault.Unity.MMO.Types.Realms;
using MLAPI.Serialization;
using UnityEngine;


namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Realms
        {
            [System.Serializable]
            public class ChatAccount : IAccount<string, ChatAccountPreview>
            {
                [SerializeField]
                private ChatAccountPreview preview;

                [SerializeField]
                private string password;

                public bool Matches(string authPassword)
                {
                    return password == authPassword;
                }

                public ChatAccountPreview GetPreview()
                {
                    return preview;
                }

                public void NetworkSerialize(NetworkSerializer serializer)
                {
                    preview.NetworkSerialize(serializer);
                }
            }
        }
    }
}
