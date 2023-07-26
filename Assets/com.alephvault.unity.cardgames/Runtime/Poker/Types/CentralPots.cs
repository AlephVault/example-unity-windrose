using System;
using System.Collections.Generic;
using System.Linq;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   Manages all the current & potential central pots in the round.
            /// </summary>
            public class CentralPots
            {
                // Players that are not all-in. A player can go all-in by having
                // zero (0) remaining chips in funds, or receiving a special
                // pseudo-all-in condition as disconnection protection.
                private HashSet<IPlayerAgent> playersThatCanAfford = new HashSet<IPlayerAgent>();

                /// <summary>
                ///   The still-affording players (i.e. players that were not
                ///   deemed as non-affording this hand by delta-collecting
                ///   of it being all-in, folded, or something similar).
                /// </summary>
                public IReadOnlyCollection<IPlayerAgent> PlayersThatCanAfford => playersThatCanAfford;

                // The current pots.
                private List<CentralPot> pots = new List<CentralPot>();

                /// <summary>
                ///   The active pots.
                /// </summary>
                public IReadOnlyList<CentralPot> Pots => pots;

                /// <summary>
                ///   The last pot index.
                /// </summary>
                /// <remarks>Always >= 0 since at least one element will always exist</remarks>
                public int LastPotIndex => pots.Count - 1;

                /// <summary>
                ///   Creates, from scratch, the whole pots of the game.
                /// </summary>
                /// <param name="players">The initial players</param>
                public CentralPots(IEnumerable<IPlayerAgent> players)
                {
                    // First, init the non-locked players with all the seats.
                    // This is a copied list that is set up when the hand starts.
                    // Players that enter later are added to the original list,
                    // and not this one.
                    //
                    // Null positions are NOT considered here.
                    foreach (IPlayerAgent player in players)
                    {
                        playersThatCanAfford.Add(player);
                    }
                    // Then, initialize the first pot.
                    pots.Add(new CentralPot(new List<IPlayerAgent>(playersThatCanAfford)));
                }

                /// <summary>
                ///   Adds a certain amount (from each player) to the last pot.
                /// </summary>
                /// <param name="amount">The per-player amount to add</param>
                public List<Tuple<IPlayerAgent, int>> AddAmountFromPlayers(int amount)
                {
                    return pots.Last().AddAmountFromPlayers(amount);
                }

                /// <summary>
                ///   Adds a "dead bet" amount. This one is not added by all
                ///   the players, but just by one of them. The amount is
                ///   added to the last pot.
                /// </summary>
                /// <param name="amount">The amount to add as dead</param>
                public void AddDeadAmount(int amount)
                {
                    pots.Last().AddDeadAmount(amount);
                }

                /// <summary>
                ///   Marks that some players went all-in this betting round.
                /// </summary>
                /// <param name="players">The players that went all-in</param>
                /// <returns>Whether a new pot was created or not (it typically will)</returns>
                public bool LockPlayers(IEnumerable<IPlayerAgent> players)
                {
                    bool removed = false;
                    foreach (IPlayerAgent player in players)
                    {
                        removed |= playersThatCanAfford.Remove(player);
                    }

                    if (removed)
                    {
                        pots.Add(new CentralPot(new List<IPlayerAgent>(playersThatCanAfford)));
                    }

                    return removed;
                }
            }
        }
    }
}