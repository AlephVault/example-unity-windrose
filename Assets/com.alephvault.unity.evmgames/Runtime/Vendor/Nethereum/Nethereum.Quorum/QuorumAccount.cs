using AlephVault.Unity.EVMGames.Nethereum.Signer;
using AlephVault.Unity.EVMGames.Nethereum.Web3.Accounts;
using System.Numerics;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum
{
    public class QuorumAccount : Account
    {

        public QuorumAccount(EthECKey key) : base(key)
        {

        }

        public QuorumAccount(string privateKey, BigInteger? chainId = null) : base(privateKey, chainId)
        {

        }

        public QuorumAccount(byte[] privateKey, BigInteger? chainId = null) : base(privateKey, chainId)
        {

        }

        protected override void InitialiseDefaultTransactionManager()
        {
            TransactionManager = new QuorumTransactionManager(null, null, this);
        }
    }
}
