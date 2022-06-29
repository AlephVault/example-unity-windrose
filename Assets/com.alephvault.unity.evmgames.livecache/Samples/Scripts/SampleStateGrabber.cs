using System;
using System.Threading.Tasks;
using AlephVault.Unity.EVMGames.LiveCache.Types.Cache;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Exception = AlephVault.Unity.EVMGames.LiveCache.Types.Exception;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Samples
    {
        public class SampleStateGrabber : StateGrabber
        {
            private void Start()
            {
                Debug.Log("Starting grab loop");
                StartGrabLoop();
            }

            protected override async Task ProcessResponseItem(JObject item)
            {
                Debug.Log($"Item: {item}");
            }

            protected override async Task OnGrabError(Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}