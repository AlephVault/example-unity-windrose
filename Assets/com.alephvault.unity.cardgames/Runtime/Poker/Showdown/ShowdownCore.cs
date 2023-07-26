using System;
using System.Collections.Generic;
using System.Linq;
using AlephVault.Unity.CardGames.Poker.Matchers;
using AlephVault.Unity.CardGames.Poker.Types;
using AlephVault.Unity.CardGames.Types.MatchedHands;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Showdown
        {
            /// <summary>
            ///   Poker showdown core algorithm. By the point this algorithm executes:
            ///   - All the side pots are built.
            ///   - All the game splits, if any, are done (e.g. hi/lo games). In this
            ///     case, an execution covers a single sub-game.
            ///   - All the hands are fully dealt.
            ///   This algorithm works the following way:
            ///   - For each pot => for each ACTIVE agent => compute the matched hand.
            /// </summary>
            public class ShowdownCore
            {
                /// <summary>
                ///   The hand matcher to use in this showdown. It will also be the
                ///   component that knows the current table settings (e.g. common
                ///   cards and extra things).
                /// </summary>
                public readonly IHandMatcher HandMatcher;

                /// <summary>
                ///   Whether the comparison is for best (high) or worst (low) score.
                /// </summary>
                public readonly bool Lowball;

                // Computes the ranks for the active hands.
                private Dictionary<IShowdownAgent, IMatchedHand> ComputeRanks(
                    IReadOnlyCollection<CentralPot> showdownPots
                )
                {
                    Dictionary<IShowdownAgent, IMatchedHand> ranks = new Dictionary<IShowdownAgent, IMatchedHand>();
                    foreach (CentralPot showdownPot in showdownPots)
                    {
                        foreach (IPlayerAgent agent in showdownPot.Agents)
                        {
                            if (agent.IsPlayingThisHand() && !ranks.ContainsKey(agent))
                            {
                                ranks[agent] = HandMatcher.MatchHand(agent.Cards());
                            }
                        }
                    }

                    return ranks;
                }

                // Distributes a single pot among perhaps many players.
                private List<ShowdownDistribution> Distribute(
                    CentralPot pot, int potIndex, Dictionary<IShowdownAgent, IMatchedHand> ranks,
                    SortedSet<IShowdownAgent> agents
                )
                {
                    // For each pot, at least ONE active player is there. Always.
                    // So in the worst case, one player will match the pot. Also,
                    // all the agents described here are active. Either all-in or
                    // not (they may be even sit-out which, for tournaments, only
                    // means auto-check-fold, thus having a chance to be active).
                    List<ShowdownDistribution> distributions = new List<ShowdownDistribution>();

                    // The first thing is to match the winners. The first pot-agent
                    // in the list of agents (which is sorted by best -> worst rank)
                    // is the winner since it has the best possible current rank and
                    // also that rank determines the one to tie (split pot). Then,
                    // the winners are collected using this logic.
                    int rank = 0;
                    List<IShowdownAgent> winners = new List<IShowdownAgent>();
                    foreach (IShowdownAgent agent in agents)
                    {
                        if (pot.Agents.Contains(agent))
                        {
                            int currentRank = ranks[agent].Rank();
                            if (rank == 0 || rank == currentRank)
                            {
                                // We consider this rank (since it is already the best one).
                                // We also, obviously, include the player.
                                rank = currentRank;
                                winners.Add(agent);
                            }
                            else
                            {
                                // Break, since on a different rank then no other players
                                // will have the same (best) rank as the first winner.
                                break;
                            }
                        }
                    }

                    // The pot has a certain amount, which might not be an exact multiple
                    // of the number of winners. The criteria is to assign the remainders
                    // to the first winners in the list (closer to however the UTG was
                    // determined by the end point). Otherwise, the amount to assign is
                    // the down-rounded quotient.
                    int remainder = pot.TotalPot % winners.Count;
                    int quantity = pot.TotalPot / winners.Count;
                    int index = 0;
                    foreach (IShowdownAgent winner in winners)
                    {
                        distributions.Add(new ShowdownDistribution(
                            winner, potIndex, quantity + (index < remainder ? 1 : 0))
                        );
                        index++;
                    }
                    // Return the distributions.
                    return distributions;
                }
                
                /// <summary>
                ///   Computes the showdown for the agents.
                /// </summary>
                /// <param name="showdownPots">The showdown pots. The 0-indexed one is the main one</param>
                /// <returns>The pots distributions and the matched hands</returns>
                public Tuple<List<ShowdownDistribution>, Dictionary<IShowdownAgent, IMatchedHand>> ComputeShowdown(
                    IReadOnlyCollection<CentralPot> showdownPots
                )
                {
                    // Prepare the ranks, first.
                    Dictionary<IShowdownAgent, IMatchedHand> ranks = ComputeRanks(showdownPots);
                    
                    // Make it a sorted set.
                    Comparer<IShowdownAgent> cmp = Comparer<IShowdownAgent>.Create(Lowball
                        ? (ag1, ag2) => ranks[ag1].Rank() < ranks[ag2].Rank() ? 1 : -1
                        : (ag1, ag2) => ranks[ag1].Rank() < ranks[ag2].Rank() ? -1 : 1);
                    SortedSet<IShowdownAgent> sortedAgents = new SortedSet<IShowdownAgent>(ranks.Keys, cmp);

                    // For each pot, distribute it using the sorted agents.
                    // Accumulate them in a single history.
                    List<ShowdownDistribution> distributions = new List<ShowdownDistribution>();
                    int index = 0;
                    foreach (CentralPot showdownPot in showdownPots)
                    {
                        distributions.AddRange(Distribute(showdownPot, index, ranks, sortedAgents));
                        index += 1;
                    }
                    
                    // Return the pots distributions and the active players' hands.
                    return new Tuple<List<ShowdownDistribution>, Dictionary<IShowdownAgent, IMatchedHand>>(
                        distributions, ranks
                    );
                }
            }   
        }
    }
}