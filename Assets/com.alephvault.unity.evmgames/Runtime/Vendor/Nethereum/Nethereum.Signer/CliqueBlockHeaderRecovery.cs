using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.Model;

namespace AlephVault.Unity.EVMGames.Nethereum.Signer
{
    public class CliqueBlockHeaderRecovery
    {
        public string RecoverCliqueSigner(BlockHeader blockHeader, bool legacy = false)
        {
            var blockEncoded = BlockHeaderEncoder.Current.EncodeCliqueSigHeader(blockHeader);
            var signature = blockHeader.ExtraData.Skip(blockHeader.ExtraData.Length - 65).ToArray();
            return
                new MessageSigner().EcRecover(BlockHeaderEncoder.Current.EncodeCliqueSigHeaderAndHash(blockHeader, legacy),
                    signature.ToHex());
        }
    }
}
