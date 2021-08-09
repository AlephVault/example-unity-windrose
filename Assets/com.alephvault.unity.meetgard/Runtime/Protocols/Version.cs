using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Protocols
    {
        public struct Version
        {
            public const byte Stable = 0;
            public const byte RC = 1;
            public const byte Beta = 2;
            public const byte Alpha = 3;
            public const byte Prealpha = 4;

            public readonly byte Major;
            public readonly byte Minor;
            public readonly byte Revision;
            public readonly byte ReleaseType;

            public Version(byte major, byte minor, byte revision, byte releaseType = Stable)
            {
                Major = major;
                Minor = minor;
                Revision = revision;
                ReleaseType = releaseType;
            }
        }
    }
}
