namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   This is a showdown agent. It stands for players to be able
            ///   to play/resolve a showdown.
            /// </summary>
            public interface IShowdownAgent
            {
                /// <summary>
                ///   Whether the agent is playing this turn (otherwise,
                ///   it folded or is sit-out).
                /// </summary>
                public bool PlaysShowdown();

                /// <summary>
                ///   The cards.
                /// </summary>
                public int[] Cards();
            }
        }
    }
}