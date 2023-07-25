using System.Collections.Generic;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A showdown pot is either the main one (1) or a side pot.
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
                public IReadOnlyCollection<IShowdownAgent> Agents { get; }

                /// <summary>
                ///   Builds the side pot with the amount and the players.
                /// </summary>
                /// <param name="eachPot">The size of each pot</param>
                /// <param name="agents">The involved agents</param>
                public CentralPot(IReadOnlyCollection<IShowdownAgent> agents)
                {
                    EachPot = 0;
                    TotalPot = 0 * agents.Count;
                    Agents = agents;
                }

                /// <summary>
                ///   Adds a certain amount (from each player) to this pot.
                /// </summary>
                /// <param name="eachPot">The per-player amount to add</param>
                public void AddAmount(int eachPot)
                {
                    if (eachPot <= 0) return;
                    EachPot += eachPot;
                    TotalPot += eachPot * Agents.Count;
                }
            }
        }
    }
}