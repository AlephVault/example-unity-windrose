using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.RPC.Eth.Blocks
{
    public interface ILastConfirmedBlockNumberService
    {
        Task<BigInteger> GetLastConfirmedBlockNumberAsync(BigInteger? waitForConfirmedBlockNumber, CancellationToken cancellationToken);
    }
}