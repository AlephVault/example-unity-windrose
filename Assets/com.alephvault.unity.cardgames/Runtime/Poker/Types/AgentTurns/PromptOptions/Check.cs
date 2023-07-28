namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Check" option. Available when "Bet" is available, but
                ///   as the only alternative (i.e. betting nothing).
                /// </summary>
                public class Check : AgentTurnPromptOption
                {
                    public Check() : base("BET", 0, 0) {}
                }
            }
        }
    }
}