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

                    // A Movement Start queued command.
                    private class MovementStartCommand : QueuedCommand
                    {
                        public ushort StartX;
                        public ushort StartY;
                        public Direction Direction;

                        public override void Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
                    }

                    // A Movement Cancel queued command.
                    private class MovementCancelCommand : QueuedCommand
                    {
                        public ushort RevertX;
                        public ushort RevertY;

                        public override void Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
                    }

                    // A Movement Finish queued command.
                    private class MovementFinishCommand : QueuedCommand
                    {
                        public ushort EndX;
                        public ushort EndY;

                        public override void Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
                    }

                    // A Speed Change queued command.
                    private class SpeedChangeCommand : QueuedCommand
                    {
                        public uint Speed;

                        public override void Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
                    }

                    // An Orientation Change queued command.
                    private class OrientationChangeCommand : QueuedCommand
                    {
                        public Direction Orientation;

                        public override void Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
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
