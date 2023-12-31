using System;
using System.Collections.Generic;
using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A central pot is either the main one (1) or a side pot.
            ///   Knows its total amount and the involved players (agents).
            /// </summary>
            public class CentralPot
            {
                /// <summary>
                ///   The amount of each pot.
                /// </summary>
                public int EachPot { get; private set; }

                /// <summary>
                ///   The amount of the total pot.
                /// </summary>
                public int TotalPot { get; private set; }

                /// <summary>
                ///   The agents.
                /// </summary>
                public IReadOnlyCollection<IPlayerAgent> Agents { get; }

                /// <summary>
                ///   Builds the side pot with the amount and the players.
                /// </summary>
                /// <param name="eachPot">The size of each pot</param>
                /// <param name="agents">The involved agents</param>
                public CentralPot(IReadOnlyCollection<IPlayerAgent> agents)
                {
                    EachPot = 0;
                    TotalPot = 0 * agents.Count;
                    Agents = agents;
                }

                /// <summary>
                ///   Adds a certain amount (from each player) to this pot.
                ///   If the player did not afford that amount in the local
                ///   pot, then the entire local pot is added.
                /// </summary>
                /// <param name="amount">The per-player amount to add</param>
                /// <returns>The effective added amounts</returns>
                public List<Tuple<IPlayerAgent, int>> AddAmountFromPlayers(int amount)
                {
                    List<Tuple<IPlayerAgent, int>> result = new List<Tuple<IPlayerAgent, int>>();
                    if (amount > 0)
                    {
                        EachPot += amount;
                        foreach (var agent in Agents)
                        {
                            int finalAmount = Values.Min(amount, agent.LocalPot());
                            result.Add(new Tuple<IPlayerAgent, int>(agent, finalAmount));
                            TotalPot += finalAmount;
                        }
                    }
                    return result;
                }

                /// <summary>
                ///   Adds a "dead bet" amount. This one is not added by all
                ///   the players, but just by one of them.
                /// </summary>
                /// <param name="amount">The amount to add as dead</param>
                public void AddDeadAmount(int amount)
                {
                    EachPot += amount;
                    TotalPot += amount;
                }
            }
        }
    }
}