using AlephVault.Unity.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Protocols
    {
        public class Version : ISerializable
        {
            public const byte Stable = 0;
            public const byte RC = 1;
            public const byte Beta = 2;
            public const byte Alpha = 3;
            public const byte Prealpha = 4;

            public byte Major;
            public byte Minor;
            public byte Revision;
            public byte ReleaseType;

            public Version() {}

            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref Major);
                serializer.Serialize(ref Minor);
                serializer.Serialize(ref Revision);
                serializer.Serialize(ref ReleaseType);
            }
        }
    }
}
