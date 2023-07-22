namespace AlephVault.Unity.CardGames
{
    namespace French52
    {
        namespace Poker
        {
            namespace Evaluators
            {
                /// <summary>
                ///   Evaluators have the ability to score hands.
                /// </summary>
                public interface IHandEvaluator
                {
                    /// <summary>
                    ///   Evaluates the hand power.
                    /// </summary>
                    /// <param nam="hand">The hand to evaluate</param>
                    /// <returns>The evaluation score</returns>
                    public int Evaluate(int[] hand);
                }
            }
        }
    }
}