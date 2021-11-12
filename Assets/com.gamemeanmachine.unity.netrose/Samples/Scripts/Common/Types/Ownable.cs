using AlephVault.Unity.Binary;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Common
        {
            namespace Types
            {
                public class Ownable : ISerializable {
                    public bool IsOwned;

                    public void Serialize(Serializer serializer) {
                        serializer.Serialize(ref IsOwned);
                    }
                }
            }
        }
    }
}