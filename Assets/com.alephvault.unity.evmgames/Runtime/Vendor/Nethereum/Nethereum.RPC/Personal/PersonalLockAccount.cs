using System;
using System.Threading.Tasks;
 
using AlephVault.Unity.EVMGames.Nethereum.Hex.HexConvertors.Extensions;
using AlephVault.Unity.EVMGames.Nethereum.JsonRpc.Client;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Personal
{
    /// <Summary>
    ///     Removes the private key with given address from memory. The account can no longer be used to send transactions.
    /// </Summary>
    public class PersonalLockAccount : RpcRequestResponseHandler<bool>, IPersonalLockAccount
    {
        public PersonalLockAccount(IClient client) : base(client, ApiMethods.personal_lockAccount.ToString())
        {
        }

        public Task<bool> SendRequestAsync(string account, object id = null)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            return base.SendRequestAsync(id, account.EnsureHexPrefix());
        }

        public RpcRequest BuildRequest(string account, object id = null)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            return base.BuildRequest(id, account.EnsureHexPrefix());
        }
    }
}