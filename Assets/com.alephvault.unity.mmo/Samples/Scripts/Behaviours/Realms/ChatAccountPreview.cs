using AlephVault.Unity.MMO.Types.Realms;
using MLAPI.Serialization;


namespace AlephVault.Unity.MMO.Samples
{
    namespace Behaviours
    {
        namespace Realms
        {
            [System.Serializable]
            public class ChatAccountPreview : IAccountPreview<string>
            {
                public string Username;

                public string GetID()
                {
                    return Username;
                }

                public void NetworkSerialize(NetworkSerializer serializer)
                {
                    serializer.Serialize(ref Username);
                }
            }
        }
    }
}
