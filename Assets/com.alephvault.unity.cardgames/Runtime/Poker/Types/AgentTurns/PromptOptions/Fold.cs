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
                public class Fold : AgentTurnPromptOption
                {
                    public Fold() : base("FOLD", 0, 0) {}
                }
            }
        }
    }
}