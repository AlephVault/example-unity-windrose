namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A payment of a user in a central pot.
            /// </summary>
            public class CentralPotPayment
            {
                /// <summary>
                ///   The user which afforded the pot.
                /// </summary>
                public readonly IPlayerAgent Player;

                /// <summary>
                ///   The index of the pot.
                /// </summary>
                public readonly int PotIndex;

                /// <summary>
                ///   The afforded amount.
                /// </summary>
                public readonly int Amount;

                public CentralPotPayment(IPlayerAgent player, int potIndex, int amount)
                {
                    Player = player;
                    PotIndex = potIndex;
                    Amount = amount;
                }
            }
        }
    }
}