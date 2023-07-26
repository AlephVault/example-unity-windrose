namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            public interface IStatusHoldingAgent
            {
                /// <summary>
                ///   Tells whether the agent is playing or not this hand.
                ///   ACTIVE and ALL_IN players are playing this hand.
                /// </summary>
                /// <returns></returns>
                public bool IsPlayingThisHand();
            }
        }
    }
}