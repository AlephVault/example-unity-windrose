using System;
using System.Collections.Generic;
using System.Linq;
using AlephVault.Unity.CardGames.Poker.Types;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace BettingManagement
        {
            /// <summary>
            ///   This is a collector of the pots. Collectors can be used many
            ///   times (they don't use a state to track).
            /// </summary>
            public class CentralPotsCollector
            {
                // Tells whether a player in the list is playing the current hand.
                private bool AtLeastOneIsPlaying(IEnumerable<IPlayerAgent> agents)
                {
                    return agents.Any(agent => agent.IsPlayingThisHand());
                }

                // Adds the payments properly from each user in the current level
                // (the current level is the one terminating).
                private void AddTheAffordedAmounts(
                    CentralPots centralPots, int currentAmountLevel, int previousAmountLevel,
                    int initialCentralPot, List<CentralPotPayment> payments
                )
                {
                    // First, add the current amount. ALL the players (even
                    // those who folded) afforded this amount so far. It is
                    // actually the DIFFERENCE between the pot and the previous
                    // amount, since previous rounds already subtracted the
                    // previous amount from all the players.
                    List<Tuple<IPlayerAgent, int>> addedAmounts = centralPots.AddAmountFromPlayers(
                        currentAmountLevel - previousAmountLevel
                    );
                                
                    // Now: for each PREVIOUS pot, these players could afford
                    // everything.
                    //
                    // However, for the last/current pot, they could afford
                    // only to the addedAmounts.
                    foreach(Tuple<IPlayerAgent, int> item in addedAmounts)
                    {
                        for (int potIndex = initialCentralPot;
                             potIndex < centralPots.LastPotIndex;
                             potIndex++)
                        {
                            payments.Add(new CentralPotPayment(
                                item.Item1, potIndex, centralPots.Pots[potIndex].EachPot
                            ));
                        }
                                    
                        payments.Add(new CentralPotPayment(
                            item.Item1, centralPots.LastPotIndex, item.Item2
                        ));
                    }
                }

                /// <summary>
                ///   Collects the local pots of the player into the central pots.
                /// </summary>
                /// <param name="centralPots">The central pots to populate</param>
                /// <param name="players">The players</param>
                /// <returns>The payments, so the pots can be assembled in front-end</returns>
                public List<CentralPotPayment> CollectPots(CentralPots centralPots, IReadOnlyCollection<IPlayerAgent> players)
                {
                    // First, we keep a sorted (ascending) list of nonzero local pots.
                    Comparer<IPlayerAgent> cmp = Comparer<IPlayerAgent>.Create(
                        (ag1, ag2) => ag1.LocalPot() < ag2.LocalPot() ? 1 : -1
                    );
                    List<IPlayerAgent> sortedPlayers = new SortedSet<IPlayerAgent>(
                        players.Where((p) => p.LocalPot() > 0)
                    ).ToList();

                    // These variables help us to drop the players (and create new
                    // side pots) based on amount changes. The players in the last
                    // amount are NOT locked.
                    HashSet<IPlayerAgent> playersToLock = new HashSet<IPlayerAgent>();
                    // Also, the current level. This one is adjusted on each iteration.
                    int currentAmountLevel = 0;
                    // This is the PREVIOUS level.
                    int previousAmountLevel = 0;
                    
                    // We'll also tell which pots are the players affording into.
                    // For this, we'll know which one is the initial central pot
                    // to afford into in this stage.
                    int initialCentralPot = centralPots.LastPotIndex;
                    
                    // We'll also maintain a list of (agent, potIndex, amount) of
                    // the payments the players made.
                    List<CentralPotPayment> payments = new List<CentralPotPayment>();

                    foreach (IPlayerAgent player in sortedPlayers)
                    {
                        int localPot = player.LocalPot();
                        // PLEASE NOTE: IT MIGHT HAPPEN THAT THE localPot IS NEVER
                        // GREATER THAN 0. Situations of pure check/check/... round.
                        
                        if (localPot > currentAmountLevel)
                        {
                            // This part is tricky: For the same previous amount, it
                            // might happen that all the players in that amount have
                            // folded, or at least one did not fold. If at least one
                            // did NOT fold, then we establish that pot level and lock
                            // all the listed players on it. If, instead, all of them
                            // folded, then we keep those for the next level.
                            //
                            // Also, we require the current amount level to not be 0,
                            // or there's nothing to add.
                            if (currentAmountLevel > 0 && AtLeastOneIsPlaying(playersToLock))
                            {
                                AddTheAffordedAmounts(
                                    centralPots, currentAmountLevel, previousAmountLevel, initialCentralPot,
                                    payments
                                );

                                // Lock the previous players, since there's an amount
                                // they did NOT afford because they went all-in or
                                // folded. If they folded, they're added only to the
                                // last pot they afforded into, for this reason.
                                centralPots.LockPlayers(playersToLock);
                                playersToLock.Clear();
                            }
                            
                            // Finally, setting the current amount level to the
                            // local pot, and the previous amount level to the
                            // current one.
                            previousAmountLevel = currentAmountLevel;
                            currentAmountLevel = localPot;
                        }

                        // Add the current player to the players to lock.
                        playersToLock.Add(player);
                    }
                    
                    // By the end, if they're players to lock, they'll not be blocked,
                    // because it might be the case (and often will) that they can STILL
                    // afford future bets in the current pot. BUT the amounts will still
                    // be added for these players.
                    //
                    // Please note: By this point, at least one player did not fold. BUT
                    // it might happen that the current amount level is 0 due to being
                    // a check/check/... round.
                    if (currentAmountLevel > 0) AddTheAffordedAmounts(
                        centralPots, currentAmountLevel, previousAmountLevel, initialCentralPot,
                        payments
                    );

                    // Also return the payments.
                    return payments;
                }
            }
        }
    }
}