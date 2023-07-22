namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Poker
        {
            namespace Evaluators
            {
                /// <summary>
                ///   An evaluator for a Poker game of 5-cards hands
                ///   (out of 52 standard cards).
                /// </summary>
                public abstract class Hand5Evaluator
                {
                    /// <summary>
                    ///   Unmatched cards.
                    /// </summary>
                    protected static int Bust = 0;
                    
                    /// <summary>
                    ///   A single pair.
                    /// </summary>
                    protected static int Pair = 1;
                    
                    /// <summary>
                    ///   A double pair.
                    /// </summary>
                    protected static int DoublePair = 2;
                    
                    /// <summary>
                    ///   Three of a kind (and two kickers).
                    /// </summary>
                    protected static int ThreeOfAKind = 3;
                    
                    /// <summary>
                    ///   Full house.
                    /// </summary>
                    /// <remarks>Notice how full house has less odds than flush only on 52-cards decks</remarks>
                    protected static int FullHouse = 6;
                    
                    /// <summary>
                    ///   Four of a kind (and a kicker).
                    /// </summary>
                    protected static int FourOfAKind = 7;

                    /// <summary>
                    ///   Packs a sparse hand power. It doesn't serve the purpose for absolute power
                    ///   computation (i.e. strength ratio over 52c5 combinations) but serves for
                    ///   the comparisons.
                    /// </summary>
                    /// <param name="rankType">The rank type</param>
                    /// <param name="card1">The 1st card. Most significant one</param>
                    /// <param name="card2">The 2nd card</param>
                    /// <param name="card3">The 3rd card</param>
                    /// <param name="card4">The 4th card</param>
                    /// <param name="card5">The 5th card. Least significant one</param>
                    /// <returns>The packed value</returns>
                    protected int Pack(int rankType, int card1, int card2, int card3, int card4, int card5)
                    {
                        return (rankType % 16) << 20 |
                               (card1 % 16) << 16 |
                               (card2 % 16) << 12 |
                               (card3 % 16) << 8 |
                               (card4 % 16) << 4 |
                               (card5 % 16);
                    }
                    
                    /// <summary>
                    ///   Evaluates the hand power.
                    /// </summary>
                    /// <returns>The evaluation score</returns>
                    public abstract int Evaluate();
                }
            }
        }
    }
}