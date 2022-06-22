using System;
using AlephVault.Unity.Binary;
using AlephVault.Unity.EVMGames.Auth.Types;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Samples
    {
        /// <summary>
        ///   The address
        /// </summary>
        [Serializable]
        public class SampleAccountPreviewDataType : IEVMAccountPreviewData
        {
            private string address;
        
            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref address);
            }

            public string Address { get => address; set => address = value; }
        }
    }
}