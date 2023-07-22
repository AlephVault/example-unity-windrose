using AlephVault.Unity.CardGames.Types.MatchedHands;

namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Types
        {
            namespace Matches
            {
                /// <summary>
                ///   A matched hand rank and indices, according to certain source.
                /// </summary>
                public class MatchedHand5 : IMatchedHand
                {
                    private int rank;
                    private int[] indices;

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
                        this.rank = rank;
                        indices = new[] {card0, card1, card2, card3, card4};
                    }

                    /// <summary>
                    ///   Specifies the matched cards by using rank and leaving
                    ///   the indices by default 0 .. 4.
                    /// </summary>
                    /// <param name="rank">The rank/score</param>
                    public MatchedHand5(int rank) : this(rank, 0, 1, 2, 3, 4) {}

                    /// <inheritdoc cref="IMatchedHand.Rank" />
                    public int Rank()
                    {
                        return rank;
                    }

                    /// <inheritdoc cref="IMatchedHand.Rank" />
                    public int[] Indices()
                    {
                        return indices;
                    }
                }
            }
        }
    }
}
