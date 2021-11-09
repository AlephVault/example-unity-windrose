using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using GameMeanMachine.Unity.NetRose.Types.Models;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Types;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                public partial class NetRoseMapObjectClientSide : MonoBehaviour
                {
                    // One of the queued commands: Movement Start, Finish, Cancel,
                    // Speed Change, and Orientation Change.
                    private abstract class QueuedCommand
                    {
                        public abstract void Execute(bool accelerate);
                    }

                    // Clears the queue.
                    private void ClearQueue()
                    {
                        // TODO implement.
                    }
                }
            }
        }
    }
}
