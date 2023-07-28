namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Bet" prompt. The "Bet" prompt can be send in two flavors:
                ///   - Bet(amount > 0, 0): Fixed Limit (amount) or All-In (any limit) if the user doesn't have the min.
                ///   - Bet(min > 0, max > min): Pot limit (min, potSize).
                ///   - Bet(min > 0, _ &lt; 0): No limit (min).
                ///   A "Bet" option is specially crafted for the agent, according to the min/fixed bet.
                ///   Not available if bets were already done this round.
                /// </summary>
                public class Bet : AgentTurnPromptOption
                {
                    public Bet(int arg1, int arg2) : base("BET", arg1, arg2) {}

                    public override bool Accepts(AgentTurnAnswer answer)
                    {
                        if (!base.Accepts(answer)) return false;

                        if (Arg2 == 0)
                        {
                            return answer.Arg1 == Arg1;
                        }
                        else if (Arg2 < 0)
                        {
                            return answer.Arg1 >= Arg1;
                        }
                        else
                        {
                            return answer.Arg1 >= Arg1 && answer.Arg1 <= Arg2;
                        }
                    }
                }
            }
        }
    }
}