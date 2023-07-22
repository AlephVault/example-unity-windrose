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
                ///   An evaluator for lowball games.
                /// </summary>
                public class LowballHandsEvaluator : Hand5Evaluator
                {
                    public override int Evaluate(int[] hand)
                    {
                        // Obtains the hand for lowball evaluation.
                        hand = hand.PrepareForEvaluation(true);
                        
                        // No straight / flush here. Return according to the match evaluators.
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