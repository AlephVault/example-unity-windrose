using System;
using System.Numerics;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.ContractHandlers;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Web3;
using AlephVault.Unity.EVMGames.Samples.EthModels;
using UnityEngine;
using UnityEngine.UI;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        public partial class SampleContractInteractor
        {
            private async Task<BigInteger> BalanceOf(Web3 client, string address)
            {
                // Computes the balance of the SMP token by using a
                // Web3 client (typically, only the direct client
                // will be used).
                IContractQueryHandler<Erc20BalanceOfFunction> call = client.Eth.GetContractQueryHandler<Erc20BalanceOfFunction>();
                return (await call.QueryDeserializingToObjectAsync<BalanceOfOutputDTO>(new Erc20BalanceOfFunction
                {
                    Owner = address
                }, ContractAddress)).Balance;
            }

            private async void DoBalanceOf(Web3 client, InputField address, Text result)
            {
                // Computes the balance of the SMP token by using a
                // Web3 client and an input text box (for the address
                // argument) and a label (for the result of the call).
                try
                {
                    result.text = (await BalanceOf(client, address.text.Trim())).ToString();
                }
                catch (Exception e)
                {
                    result.text = "An error has occurred - see logs";
                    Debug.LogException(e);
                }
            }
            
            private async Task<TransactionReceipt> Transfer(Web3 client, string from, string to, BigInteger value)
            {
                // Performs a transfer from a given address (which will
                // use the 1st one from the current account) to another
                // address and for a specified amount.
                IContractTransactionHandler<Erc20TransferFunction> send =
                    client.Eth.GetContractTransactionHandler<Erc20TransferFunction>();
                return await send.SendRequestAndWaitForReceiptAsync(ContractAddress, new Erc20TransferFunction()
                {
                    To = to,
                    TokenAmount = value,
                    FromAddress = from,
                    Gas = 500000,
                    GasPrice = 40000000000
                });
            }

            private async void DoTransfer(Web3 client, InputField to, InputField amount, Text result)
            {
                // Performs a transfer from a given address (which will
                // use the 1st one from the current account) to another
                // address and for an amount, but taking the values from
                // certain UI components.
                try
                {
                    BigInteger value = BigInteger.Parse(amount.text.Trim());
                    string[] accounts = await client.Eth.Accounts.SendRequestAsync();
                    string txId = (await Transfer(client, accounts[0], to.text.Trim(), value)).TransactionHash;
                    result.text = $"tx: {txId}";
                }
                catch (Exception e)
                {
                    result.text = "An error has occurred - see logs";
                    Debug.LogException(e);
                }
            }
        }
    }
}
