using System.Text;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Network
{
    public interface ICipher
    {   
        Task<EncryptedPayload> EncryptWithKey(byte[] key, string data, Encoding encoding = null);

        Task<string> DecryptWithKey(byte[] key, EncryptedPayload encryptedData, Encoding encoding = null);
    }
}