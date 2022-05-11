using AlephVault.Unity.EVMGames.Nethereum.RPC.Shh.KeyPair;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Shh
{
    public interface IShhKeyPair
    {
        IShhNewKeyPair NewKeyPair { get; }
        IShhAddPrivateKey AddPrivateKey { get; }
        IShhDeleteKeyPair DeleteKeyPair { get; }
        IShhHasKeyPair HasKeyPair { get; }
        IShhGetPublicKey GetPublicKey { get; }
        IShhGetPrivateKey GetPrivateKey { get; }
    }
}
