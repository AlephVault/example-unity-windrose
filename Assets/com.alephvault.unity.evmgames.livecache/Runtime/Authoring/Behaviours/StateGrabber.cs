using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using AlephVault.Unity.Support.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        namespace Cache
        {
            /// <summary>
            ///   Grabs the state periodically and processed it.
            /// </summary>
            public abstract class StateGrabber : MonoBehaviour
            {
                /// <summary>
                ///   The interval for which this task will run.
                ///   The minimum, and default value, is 10.
                /// </summary>
                [SerializeField]
                private float interval = 10f;

                /// <summary>
                ///   The API key to use. This API key is meant
                ///   to be set publicly..
                /// </summary>
                public string ApiKey;

                /// <summary>
                ///   The cache URL.
                /// </summary>
                public string CacheURL = "http://localhost:6666";

                /// <summary>
                ///   The resource key for the state.
                /// </summary>
                public string StateResourceKey = "evm-state";

                /// <summary>
                ///   Tells whether the loop is running or not.
                /// </summary>
                public bool Running { get; private set; }

                protected void Awake()
                {
                    if (interval < 10) interval = 10;
                }
                
                /// <summary>
                ///   Processes a response item from the periodic grab.
                ///   It is useful to pay attention to the 'contract-key'
                ///   field of the item and then use <c>item.ToObject{T}</c>.
                /// </summary>
                /// <param name="item">The item to process</param>
                protected abstract Task ProcessResponseItem(JObject item);

                /// <summary>
                ///   Processes an exception produced on a grab operation.
                /// </summary>
                /// <param name="exception">The exception to process</param>
                protected virtual Task OnGrabError(Exception exception)
                {
                    return Task.CompletedTask;
                }

                /// <summary>
                ///   See <see cref="interval" />.
                /// </summary>
                public float Interval
                {
                    get => interval;
                    set => interval = value < 10 ? 10 : value;
                }

                /// <summary>
                ///   Starts a grab loop. It will stop when the next
                ///   iteration finds that <see cref="StopGrabLoop"/>
                ///   was invoked.
                /// </summary>
                /// <param name="realTime">Whether to use real-time intervals or perhaps-scaled ones</param>
                public async void StartGrabLoop(bool realTime = false)
                {
                    if (Running) return;
                    Running = true;
                    Root root = new Root(CacheURL, new Authorization("Bearer", ApiKey));
                    StateCacheHandler stateHandler = new StateCacheHandler(root, StateResourceKey);
                    while (await GrabOperation(realTime, stateHandler)) {}
                    Running = false;
                }

                /// <summary>
                ///   Stops the current grab loop. This will not stop
                ///   the current iteration, if any, but prevents new
                ///   ones from starting.
                /// </summary>
                public void StopGrabLoop()
                {
                    Running = false;
                }

                // This is the grab operation. It waits for a certain
                // interval and then creates a grabber instance and
                // runs it.
                private async Task<bool> GrabOperation(bool realTime, StateCacheHandler stateHandler)
                {
                    float current = 0;
                    if (realTime)
                    {
                        while (current < interval)
                        {
                            await Tasks.Blink();
                            current += Time.unscaledDeltaTime;
                        }
                    }
                    else
                    {
                        while (current < interval)
                        {
                            await Tasks.Blink();
                            current += Time.deltaTime;
                        }
                    }

                    try
                    {
                        Debug.Log("Performing grab iteration");
                        if (!Running) return false;
                        while (true)
                        {
                            Result<JObject[], string> result = await stateHandler.Grab();
                            bool found = false;
                            foreach (JObject entry in result.Element)
                            {
                                await ProcessResponseItem(entry);
                                found = true;
                            }
                            if (result.Code != ResultCode.Ok)
                            {
                                throw new Exception(500, "grab");
                            }
                            // If no events were found, break. Otherwise,
                            // remain for a next iteration.
                            if (!found) break;
                        }
                        if (!Running) return false;
                        return true;
                    }
                    catch (Exception e)
                    {
                        await OnGrabError(e);
                        return false;
                    }
                }
            }
        }
    }
}