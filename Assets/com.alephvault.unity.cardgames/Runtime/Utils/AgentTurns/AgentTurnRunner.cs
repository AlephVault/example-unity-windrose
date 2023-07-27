using System;
using System.Linq;
using System.Threading.Tasks;
using AlephVault.Unity.CardGames.Types;
using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.CardGames
{
    namespace Utils
    {
        namespace AgentTurns
        {
            /// <summary>
            ///   Runs a turn in a specific agent. Running a turn means only:
            ///   1. Launching a timer which ends after some time, or
            ///      when it is killed "for good".
            ///   2. Sending the prompt to the agent.
            ///   3. Loop:
            ///      - Wait for an answer from the agent.
            ///      - If it is valid, break the loop.
            ///   4. Process the (valid) answer.
            /// </summary>
            public abstract class AgentTurnRunner
            {
                /// <summary>
                ///   Whether this turn is already consumed or not.
                /// </summary>
                public bool IsConsumed { get; private set; }
                
                /// <summary>
                ///   Whether this turn's answer is set or not.
                /// </summary>
                public AgentTurnAnswer AgentAnswer { get; private set; }
                
                /// <summary>
                ///   The agent this turn stands for.
                /// </summary>
                public ITurnAttendingAgent Agent { get; private set; }
                
                /// <summary>
                ///   The standard time this turn stands for.
                /// </summary>
                public int Time { get; private set; }

                // What to do on timeout.
                private Func<Task> onTimeout;
                
                /// <summary>
                ///   Processes the full turn.
                /// </summary>
                public async Task Turn()
                {
                    if (IsConsumed) return;
                    AgentTurnWaiter.Wait(Agent, Time, () => AgentAnswer != null, async () =>
                    {
                        IsConsumed = true;
                        Agent.TimedOut();
                        await OnTimeout();
                    }, async () =>
                    {
                        IsConsumed = true;
                        await OnComplete();
                    });
                    await Feedback();
                }

                // Performs a full feedback to get a valid answer from the agent.
                private async Task Feedback()
                {
                    Agent.SendPrompt(GetPrompt());
                    Agent.ClearAnswer();
                    while (!IsConsumed)
                    {
                        AgentTurnAnswer answer = Agent.GetAnswer();
                        Agent.ClearAnswer();

                        if (answer != null)
                        {
                            // Validate the action. If it is valid, processes it.
                            // Otherwise, it deems it as invalid.
                            if (GetPrompt().Any(opt => opt.Accepts(answer)))
                            {
                                Agent.ACKValidPrompt();
                                AgentAnswer = answer;
                            }
                            else
                            {
                                Agent.ACKInvalidPrompt();
                            }
                        }

                        await Tasks.Blink();
                    }
                }

                /// <summary>
                ///   Prepares the prompt. This might depend on the agent status.
                ///   This prompt is typically determined once, based on the agent
                ///   status against the table this hand, and only at turn-construction
                ///   time (which is typically immediate).
                /// </summary>
                protected abstract AgentTurnPromptOption[] GetPrompt();
                
                /// <summary>
                ///   Processes the turn completion (an explicit action from the agent).
                ///   By this point, <see cref="AgentAnswer" /> is set and can fully be
                ///   processed (i.e. into the turn context).
                /// </summary>
                protected abstract Task OnComplete();

                /// <summary>
                ///   Processes the turn timeout (and triggers a default agent action).
                /// </summary>
                protected abstract Task OnTimeout();
            }
        }
    }
}