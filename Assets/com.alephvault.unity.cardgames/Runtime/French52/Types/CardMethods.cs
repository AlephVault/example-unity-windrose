namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Types
        {
            /**
             * Extracts data from a card number. Card values are 0 .. 51 and the
             * source of these values is strictly verified.
             */
            public static class CardMethods
            {
                /// <summary>
                ///   The card suit is 0 .. 3 representing the suits:
                ///   Club, Hearts, Diamonds, Spades.
                /// </summary>
                /// <param name="card">The card number</param>
                /// <returns>Its suit</returns>
                public static int Suit(this int card)
                {
                    return card / 13;
                }
                
                /// <summary>
                ///   The card value is 0 .. 12 representing the values:
                ///   2, 3, ..., J, Q, K, A. In that order.
                /// </summary>
                /// <param name="card">The card number</param>
                /// <returns>Its face value</returns>
                public static int Value(this int card)
                {
                    return card % 13;
                }
            }
        }
    }
}
