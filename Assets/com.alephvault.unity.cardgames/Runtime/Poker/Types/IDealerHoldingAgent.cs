namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   Tells whether an agent has the dealer or not. This
            ///   interface will be used for agents in games where the
            ///   dealer token is used (games like Stud use no concept
            ///   of dealer at all, and thus the only available method
            ///   <see cref="IsDealer"/> returns false).
            /// </summary>
            public interface IDealerHoldingAgent
            {
                /// <summary>
                ///   Tells whether the current game is a Dealer one
                ///   or not, even if the agent doesn't currently hold
                ///   the dealer token.
                /// </summary>
                public bool InDealerGame();
                
                /// <summary>
                ///   Returns whether the current game is a Dealer one
                ///   and also returns whether the current agent holds
                ///   the dealer or not.
                /// </summary>
                public bool IsDealer();
            }
        }
    }
}