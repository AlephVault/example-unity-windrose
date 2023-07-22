using AlephVault.Unity.CardGames.Types.MatchedHands;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Matchers
        {
            /// <summary>
            ///   A matcher is a black box which takes whatever is
            ///   the table status and then the agent's hands and
            ///   builds the BEST possible hand
            /// </summary>
            public interface IHandMatcher
            {
                /// <summary>
                ///   Builds a matched hand from the agent's hand.
                ///   This is done by default (for 5-card games)
                ///   or will involve meta-comparing (e.g. 7-card
                ///   stud, Hold'em) before producing the hand.
                /// </summary>
                /// <param name="hand">The hand to match again</param>
                /// <returns>The matched hand</returns>
                public IMatchedHand MatchHand(int[] hand);
            }
        }
    }
}