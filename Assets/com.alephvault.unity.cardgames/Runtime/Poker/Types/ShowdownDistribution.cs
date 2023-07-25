namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A showdown pot distribution is aware of:
            ///   - The agent who won.
            ///   - Which pot index.
            ///   - How much they won (perhaps splitting the pot).
            /// </summary>
            public class ShowdownDistribution
            {
                /// <summary>
                ///   The agent (perhaps one of many).
                /// </summary>
                public readonly IShowdownAgent ShowdownAgent;

                /// <summary>
                ///   The pot index.
                /// </summary>
                public readonly int PotIndex;

                /// <summary>
                ///   The amount.
                /// </summary>
                public readonly int Amount;

                public ShowdownDistribution(IShowdownAgent showdownAgent, int potIndex, int amount)
                {
                    ShowdownAgent = showdownAgent;
                    PotIndex = potIndex;
                    Amount = amount;
                }
            }
        }
    }
}