namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Fold" option. Available when "Call" is available.
                /// </summary>
                public class Call : AgentTurnPromptOption
                {
                    public Call() : base("BET", 0, 0) {}
                }
            }
        }
    }
}