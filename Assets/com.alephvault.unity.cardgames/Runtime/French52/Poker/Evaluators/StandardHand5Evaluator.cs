using AlephVault.Unity.CardGames.French52.Types;

namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Poker
        {
            namespace Evaluators
            {
                /// <summary>
                ///   An evaluator for standard games.
                /// </summary>
                public class StandardHand5Evaluator : Hand5Evaluator
                {
                    /// <summary>
                    ///   A straight flush.
                    /// </summary>
                    public const int StraightFlush = 8;

                    /// <summary>
                    ///   A flush.
                    /// </summary>
                    public const int Flush = 5;

                    /// <summary>
                    ///   A straight.
                    /// </summary>
                    public const int Straight = 4;
                    
                    /// <summary>
                    ///   Evaluates the hand power using standard rules.
                    /// </summary>
                    /// <param name="hand">The hand to evaluate</param>
                    /// <returns>The evaluation score</returns>
                    public override int Evaluate(int[] hand)
                    {
                        // Obtains the hand for standard evaluation.
                        hand = hand.PrepareForEvaluation();
                        
                        // Special characteristics: sequence and suit.
                        bool isStraight = hand.IsStraight();
                        bool isFlush = hand.IsSuited();
                        
                        // Check for straight or flushed straight.
                        if (isStraight)
                        {
                            int rankType = isFlush ? StraightFlush : Straight;
                            if (hand[0] == 12 && hand[1] == 3)
                            {
                                // A 5 4 3 2. Use 5 (encoded as 3).
                                return Pack(rankType, 3, 0, 0, 0, 0);
                            }
                            // Use the topmost card as rank value.
                            return Pack(rankType, hand[0], 0, 0, 0, 0);
                        }

                        // Check for simple flush.
                        if (isFlush)
                        {
                            return Pack(
                                Flush, hand[0], hand[1], hand[2],hand[3], hand[4]
                            );
                        }
                        
                        // Otherwise, return according to the match evaluators.
                        return MatchFourOfAKind(hand) ?? MatchFullHouse(hand) ?? Match3OfAKind(hand) ??
                            MatchDoublePair(hand) ?? MatchPair(hand) ?? Pack(
                                Bust, hand[0], hand[1], hand[2],hand[3], hand[4]
                            );
                    }
                }
            }
        }
    }
}