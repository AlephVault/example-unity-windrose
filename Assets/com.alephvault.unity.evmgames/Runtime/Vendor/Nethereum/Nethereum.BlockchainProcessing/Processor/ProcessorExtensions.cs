using System;
using System.Threading.Tasks;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.Processor
{
    public static class ProcessorExtensions
    {
        public static void AddSynchronousProcessorHandler<T>(this IProcessor<T> processor, Action<T> action)
        {
            processor.AddProcessorHandler(t => { action(t); return Task.FromResult(0); });
        }
    }
}
