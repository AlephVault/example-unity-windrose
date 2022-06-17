
using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Privacy
{
    ///<Summary>
    /// Get the address of the privacy precompile contract, to be used as the to address for privacy marker transactions.
    /// 
    /// Parameters
    /// None
    /// 
    /// Returns
    /// string - contract address for the privacy precompile in hex format    
    ///</Summary>
    public interface IEthGetPrivacyPrecompileAddress
    {
        Task<String> SendRequestAsync(object id);
        RpcRequest BuildRequest(object id = null);
    }

    ///<Summary>
/// Get the address of the privacy precompile contract, to be used as the to address for privacy marker transactions.
/// 
/// Parameters
/// None
/// 
/// Returns
/// string - contract address for the privacy precompile in hex format    
///</Summary>
    public class EthGetPrivacyPrecompileAddress : GenericRpcRequestResponseHandlerNoParam<string>, IEthGetPrivacyPrecompileAddress
    {
        public EthGetPrivacyPrecompileAddress(IClient client) : base(client, ApiMethods.eth_getPrivacyPrecompileAddress.ToString()) { }
    }

}
