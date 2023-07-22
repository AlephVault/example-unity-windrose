namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Types
        {
            /// <summary>
            ///   A matched hand rank and indices, according to certain source.
            /// </summary>
            public class MatchedHand5
            {
                /// <summary>
                ///   Index of the first card.
                /// </summary>
                public int Card0;

                /// <summary>
                ///   Index of the second card.
                /// </summary>
                public int Card1;

                /// <summary>
                ///   Index of the third card.
                /// </summary>
                public int Card2;

                /// <summary>
                ///   Index of the fourth card.
                /// </summary>
                public int Card3;
                
                /// <summary>
                ///   Index of the fifth card.
                /// </summary>
                public int Card4;

                /// <summary>
                ///   The hand rank.
                /// </summary>
                public int Rank;

                /// <summary>
                ///   Completely specifies the matched cards.
                /// </summary>
                /// <param name="rank">The rank/score</param>
                /// <param name="card0">The 1st card index in the source</param>
                /// <param name="card1">The 2nd card index in the source</param>
                /// <param name="card2">The 3rd card index in the source</param>
                /// <param name="card3">The 4th card index in the source</param>
                /// <param name="card4">The 5th card index in the source</param>
                public MatchedHand5(int rank, int card0, int card1, int card2, int card3, int card4)
                {
                    Rank = rank;
                    Card0 = card0;
                    Card1 = card1;
                    Card2 = card2;
                    Card3 = card3;
                    Card4 = card4;
                }

                /// <summary>
                ///   Specifies the matched cards by using rank and leaving
                ///   the indices by default 0 .. 4.
                /// </summary>
                /// <param name="rank">The rank/score</param>
                public MatchedHand5(int rank) : this(rank, 0, 1, 2, 3, 4) {}
            }
        }
    }
}