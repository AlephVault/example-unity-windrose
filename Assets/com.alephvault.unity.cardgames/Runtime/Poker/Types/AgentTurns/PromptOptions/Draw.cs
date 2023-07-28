using UnityEditor.Experimental.GraphView;

namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        namespace AgentTurns
        {
            namespace PromptOptions
            {
                /// <summary>
                ///   A "Draw" action. It expects proper player's cards as indices.
                ///   Typically, it is for 5-card games.
                /// </summary>
                public class Draw : AgentTurnPromptOption
                {
                    public Draw(int cards) : base("DRAW", cards, 0) {}

                    public override bool Accepts(AgentTurnAnswer answer)
                    {
                        return answer.Arg1 < 1 << Arg1;
                    }
                }
            }
        }
    }
}