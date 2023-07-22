namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace MatchedHands
        {
            /// <summary>
            ///   A matched hand.
            /// </summary>
            public interface IMatchedHand
            {
                /// <summary>
                ///   The rank of the hand.
                /// </summary>
                public int Rank();

                /// <summary>
                ///   The card indices (for games making
                ///   a hand out of a bigger set of known
                ///   cards).
                /// </summary>
                public int[] Indices();
            }
        }
    }
}