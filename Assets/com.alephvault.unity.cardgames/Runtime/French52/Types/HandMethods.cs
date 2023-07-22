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
                /**
                 * In-place sorts the cards by their value.
                 */
                public static void SortByValue(this int[] hand)
                {
                    int length = hand.Length;
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = i + 1; j < length; j++)
                        {
                            if (hand[i].Value() < hand[j].Value())
                            {
                                (hand[i], hand[j]) = (hand[j], hand[i]);
                            }
                        }
                    }
                }
            }
        }
    }
}