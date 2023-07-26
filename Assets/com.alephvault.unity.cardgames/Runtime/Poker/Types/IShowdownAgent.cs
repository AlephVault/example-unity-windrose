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
                ///   The cards.
                /// </summary>
                public int[] Cards();
            }
        }
    }
}