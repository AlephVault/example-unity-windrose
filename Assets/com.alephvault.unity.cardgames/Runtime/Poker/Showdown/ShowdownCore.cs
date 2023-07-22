using System;
using System.Collections.Generic;
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
                ///   The hand matcher to use in this showdown.
                /// </summary>
                public readonly IHandMatcher HandMatcher;

                /// <summary>
                ///   Whether the comparison is for best (high) or worst (low) score.
                /// </summary>
                public readonly bool Lowball;

                // Computes the ranks for the active hands.
                private Dictionary<IAgent, IMatchedHand> ComputeRanks(ShowdownPot[] showdownPots)
                {
                    Dictionary<IAgent, IMatchedHand> ranks = new Dictionary<IAgent, IMatchedHand>();
                    foreach (ShowdownPot showdownPot in showdownPots)
                    {
                        foreach (IAgent agent in showdownPot.Agents)
                        {
                            if (agent.Active() && !ranks.ContainsKey(agent))
                            {
                                ranks[agent] = HandMatcher.MatchHand(agent.Cards());
                            }
                        }
                    }

                    return ranks;
                }
                
                /// <summary>
                ///   Computes the showdown for the agents.
                /// </summary>
                /// <param name="showdownPots">The showdown pots. The 0-indexed one is the main one</param>
                /// <returns>The pots distribution</returns>
                public Tuple<IAgent, int, int>[] ComputeShowdown(ShowdownPot[] showdownPots)
                {
                    // Prepare the ranks, first.
                    Dictionary<IAgent, IMatchedHand> ranks = ComputeRanks(showdownPots);
                    
                    // Make it a sorted set.
                    SortedSet<IAgent> sortedAgents = new SortedSet<IAgent>(ranks.Keys, Comparer<IAgent>.Create(
                        Lowball ?
                            (ag1, ag2) => ranks[ag1].Rank() < ranks[ag2].Rank() ? 1 : -1 :
                            (ag1, ag2) => ranks[ag1].Rank() < ranks[ag2].Rank() ? -1 : 1
                    ));
                    
                    // TODO IMPLEMENT LATER.
                    return null;
                }
            }   
        }
    }
}