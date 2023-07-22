namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Types
        {
            /**
             * Convenience utilities for a hand (a sequence of cards).
             */
            public static class HandMethods
            {
                /// <summary>
                ///   Sorts (as new hand) the cards by their value and, for lowball
                ///   games, the values are re-mapped: Ace (12) changes to 0 while
                ///   all the other values are increased by 1.
                /// </summary>
                /// <param name="hand">The hand. Typically, 5 cards</param>
                /// <param name="lowball">Whether to apply lowball encoding or not</param>
                public static int[] PrepareForEvaluation(this int[] hand, bool lowball = false)
                {
                    int length = hand.Length;
                    int[] newHand = (int[])hand.Clone();
                    if (lowball)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            newHand[i] = newHand[i].Value() == 12 ? newHand[i] - 12 : newHand[i] + 1;
                        }
                    }
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = i + 1; j < length; j++)
                        {
                            if (newHand[i].Value() < newHand[j].Value())
                            {
                                (newHand[i], newHand[j]) = (newHand[j], newHand[i]);
                            }
                        }
                    }

                    return newHand;
                }

                /// <summary>
                ///   Tells whether a hand is suited.
                /// </summary>
                /// <param name="hand">The hand to evaluate</param>
                /// <returns>Whether it is on-suit or not</returns>
                public static bool IsSuited(this int[] hand)
                {
                    int length = hand.Length;
                    if (length == 0) return true;
                    int suit = hand[0].Suit();
                    for (int i = 1; i < length; i++)
                    {
                        if (suit != hand[i].Suit()) return false;
                    }

                    return true;
                }

                /// <summary>
                ///   Tells whether a hand is straight or not.
                /// </summary>
                /// <param name="hand">The hand to evaluate</param>
                /// <param name="lowball">Whether to use lowball mode</param>
                /// <returns>Whether it is straight or not</returns>
                public static bool IsStraight(this int[] hand, bool lowball = false)
                {
                    int length = hand.Length;
                    if (length == 0) return true;
                    
                    // First, a natural straight: T, T-1, T-2, ...
                    int top = hand[0].Value();
                    bool natural = true;
                    for (int i = 1; i < length; i++)
                    {
                        if (hand[i].Value() != top - i)
                        {
                            natural = false;
                            break;
                        }
                    }
                    if (natural) return true;

                    // In standard modes, (12 3 2 1 0) stands for (A 5 4 3 2)
                    // which is an accepted lowest straight. So we return false
                    // unless this is standard and that combination of values.
                    return !lowball && hand[0].Value() == 12 && hand[1].Value() == 3 && hand[2].Value() == 2 &&
                                       hand[3].Value() == 1 && hand[4].Value() == 0;
                }
            }
        }
    }
}