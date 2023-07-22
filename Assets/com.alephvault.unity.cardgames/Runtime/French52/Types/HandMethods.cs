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
            }
        }
    }
}