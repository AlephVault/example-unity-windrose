using AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.MessageEncodingServices
{
    public interface IContractMessageTransactionInputCreator<TContractMessage>: IDefaultAddressService
        where TContractMessage : ContractMessageBase
    {
        TransactionInput CreateTransactionInput(TContractMessage contractMessage);
        CallInput CreateCallInput(TContractMessage contractMessage);
    }


    public interface IDefaultAddressService
    {
        string DefaultAddressFrom { get; set; }
    }
}