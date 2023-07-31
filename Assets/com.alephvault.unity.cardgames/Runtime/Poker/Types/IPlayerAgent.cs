using AlephVault.Unity.CardGames.Types;

namespace AlephVault.Unity.CardGames
{
    namespace Poker
    {
        namespace Types
        {
            /// <summary>
            ///   A player agent.
            /// </summary>
            public interface IPlayerAgent : ITurnAttendingAgent, IDealerHoldingAgent, IStatusHoldingAgent,
                ILocalPotAgent, IShowdownAgent 
            {
            }
        }
    }
}