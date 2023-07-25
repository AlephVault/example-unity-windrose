namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   This is a local pot agent. It stands for players to be able
            ///   to show their local pot.
            /// </summary>
            public interface ILocalPotAgent
            {
                /// <summary>
                ///   Tells the current local pot.
                /// </summary>
                public int LocalPot();
            }
        }
    }
}