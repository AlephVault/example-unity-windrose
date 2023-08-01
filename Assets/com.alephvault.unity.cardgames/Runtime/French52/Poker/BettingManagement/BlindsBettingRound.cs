using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Poker
        {
            namespace BettingManagement
            {
                /// <summary>
                ///   A betting rounds with blinds uses a shift of 2 players
                ///   to determine the UTG, and charges the user with the
                ///   proper costs of the blinds.
                /// </summary>
                public abstract class BlindsBettingRound : DealerBettingRound
                {
                    /// <summary>
                    ///   The big blind to use.
                    /// </summary>
                    public readonly int BigBlind;
                    
                    /// <summary>
                    ///   The small blind to use.
                    /// </summary>
                    public readonly int SmallBlind;
                    
                    public BlindsBettingRound(int bigBlind, int smallBlind = 0) : base(2)
                    {
                        bigBlind = Values.Max(2, bigBlind);
                        if (smallBlind < 0)
                        {
                            smallBlind = bigBlind / 2;
                        }

                        BigBlind = bigBlind;
                        SmallBlind = smallBlind;
                    }
                }
            }
        }
    }
}