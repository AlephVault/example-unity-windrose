using System;
using AlephVault.Unity.Support.Generic.Vendor.IUnified.Authoring.Types;
using GameMeanMachine.Unity.NetRose.Authoring.Protocols;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                namespace World
                {
                    [RequireComponent(typeof(Scope))]
                    public partial class ScopeServerSide : MonoBehaviour
                    {
                        // This is the list of actions that are pending to
                        // be executed. These actions wrap things that must
                        // be waited for somewhere else, instead of using
                        // a mutex for it.
                        private ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

                        // Queues an action to be executed later. Returns
                        // the task that will be resolved when the action
                        // is finished. The task will be actually executed
                        // in the main thread.
                        private Task QueueAction(Action action)
                        {
                            if (action == null) return null;
                            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
                            actions.Enqueue(() => {
                                try
                                {
                                    action();
                                    source.SetResult(true);
                                }
                                catch(Exception e)
                                {
                                    source.SetException(e);
                                }
                            });
                            return source.Task;
                        }

                        // Runs all the queued actions. If any of them throws
                        // an exception, it will be sent as task exception but
                        // it will not halt the execution of this whole run.
                        private void RunPendingActions()
                        {
                            while (actions.TryDequeue(out Action action)) {
                                action();
                            };
                        }
                    }
                }
            }
        }
    }
}
