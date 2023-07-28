namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Raise" prompt. The "Bet" prompt can be send in two flavors:
                ///   - Bet(amount > 0, 0): Fixed Limit (amount) or All-In (any limit) if the user doesn't have the min.
                ///   - Bet(min > 0, max > min): Pot limit (min, potSize) or No Limit (min, userChips).
                ///   A "Raise" option is specially crafted for the agent, according to the min/fixed bet.
                ///   Different to "Bet", a Raise is also computed on user's chips - amountToCall. If
                ///   the amount is &lt;= 0, then the option is not available (and a "Call" becomes "All-In").
                ///   But, otherwise, the concept is similar.
                /// </summary>
                public class Raise : AgentTurnPromptOption
                {
                    public Raise(int arg1, int arg2) : base("BET", arg1, arg2) {}

                    public override bool Accepts(AgentTurnAnswer answer)
                    {
                        if (!base.Accepts(answer)) return false;

                        if (Arg2 == 0)
                        {
                            return answer.Arg1 == Arg1;
                        }
                        if (Arg2 > 0)
                        {
                            return answer.Arg1 >= Arg1 && answer.Arg1 <= Arg2;
                        }

                        return false;
                    }
                }
            }
        }
    }
}