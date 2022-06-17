
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;
using AlephVault.Unity.EVMGames.Nethereum.RPC.Infrastructure;
using Newtonsoft.Json.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Quorum.RPC.Permission
{
    ///<Summary>
    /// Returns a list of roles in the network.
    /// 
    /// Parameters
    /// None
    /// 
    /// Returns
    /// result: array of objects - list of role objects with the following fields:
    /// 
    /// access: number - account access
    /// 
    /// active: boolean - indicates if the role is active or not
    /// 
    /// isAdmin: boolean - indicates if the role is organization admin role
    /// 
    /// isVoter: boolean - indicates if the role is enabled for voting - applicable only for network admin role
    /// 
    /// orgId: string - organization ID to which the role is linked
    /// 
    /// roleId: string - unique role ID    
    ///</Summary>
    public interface IQuorumPermissionRoleList
    {
        Task<JArray> SendRequestAsync(object id);
        RpcRequest BuildRequest(object id = null);
    }

    ///<Summary>
/// Returns a list of roles in the network.
/// 
/// Parameters
/// None
/// 
/// Returns
/// result: array of objects - list of role objects with the following fields:
/// 
/// access: number - account access
/// 
/// active: boolean - indicates if the role is active or not
/// 
/// isAdmin: boolean - indicates if the role is organization admin role
/// 
/// isVoter: boolean - indicates if the role is enabled for voting - applicable only for network admin role
/// 
/// orgId: string - organization ID to which the role is linked
/// 
/// roleId: string - unique role ID    
///</Summary>
    public class QuorumPermissionRoleList : GenericRpcRequestResponseHandlerNoParam<JArray>, IQuorumPermissionRoleList
    {
        public QuorumPermissionRoleList(IClient client) : base(client, ApiMethods.quorumPermission_roleList.ToString()) { }
    }

}
