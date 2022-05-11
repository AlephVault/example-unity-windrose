using System.Collections.Generic;

namespace AlephVault.Unity.EVMGames.Nethereum.Signer
{
    public class AccessListItem
    {
        public string Address { get; set; }
        public List<byte[]> StorageKeys { get; set; }

        public AccessListItem()
        {
            StorageKeys = new List<byte[]>();
        }

        public AccessListItem(string address, List<byte[]> storageKeys)
        {
            this.Address = address;
            this.StorageKeys = storageKeys;
        }
    }
}