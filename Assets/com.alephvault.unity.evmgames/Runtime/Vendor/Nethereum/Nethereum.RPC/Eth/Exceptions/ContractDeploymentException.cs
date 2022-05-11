using System;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Exceptions
{
    public class ContractDeploymentException : Exception
    {
        public ContractDeploymentException(string message, TransactionReceipt transactionReceipt) : base(message)
        {
            TransactionReceipt = transactionReceipt;
        }

        public TransactionReceipt TransactionReceipt { get; set; }
    }
}