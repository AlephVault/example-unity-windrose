using System.Threading.Tasks;

using AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.DTOs;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Standards.ProofOfHumanity.ContractDefinition;
using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.QueryHandlers.MultiCall;
using AlephVault.Unity.EVMGames.Nethereum.Contracts.Constants;
using System.Linq;

namespace AlephVault.Unity.EVMGames.Nethereum.Contracts.Standards.ProofOfHumanity
{
    public partial class ProofOfHumanityContractService
    {
#if !DOTNET35
        public async Task<List<IsRegisteredInfo>> AreRegisteredQueryUsingMulticallAsync(IEnumerable<string> registeredAddresses,
          BlockParameter block = null,
          int numberOfCallsPerRequest = MultiQueryHandler.DEFAULT_CALLS_PER_REQUEST,
          string multiCallAddress = CommonAddresses.MULTICALL_ADDRESS)
        {
            var registeredCalls = new List<MulticallInputOutput<IsRegisteredFunction, IsRegisteredOutputDTO>>();
            foreach (var registeredAddress in registeredAddresses)
            {
             
                    var registeredCall  = new IsRegisteredFunction() { SubmissionID = registeredAddress };
                    registeredCalls.Add(new MulticallInputOutput<IsRegisteredFunction, IsRegisteredOutputDTO>(registeredCall,
                    ContractAddress));

            }

            var multiqueryHandler = this._ethApiContractService.GetMultiQueryHandler(multiCallAddress);
            var results = await multiqueryHandler.MultiCallAsync(block, numberOfCallsPerRequest, registeredCalls.ToArray());
            return registeredCalls.Select(x => new IsRegisteredInfo()
            {
                IsRegistered = x.Output.IsRegistered,
                Address = x.Input.SubmissionID,
            }).ToList();
        }
#endif
    }
}
