using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.Quorum.Enclave;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Transactions;
using AlephVault.Unity.EVMGames.Nethereum.RPC.NonceServices;
using AlephVault.Unity.EVMGames.Nethereum.RPC.TransactionManagers;
using AlephVault.Unity.EVMGames.Nethereum.Signer;
using AlephVault.Unity.EVMGames.Nethereum.Util;
using AlephVault.Unity.EVMGames.Nethereum.Web3.Accounts;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum
{
    public class QuorumTransactionManager : TransactionManagerBase
    {
        public string PrivateUrl { get; set; }
        private readonly AccountOfflineTransactionSigner _transactionSigner;
        internal List<string> PrivateFor { get; set; }
        internal string PrivateFrom { get; set; }

        public QuorumTransactionManager(IClient rpcClient, string privateUrl, QuorumAccount account)
        {
            PrivateUrl = privateUrl;
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Client = rpcClient;
            _transactionSigner = new AccountOfflineTransactionSigner();
        }


        public QuorumTransactionManager(IClient rpcClient, string privateUrl, string privateKey)
        {
 
            if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
            PrivateUrl = privateUrl;
            Client = rpcClient;
            Account = new QuorumAccount(privateKey);
            Account.NonceService = new InMemoryNonceService(Account.Address, rpcClient);
            _transactionSigner = new AccountOfflineTransactionSigner();
        }

        public QuorumTransactionManager(string privateKey) : this(null, null, privateKey)
        {

        }

        public override BigInteger DefaultGas { get; set; } = LegacyTransaction.DEFAULT_GAS_LIMIT;


        public override Task<string> SendTransactionAsync(TransactionInput transactionInput)
        {
            if (transactionInput == null) throw new ArgumentNullException(nameof(transactionInput));
            return SignAndSendTransactionAsync(transactionInput);
        }

        public override Task<string> SignTransactionAsync(TransactionInput transaction)
        { 
            return SignTransactionRetrievingNextNonceAsync(transaction);
        }

        public string SignTransaction(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            SetDefaultGasPriceAndCostIfNotSet(transaction);
            var  txnSigned = _transactionSigner.SignTransaction((QuorumAccount)Account, transaction);
            return GetPrivateSignedTransaction(txnSigned);
        }

        private string GetPrivateSignedTransaction(string txnSigned)
        {
            if (PrivateFor != null && PrivateFor.Count > 0)
            {
                var signedData = RLPDecoder.DecodeSigned(txnSigned.HexToByteArray(), 6);

                if (signedData.V[0] == 28)
                {
                    signedData.V[0] = 38;
                }
                else
                {
                    signedData.V[0] = 37;
                }

                return RLPEncoder.EncodeSigned(signedData, 6).ToHex();
            }

            return txnSigned;
        }

        protected async Task<string> SignTransactionRetrievingNextNonceAsync(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");
            var nonce = await GetNonceAsync(transaction).ConfigureAwait(false);
            transaction.Nonce = nonce;
            var gasPrice = await GetGasPriceAsync(transaction).ConfigureAwait(false);
            transaction.GasPrice = gasPrice;
            return SignTransaction(transaction);
        }

        public async Task<HexBigInteger> GetNonceAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            var nonce = transaction.Nonce;
            if (nonce == null)
            {
                if (Account.NonceService == null)
                    Account.NonceService = new InMemoryNonceService(Account.Address, Client);
                Account.NonceService.Client = Client;
                nonce = await Account.NonceService.GetNextNonceAsync().ConfigureAwait(false);
            }
            return nonce;
        }

        private async Task<string> SignAndSendTransactionAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");

            if (PrivateFor != null && PrivateFor.Count > 0)
            {
                var enclave = new QuorumEnclave(PrivateUrl);
                var key = await enclave.StoreRawAsync(Convert.ToBase64String(transaction.Data.HexToByteArray()), PrivateFrom);
                transaction.Data = Convert.FromBase64String(key).ToHex();
            }

            var ethSendTransaction = new EthSendRawTransaction(Client);
            var signedTransaction = await SignTransactionRetrievingNextNonceAsync(transaction).ConfigureAwait(false);
            return await ethSendTransaction.SendRequestAsync(signedTransaction.EnsureHexPrefix()).ConfigureAwait(false);
        }
    }
}
