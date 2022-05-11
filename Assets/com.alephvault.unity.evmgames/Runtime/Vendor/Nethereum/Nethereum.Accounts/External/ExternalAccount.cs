using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Accounts;
using AlephVault.Unity.EVMGames.Nethereum.RPC.NonceServices;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;
using AlephVault.Unity.EVMGames.Nethereum.Signer;

namespace AlephVault.Unity.EVMGames.Nethereum.Web3.Accounts
{
    public class ExternalAccount : IAccount
    {
        public ExternalAccount(IEthExternalSigner externalSigner, BigInteger? chainId = null)
        {
            ExternalSigner = externalSigner;
            ChainId = chainId;
        }

        public ExternalAccount(string address, IEthExternalSigner externalSigner, BigInteger? chainId = null)
        {
            ChainId = chainId;
            Address = address;
            ExternalSigner = externalSigner;
        }

        public IEthExternalSigner ExternalSigner { get; }
        public BigInteger? ChainId { get; }

        public string Address { get; protected set; }
        public ITransactionManager TransactionManager { get; protected set; }
        public INonceService NonceService { get; set; }

        public async Task InitialiseAsync()
        {
            Address = await ExternalSigner.GetAddressAsync().ConfigureAwait(false);
        }

        public void InitialiseDefaultTransactionManager(IClient client)
        {
            TransactionManager = new ExternalAccountSignerTransactionManager(client, this, ChainId);
        }
    }
}