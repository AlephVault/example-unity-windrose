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
            public class ShowdownPot
            {
                /// <summary>
                ///   The amount of each pot.
                /// </summary>
                public readonly int EachPot;

                /// <summary>
                ///   The amount of the total pot.
                /// </summary>
                public readonly int TotalPot;

                /// <summary>
                ///   The agents.
                /// </summary>
                public readonly IReadOnlyCollection<IShowdownAgent> Agents;

                /// <summary>
                ///   Builds the sidepot with the amount and the players.
                /// </summary>
                /// <param name="eachPot">The size of each pot</param>
                /// <param name="agents">The involved agents</param>
                public ShowdownPot(int eachPot, IReadOnlyCollection<IShowdownAgent> agents)
                {
                    EachPot = eachPot;
                    TotalPot = eachPot * agents.Count;
                    Agents = agents;
                }
            }
        }
    }
}