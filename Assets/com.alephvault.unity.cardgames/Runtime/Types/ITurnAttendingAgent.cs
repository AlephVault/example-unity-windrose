namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   Agents can attend turns. Attending a turn means only:
        ///   1. Launching a timer which ends after some time, or
        ///      when it is killed "for good".
        ///   2. Sending the prompt to the agent.
        ///   3. Loop:
        ///      - Wait for an answer from the agent.
        ///      - If it is valid, break the loop.
        ///   4. Process the (valid) answer.
        /// </summary>
        public interface ITurnAttendingAgent
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
        }
    }
}