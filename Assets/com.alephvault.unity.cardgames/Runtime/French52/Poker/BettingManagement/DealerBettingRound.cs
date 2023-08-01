using System.Collections.Generic;
using AlephVault.Unity.CardGames.Poker.BettingManagement;
using AlephVault.Unity.CardGames.Poker.Types;
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
                ///   These are betting rounds for Dealer games. Actually,
                ///   most of the Poker games use this mechanism of Dealer
                ///   token(s), independently of the betting system. The
                ///   dealer 
                /// </summary>
                public abstract class DealerBettingRound : BaseBettingRound
                {
                    /// <summary>
                    ///   Number of shift to apply. Typically, 2 for the initial
                    ///   betting round (one for SB, one for BB).
                    /// </summary>
                    public readonly int ExtraPlayerShifts;
                    
                    public DealerBettingRound(int extraPlayerShift = 2)
                    {
                        ExtraPlayerShifts = Values.Max(0, extraPlayerShift);
                    }
                    
                    /// <summary>
                    ///   Determines the list with the following algorithm:
                    ///   1. First, collects all the players as they arrive into the "last" list.
                    ///      This is until the "dealer" token is found.
                    ///   2. Then, collects all the players as they arrive 
                    /// </summary>
                    /// <param name="players">The players (might be effective players or not)</param>
                    /// <returns>The shifted players</returns>
                    protected override IReadOnlyList<IPlayerAgent> GetShiftedList(IEnumerable<IPlayerAgent> players)
                    {
                        bool dealerReached = false;
                        List<IPlayerAgent> lastPlayers = new List<IPlayerAgent>();
                        List<IPlayerAgent> firstPlayers = new List<IPlayerAgent>();
                        foreach (IPlayerAgent player in players)
                        {
                            if (dealerReached)
                            {
                                firstPlayers.Add(player);
                            }
                            else
                            {
                                lastPlayers.Add(player);
                            }
                            
                            if (player.IsDealer())
                            {
                                dealerReached = true;
                            }
                        }
                        
                        int totalCount = lastPlayers.Count + firstPlayers.Count;

                        if (totalCount < 2)
                        {
                            // The case of only 1 player is trivial.
                            firstPlayers.AddRange(lastPlayers);
                            return firstPlayers;
                        }

                        if (totalCount == 2)
                        {
                            // The case of 2 players is inverse: [Dealer, NotDealer].
                            lastPlayers.AddRange(firstPlayers);
                            return lastPlayers;
                        }
                        
                        // Other cases have their quirks like this:
                        // - Blind-having rounds will hold S players after the dealer.
                        //   Those S ("shifted") players have blinds, and S is typically
                        //   2 (small blind, and big blind).
                        // - Standard rounds will have no shift.
                        firstPlayers.AddRange(lastPlayers);

                        // Applying the shifts, if any.
                        for (int i = 0; i < ExtraPlayerShifts; i++)
                        {
                            firstPlayers.Add(firstPlayers[0]);
                            firstPlayers.RemoveAt(0);
                        }
                        
                        // Return the list.
                        return firstPlayers;
                    }
                }
            }
        }
    }
}