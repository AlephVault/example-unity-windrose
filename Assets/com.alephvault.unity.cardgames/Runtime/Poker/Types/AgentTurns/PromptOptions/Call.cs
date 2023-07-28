namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Call" option. Available when there's a previous unmatched
                ///   bet from previous players this round.
                /// </summary>
                public class Call : AgentTurnPromptOption
                {
                    public Call() : base("CALL", 0, 0) {}
                }
            }
        }
    }
}