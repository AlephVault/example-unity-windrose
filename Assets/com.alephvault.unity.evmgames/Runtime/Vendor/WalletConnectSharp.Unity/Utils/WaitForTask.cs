using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Unity.Utils
{
    public class WaitForTask : CustomYieldInstruction
    {
        private Task source;

        public Task Source
        {
            get { return source; }
        }

        public override bool keepWaiting
        {
            get { return !source.IsCompleted && !source.IsFaulted && !source.IsCanceled; }
        }
        
        public WaitForTask(Task task)
        {
            this.source = task;
        }
    }
}