using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.CardGames.Poker.Types;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace BettingManagement
        {
            /// <summary>
            ///   This is a base betting round. Betting rounds are
            ///   created for each betting stage and it might occur
            ///   that they're different. For example: the 1st and
            ///   2nd round of Fixed Limit Texas Hold'Em are of 1x
            ///   and the 3rd (turn) and 4th (river) rounds are of
            ///   2x amount (where 1x is the Big Blind size). They
            ///   will be implemented as different FL instances,
            ///   having respective amounts. 
            /// </summary>
            public abstract class BaseBettingRound
            {
                /// <summary>
                ///   Gets the properly shifted list. This betting
                ///   round must know which one is the player that
                ///   must start the round. By this point, it will
                ///   happen that: 1. At least 2 players are able
                ///   to bet. 2. There might be null agents in the
                ///   table (i.e. empty places).
                /// </summary>
                /// <param name="players">The list of players</param>
                /// <returns>The shifted players list</returns>
                protected abstract IReadOnlyList<IPlayerAgent> GetShiftedList(IEnumerable<IPlayerAgent> players);
                
                /// <summary>
                ///   Collects the bets. Each betting round is different
                ///   in structure and rules to collect the bets, although
                ///   all of them follow a structure similar to {blinds,
                ///   call, bet, raise, check, fold}, and it ends with
                ///   nobody is able to raise / call anymore. The local
                ///   pots will be full of the bets, and it will be known
                ///   whether players folded or not.
                /// </summary>
                /// <param name="players">The players in the list, already properly sorted</param>
                protected abstract Task AttendPlayersBets(IEnumerable<IPlayerAgent> players);

                /// <summary>
                ///   Executes an entire round. Rounds will NOT be
                ///   stoppable: they might be audited by the end,
                ///   but so far they will know how to execute.
                /// </summary>
                /// <param name="players">The players</param>
                /// <param name="centralPots">The central pots to collect the bets into</param>
                /// <remarks>
                ///   This is NOT a full hand execution, and any operation
                ///   done here will NOT be persisted in the agent or table
                ///   data until the very end of the entire hand.
                /// </remarks>
                /// <returns>All the payments to integrate the side pots</returns>
                public async Task<List<CentralPotPayment>> Round(IEnumerable<IPlayerAgent> players, CentralPots centralPots)
                {
                    // 1. Prepare the round players.
                    IReadOnlyList<IPlayerAgent> playersList = GetShiftedList(players);
                    
                    // 2. Play the entire round's hands.
                    await AttendPlayersBets(playersList);
                    
                    // 3. Collect the bets into the central pots.
                    List<CentralPotPayment> payments = LocalPotsCollector.CollectPots(centralPots, playersList);
                    
                    // 4. Clear the players' pots.
                    LocalPotsCollector.ClearLocalPots(playersList);
                    
                    // 5. Return the payments.
                    return payments;
                }
            }
        }
    }
}