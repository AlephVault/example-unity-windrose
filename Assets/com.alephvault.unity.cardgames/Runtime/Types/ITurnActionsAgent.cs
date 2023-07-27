using AlephVault.Unity.CardGames.Utils.AgentTurns;

namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   Agents can attend turns' prompts and answer them.
        /// </summary>
        public interface ITurnActionsAgent
        {
            /// <summary>
            ///   Sends the prompt.
            /// </summary>
            public void SendPrompt(params AgentTurnPromptOption[] options);

            /// <summary>
            ///   Clears the current answer, if any.
            /// </summary>
            public void ClearAnswer();

            /// <summary>
            ///   Gets the agent answer. It will be null if the agent
            ///   did not answer yet.
            /// </summary>
            public AgentTurnAnswer GetAnswer();

            /// <summary>
            ///   Tell the agent the received prompt was valid.
            /// </summary>
            public void ACKValidPrompt();

            /// <summary>
            ///   Tell the agent the received prompt was invalid.
            /// </summary>
            public void ACKInvalidPrompt();

            /// <summary>
            ///   Tell the agent that it timed out.
            /// </summary>
            public void TimedOut();
        }
    }
}