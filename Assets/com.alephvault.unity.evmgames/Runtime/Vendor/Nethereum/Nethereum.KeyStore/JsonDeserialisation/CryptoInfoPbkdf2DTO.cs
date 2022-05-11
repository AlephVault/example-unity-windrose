using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.KeyStore.Model;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Nethereum.KeyStore.JsonDeserialisation
{
    public class CryptoInfoPbkdf2DTO : CryptoInfoDTOBase
    {
        public CryptoInfoPbkdf2DTO()
        {
            kdfparams = new Pbkdf2ParamsDTO();
        }

        public Pbkdf2ParamsDTO kdfparams { get; set; }
    }
}