using AlephVault.Unity.CardGames.Utils.AgentTurns;

namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   Agents can attend turns. See <see cref="AgentTurnRunner" />
        ///   for more details on how a turn goes, but it is essentially
        ///   made of prompts and (valid) answers.
        /// </summary>
        public interface ITurnAttendingAgent : ITurnActionsAgent, ITimePoolHoldingAgent {}
    }
}