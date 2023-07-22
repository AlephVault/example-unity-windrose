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
                    ///   Matches two of a kind. This one will be executed after
                    ///   other (more powerful & strict) matches failed.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>A packed Pair rank, or null</returns>
                    protected int? MatchPair(int[] hand)
                    {
                        if (hand[0] == hand[1])
                            return Pack(
                                Pair, hand[0], hand[2], hand[3], hand[4], 0
                            );
                        if (hand[1] == hand[2])
                            return Pack(
                                Pair, hand[1], hand[0], hand[3], hand[4], 0
                            );
                        if (hand[2] == hand[3])
                            return Pack(
                                Pair, hand[2], hand[0], hand[1], hand[4], 0
                            );
                        if (hand[3] == hand[4])
                            return Pack(
                                Pair, hand[3], hand[0], hand[1], hand[2], 0
                            );
                        return null;
                    }

                    /// <summary>
                    ///   Matches two pairs. This one will be executed after other
                    ///   (more powerful & strict) matches failed.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>A packed Double Pair rank, or null</returns>
                    protected int? MatchDoublePair(int[] hand)
                    {
                        if (hand[0] == hand[1])
                        {
                            if (hand[2] == hand[3])
                            {
                                return Pack(
                                    DoublePair, hand[0], hand[2], hand[4], 0, 0
                                );
                            }
                            if (hand[3] == hand[4])
                            {
                                return Pack(
                                    DoublePair, hand[0], hand[4], hand[2], 0, 0
                                );
                            }
                        }
                        else if (hand[1] == hand[2] && hand[3] == hand[4])
                        {
                            return Pack(
                                DoublePair, hand[2], hand[4], hand[0], 0, 0
                            );
                        }

                        return null;
                    }

                    /// <summary>
                    ///   Matches 3 of a kind. This one will be executed after
                    ///   other (more powerful & strict) matches failed.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>A packed Three of a Kind rank, or null</returns>
                    public int? Match3OfAKind(int[] hand)
                    {
                        if (hand[0] == hand[1] && hand[0] == hand[2])
                        {
                            return Pack(ThreeOfAKind, hand[0], hand[3], hand[4], 0, 0);
                        }
                        if (hand[1] == hand[2] && hand[1] == hand[3])
                        {
                            return Pack(ThreeOfAKind, hand[1], hand[0], hand[4], 0, 0);
                        }
                        if (hand[2] == hand[3] && hand[3] == hand[4])
                        {
                            return Pack(ThreeOfAKind, hand[2], hand[0], hand[1], 0, 0);
                        }

                        return null;
                    }

                    /// <summary>
                    ///   Matches full house.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>A packed Full House rank, or null</returns>
                    public int? MatchFullHouse(int[] hand)
                    {
                        if (hand[0] == hand[1] && hand[0] == hand[2] && hand[3] == hand[4])
                        {
                            return Pack(FullHouse, hand[0], hand[3], 0, 0, 0);
                        }
                        if (hand[0] == hand[1] && hand[2] == hand[3] && hand[2] == hand[4])
                        {
                            return Pack(FullHouse, hand[2], hand[0], 0, 0, 0);
                        }

                        return null;
                    }

                    /// <summary>
                    ///   Matches four of a kind.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>A packed Four of a Kind, or null</returns>
                    public int? MatchFourOfAKind(int[] hand)
                    {
                        if (hand[0] == hand[1] && hand[0] == hand[2] && hand[0] == hand[3])
                        {
                            return Pack(FullHouse, hand[0], hand[4], 0, 0, 0);
                        }
                        if (hand[1] == hand[2] && hand[1] == hand[3] && hand[1] == hand[4])
                        {
                            return Pack(FullHouse, hand[1], hand[0], 0, 0, 0);
                        }

                        return null;
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