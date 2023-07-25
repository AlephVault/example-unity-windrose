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
                private HashSet<IPlayerAgent> nonLockedPlayers = new HashSet<IPlayerAgent>();
                
                // The current pots.
                private List<CentralPot> pots = new List<CentralPot>();

                /// <summary>
                ///   The active pots.
                /// </summary>
                public IReadOnlyList<CentralPot> Pots => pots;

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
                        nonLockedPlayers.Add(player);
                    }
                    // Then, initialize the first pot.
                    pots.Add(new CentralPot(new List<IPlayerAgent>(nonLockedPlayers)));
                }

                /// <summary>
                ///   Adds a certain amount (from each player) to the last pot.
                /// </summary>
                /// <param name="amount">The per-player amount to add</param>
                public void AddAmountFromPlayers(int amount)
                {
                    pots.Last().AddAmountFromPlayers(amount);
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
                public bool LockPlayer(IEnumerable<IPlayerAgent> players)
                {
                    bool removed = false;
                    foreach (IPlayerAgent player in players)
                    {
                        removed |= nonLockedPlayers.Remove(player);
                    }

                    if (removed)
                    {
                        pots.Add(new CentralPot(new List<IPlayerAgent>(nonLockedPlayers)));
                    }

                    return removed;
                }
            }
        }
    }
}