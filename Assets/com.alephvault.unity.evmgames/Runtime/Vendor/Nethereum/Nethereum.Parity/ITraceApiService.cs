using AlephVault.Unity.EVMGames.Nethereum.Parity.RPC.Trace;

namespace AlephVault.Unity.EVMGames.Nethereum.Parity
{
    public interface ITraceApiService
    {
        ITraceBlock TraceBlock { get; }
        ITraceCall TraceCall { get; }
        ITraceFilter TraceFilter { get; }
        ITraceGet TraceGet { get; }
        ITraceRawTransaction TraceRawTransaction { get; }
        ITraceTransaction TraceTransaction { get; }
    }
}