using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexTypes;
using AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.Attributes;
using AlephVault.Unity.EVMGames.Nethereum.Web3;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.CQS;
using AlephVault.Unity.EVMGames.Nethereum.Contracts;
using System.Threading;

namespace AlephVault.Unity.EVMGames.Nethereum.Optimism.L2StandardBridge.ContractDefinition
{


    public partial class L2StandardBridgeDeployment : L2StandardBridgeDeploymentBase
    {
        public L2StandardBridgeDeployment() : base(BYTECODE) { }
        public L2StandardBridgeDeployment(string byteCode) : base(byteCode) { }
    }

    public class L2StandardBridgeDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b50604051610d37380380610d3783398101604081905261002f9161007c565b600080546001600160a01b039384166001600160a01b031991821617909155600180549290931691161790556100af565b80516001600160a01b038116811461007757600080fd5b919050565b6000806040838503121561008f57600080fd5b61009883610060565b91506100a660208401610060565b90509250929050565b610c79806100be6000396000f3fe608060405234801561001057600080fd5b50600436106100575760003560e01c806332b7006d1461005c57806336c717c1146100715780633cb747bf146100a0578063662a633a146100b3578063a3a79548146100c6575b600080fd5b61006f61006a3660046108f4565b6100d9565b005b600154610084906001600160a01b031681565b6040516001600160a01b03909116815260200160405180910390f35b600054610084906001600160a01b031681565b61006f6100c1366004610965565b6100ef565b61006f6100d43660046109fd565b6104a8565b6100e8853333878787876104bf565b5050505050565b6001546001600160a01b031661010d6000546001600160a01b031690565b6001600160a01b0316336001600160a01b0316146101895760405162461bcd60e51b815260206004820152602e60248201527f4f564d5f58434841494e3a206d657373656e67657220636f6e7472616374207560448201526d1b985d5d1a195b9d1a58d85d195960921b60648201526084015b60405180910390fd5b806001600160a01b03166101a56000546001600160a01b031690565b6001600160a01b0316636e296e456040518163ffffffff1660e01b8152600401602060405180830381865afa1580156101e2573d6000803e3d6000fd5b505050506040513d601f19601f820116820180604052508101906102069190610a80565b6001600160a01b0316146102755760405162461bcd60e51b815260206004820152603060248201527f4f564d5f58434841494e3a2077726f6e672073656e646572206f662063726f7360448201526f732d646f6d61696e206d65737361676560801b6064820152608401610180565b61028687631d1d8b6360e01b6106ce565b80156103065750866001600160a01b031663c01e1bd66040518163ffffffff1660e01b81526004016020604051808303816000875af11580156102cd573d6000803e3d6000fd5b505050506040513d601f19601f820116820180604052508101906102f19190610a80565b6001600160a01b0316886001600160a01b0316145b156103cd576040516340c10f1960e01b81526001600160a01b038681166004830152602482018690528816906340c10f1990604401600060405180830381600087803b15801561035557600080fd5b505af1158015610369573d6000803e3d6000fd5b50505050856001600160a01b0316876001600160a01b0316896001600160a01b03167fb0444523268717a02698be47d0803aa7468c00acbed2f8bd93a0459cde61dd89888888886040516103c09493929190610acd565b60405180910390a461049e565b600063a9f9e67560e01b8989888a8989896040516024016103f49796959493929190610af5565b60408051601f198184030181529190526020810180516001600160e01b03166001600160e01b031990931692909217909152600154909150610441906001600160a01b03166000836106f3565b866001600160a01b0316886001600160a01b03168a6001600160a01b03167f7ea89a4591614515571c2b51f5ea06494056f261c10ab1ed8c03c7590d87bce0898989896040516104949493929190610acd565b60405180910390a4505b5050505050505050565b6104b7863387878787876104bf565b505050505050565b604051632770a7eb60e21b8152336004820152602481018590526001600160a01b03881690639dc29fac90604401600060405180830381600087803b15801561050757600080fd5b505af115801561051b573d6000803e3d6000fd5b505050506000876001600160a01b031663c01e1bd66040518163ffffffff1660e01b81526004016020604051808303816000875af1158015610561573d6000803e3d6000fd5b505050506040513d601f19601f820116820180604052508101906105859190610a80565b905060606001600160a01b03891673deaddeaddeaddeaddeaddeaddeaddeaddead000014156106095760405163054cbb0d60e21b906105d0908a908a908a9089908990602401610b46565b60408051601f198184030181529190526020810180516001600160e01b03166001600160e01b0319909316929092179091529050610664565b60405163a9f9e67560e01b9061062f9084908c908c908c908c908b908b90602401610af5565b60408051601f198184030181529190526020810180516001600160e01b03166001600160e01b03199093169290921790915290505b60015461067b906001600160a01b031686836106f3565b336001600160a01b0316896001600160a01b0316836001600160a01b03167f73d170910aba9e6d50b102db522b1dbcd796216f5128b445aa2135272886497e8a8a89896040516104949493929190610acd565b60006106d98361075e565b80156106ea57506106ea8383610791565b90505b92915050565b600054604051633dbb202b60e01b81526001600160a01b0390911690633dbb202b9061072790869085908790600401610bb5565b600060405180830381600087803b15801561074157600080fd5b505af1158015610755573d6000803e3d6000fd5b50505050505050565b6000610771826301ffc9a760e01b610791565b80156106ed575061078a826001600160e01b0319610791565b1592915050565b604080516001600160e01b0319831660248083019190915282518083039091018152604490910182526020810180516001600160e01b03166301ffc9a760e01b179052905160009190829081906001600160a01b03871690617530906107f8908690610c05565b6000604051808303818686fa925050503d8060008114610834576040519150601f19603f3d011682016040523d82523d6000602084013e610839565b606091505b509150915060208151101561085457600093505050506106ed565b8180156108705750808060200190518101906108709190610c21565b9695505050505050565b6001600160a01b038116811461088f57600080fd5b50565b803563ffffffff811681146108a657600080fd5b919050565b60008083601f8401126108bd57600080fd5b50813567ffffffffffffffff8111156108d557600080fd5b6020830191508360208285010111156108ed57600080fd5b9250929050565b60008060008060006080868803121561090c57600080fd5b85356109178161087a565b94506020860135935061092c60408701610892565b9250606086013567ffffffffffffffff81111561094857600080fd5b610954888289016108ab565b969995985093965092949392505050565b600080600080600080600060c0888a03121561098057600080fd5b873561098b8161087a565b9650602088013561099b8161087a565b955060408801356109ab8161087a565b945060608801356109bb8161087a565b93506080880135925060a088013567ffffffffffffffff8111156109de57600080fd5b6109ea8a828b016108ab565b989b979a50959850939692959293505050565b60008060008060008060a08789031215610a1657600080fd5b8635610a218161087a565b95506020870135610a318161087a565b945060408701359350610a4660608801610892565b9250608087013567ffffffffffffffff811115610a6257600080fd5b610a6e89828a016108ab565b979a9699509497509295939492505050565b600060208284031215610a9257600080fd5b8151610a9d8161087a565b9392505050565b81835281816020850137506000828201602090810191909152601f909101601f19169091010190565b60018060a01b0385168152836020820152606060408201526000610870606083018486610aa4565b6001600160a01b03888116825287811660208301528681166040830152851660608201526080810184905260c060a08201819052600090610b399083018486610aa4565b9998505050505050505050565b6001600160a01b0386811682528516602082015260408101849052608060608201819052600090610b7a9083018486610aa4565b979650505050505050565b60005b83811015610ba0578181015183820152602001610b88565b83811115610baf576000848401525b50505050565b60018060a01b03841681526060602082015260008351806060840152610be2816080850160208801610b85565b63ffffffff93909316604083015250601f91909101601f19160160800192915050565b60008251610c17818460208701610b85565b9190910192915050565b600060208284031215610c3357600080fd5b81518015158114610a9d57600080fdfea264697066735822122018b7b89b5c6e4bf199d63b1a4d1328c09d164ee879dd82e968359b88cff2f01664736f6c634300080b0033";
        public L2StandardBridgeDeploymentBase() : base(BYTECODE) { }
        public L2StandardBridgeDeploymentBase(string byteCode) : base(byteCode) { }
        [Parameter("address", "_l2CrossDomainMessenger", 1)]
        public virtual string L2CrossDomainMessenger { get; set; }
        [Parameter("address", "_l1TokenBridge", 2)]
        public virtual string L1TokenBridge { get; set; }
    }

    public partial class FinalizeDepositFunction : FinalizeDepositFunctionBase { }

    [Function("finalizeDeposit")]
    public class FinalizeDepositFunctionBase : FunctionMessage
    {
        [Parameter("address", "_l1Token", 1)]
        public virtual string L1Token { get; set; }
        [Parameter("address", "_l2Token", 2)]
        public virtual string L2Token { get; set; }
        [Parameter("address", "_from", 3)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_amount", 5)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bytes", "_data", 6)]
        public virtual byte[] Data { get; set; }
    }

    public partial class L1TokenBridgeFunction : L1TokenBridgeFunctionBase { }

    [Function("l1TokenBridge", "address")]
    public class L1TokenBridgeFunctionBase : FunctionMessage
    {

    }

    public partial class MessengerFunction : MessengerFunctionBase { }

    [Function("messenger", "address")]
    public class MessengerFunctionBase : FunctionMessage
    {

    }

    public partial class WithdrawFunction : WithdrawFunctionBase { }

    [Function("withdraw")]
    public class WithdrawFunctionBase : FunctionMessage
    {
        [Parameter("address", "_l2Token", 1)]
        public virtual string L2Token { get; set; }
        [Parameter("uint256", "_amount", 2)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("uint32", "_l1Gas", 3)]
        public virtual uint L1Gas { get; set; }
        [Parameter("bytes", "_data", 4)]
        public virtual byte[] Data { get; set; }
    }

    public partial class WithdrawToFunction : WithdrawToFunctionBase { }

    [Function("withdrawTo")]
    public class WithdrawToFunctionBase : FunctionMessage
    {
        [Parameter("address", "_l2Token", 1)]
        public virtual string L2Token { get; set; }
        [Parameter("address", "_to", 2)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_amount", 3)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("uint32", "_l1Gas", 4)]
        public virtual uint L1Gas { get; set; }
        [Parameter("bytes", "_data", 5)]
        public virtual byte[] Data { get; set; }
    }

    public partial class DepositFailedEventDTO : DepositFailedEventDTOBase { }

    [Event("DepositFailed")]
    public class DepositFailedEventDTOBase : IEventDTO
    {
        [Parameter("address", "_l1Token", 1, true)]
        public virtual string L1Token { get; set; }
        [Parameter("address", "_l2Token", 2, true)]
        public virtual string L2Token { get; set; }
        [Parameter("address", "_from", 3, true)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4, false)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_amount", 5, false)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bytes", "_data", 6, false)]
        public virtual byte[] Data { get; set; }
    }

    public partial class DepositFinalizedEventDTO : DepositFinalizedEventDTOBase { }

    [Event("DepositFinalized")]
    public class DepositFinalizedEventDTOBase : IEventDTO
    {
        [Parameter("address", "_l1Token", 1, true)]
        public virtual string L1Token { get; set; }
        [Parameter("address", "_l2Token", 2, true)]
        public virtual string L2Token { get; set; }
        [Parameter("address", "_from", 3, true)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4, false)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_amount", 5, false)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bytes", "_data", 6, false)]
        public virtual byte[] Data { get; set; }
    }

    public partial class WithdrawalInitiatedEventDTO : WithdrawalInitiatedEventDTOBase { }

    [Event("WithdrawalInitiated")]
    public class WithdrawalInitiatedEventDTOBase : IEventDTO
    {
        [Parameter("address", "_l1Token", 1, true)]
        public virtual string L1Token { get; set; }
        [Parameter("address", "_l2Token", 2, true)]
        public virtual string L2Token { get; set; }
        [Parameter("address", "_from", 3, true)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 4, false)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_amount", 5, false)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bytes", "_data", 6, false)]
        public virtual byte[] Data { get; set; }
    }



    public partial class L1TokenBridgeOutputDTO : L1TokenBridgeOutputDTOBase { }

    [FunctionOutput]
    public class L1TokenBridgeOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class MessengerOutputDTO : MessengerOutputDTOBase { }

    [FunctionOutput]
    public class MessengerOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }




}
