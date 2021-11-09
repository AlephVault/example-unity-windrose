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
                        public MapObject MapObject;

                        public abstract bool Execute(bool accelerate);
                    }

                    // A Movement Start queued command.
                    private class MovementStartCommand : QueuedCommand
                    {
                        public ushort StartX;
                        public ushort StartY;
                        public Direction Direction;

                        public override bool Execute(bool accelerate)
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

                        public override bool Execute(bool accelerate)
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

                        public override bool Execute(bool accelerate)
                        {
                            // TODO implement.
                            throw new System.NotImplementedException();
                        }
                    }

                    // A Speed Change queued command.
                    private class SpeedChangeCommand : QueuedCommand
                    {
                        public uint Speed;

                        public override bool Execute(bool accelerate)
                        {
                            // Just set the speed.
                            MapObject.Speed = Speed;
                            // Return true: to tell that the queue must
                            // execute immediately.
                            return true;
                        }
                    }

                    // An Orientation Change queued command.
                    private class OrientationChangeCommand : QueuedCommand
                    {
                        public Direction Orientation;

                        public override bool Execute(bool accelerate)
                        {
                            // Just set the orientation.
                            MapObject.Orientation = Orientation;
                            // Return true: to tell that the queue must
                            // execute immediately.
                            return true;
                        }
                    }

                    // Clears the queue.
                    private void ClearQueue()
                    {
                        // TODO implement.
                    }

                    // Runs the next element in the queue.
                    private void RunQueue()
                    {
                        // TODO implement.
                    }

                    // Queues an element into the queue.
                    private void QueueElement(QueuedCommand command)
                    {
                        // TODO implement.
                    }
                }
            }
        }
    }
}
