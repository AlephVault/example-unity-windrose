
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.IBFT
{
///<Summary>
/// Returns the current candidates which the node tries to vote in or out.
/// 
/// Parameters
/// None
/// 
/// Returns
/// result: map of strings to booleans - current candidates map    
///</Summary>
    public interface IIstanbulCandidates
    {
        Task<JObject> SendRequestAsync(object id);
        RpcRequest BuildRequest(object id = null);
    }

    ///<Summary>
/// Returns the current candidates which the node tries to vote in or out.
/// 
/// Parameters
/// None
/// 
/// Returns
/// result: map of strings to booleans - current candidates map    
///</Summary>
    public class IstanbulCandidates : GenericRpcRequestResponseHandlerNoParam<JObject>, IIstanbulCandidates
    {
        public IstanbulCandidates(IClient client) : base(client, ApiMethods.istanbul_candidates.ToString()) { }
    }

}
