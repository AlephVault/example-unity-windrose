using AlephVault.Unity.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Samples
    {
        public class Echo : ISerializable
        {
            public string Content;

            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref Content);
            }
        }

    }
}
