namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A Poker agent is a player, which has a private hands
            ///   (either disclosed or not) and a pot.
            /// </summary>
            public interface IAgent
            {
                // Agent attributes for the showdown.

                /// <summary>
                ///   Whether the agent is playing this turn (otherwise,
                ///   it folded or is sit-out).
                /// </summary>
                public bool Active();
            
                /// <summary>
                ///   Whether the agent went all-in this turn.
                /// </summary>
                public bool AllIn();
            
                /// <summary>
                ///   The total gambled pot for this agent this turn.
                /// </summary>
                public int Pot();

                /// <summary>
                ///   The cards.
                /// </summary>
                public int[] Cards();
            }
        }
    }
}