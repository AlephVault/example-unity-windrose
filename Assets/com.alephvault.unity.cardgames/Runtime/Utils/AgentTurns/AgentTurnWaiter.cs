using System;
using System.Threading.Tasks;
using AlephVault.Unity.Support.Utils;
using UnityEngine;

namespace AlephVault.Unity.CardGames
{
    using Types;

    namespace Utils
    {
        namespace AgentTurns
        {
            /// <summary>
            ///   Creates a timer for an agent's turn.
            ///   When the turn is exhausted, a default
            ///   or absence action will be executed on
            ///   behalf of the agent.
            /// </summary>
            public class AgentTurnWaiter
            {
                // Waits a certain time until a condition is given.
                private static async Task<float> WaitTime(float time, Func<bool> completionCheck)
                {
                    float remainingTime = time;
                    while (remainingTime > 0)
                    {
                        await Tasks.Blink();
                        if (completionCheck()) break;
                        remainingTime -= Time.unscaledDeltaTime;
                    }

                    return remainingTime;
                }
                
                /// <summary>
                ///   Waits a certain amount of time for an agent.
                ///   If the time is consumed before completion, then
                ///   waits a certain amount of time from the agent's
                ///   time pool. After that full or partial consumption,
                ///   then the agent keeps the new time pool. When the
                ///   time pool reaches 0 (or is already 0 on the first
                ///   evaluation, after the standard time) the agent is
                ///   considered as timed out.
                /// </summary>
                /// <param name="agent">The agent</param>
                /// <param name="amount">The standard amount to wait for</param>
                /// <param name="completionCheck">The completion check</param>
                /// <param name="onTimeout">What to do on timeout</param>
                /// <param name="onComplete">What to do on completion</param>
                public static async void Wait(
                    ITimePoolHoldingAgent agent, int amount, Func<bool> completionCheck,
                    Func<Task> onTimeout, Func<Task> onComplete
                )
                {
                    float remainingTime = await WaitTime(amount, completionCheck);
                    if (remainingTime > 0)
                    {
                        await onComplete();
                        return;
                    }
                    remainingTime = await WaitTime(remainingTime + agent.GetTimePool(), completionCheck);
                    await (remainingTime < 0 ? onTimeout() : onComplete());
                    agent.SetTimePool((int) Mathf.Ceil(Values.Max(0, remainingTime)));
                }
            }
        }
    }
}