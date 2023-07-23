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
            public class ShowdownPotDistribution
            {
                /// <summary>
                ///   The agent (perhaps one of many).
                /// </summary>
                public readonly IAgent Agent;

                /// <summary>
                ///   The pot index.
                /// </summary>
                public readonly int PotIndex;

                /// <summary>
                ///   The amount.
                /// </summary>
                public readonly int Amount;

                public ShowdownPotDistribution(IAgent agent, int potIndex, int amount)
                {
                    Agent = agent;
                    PotIndex = potIndex;
                    Amount = amount;
                }
            }
        }
    }
}