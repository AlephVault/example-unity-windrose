using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Protocols;
using GameMeanMachine.Unity.VirtualPad.Types.Models;


namespace GameMeanMachine.Unity.VirtualPad.Protocols
{
    namespace Protocols
    {
        public class VirtualPadProtocol : ProtocolDefinition
        {
            protected override void DefineMessages()
            {
                // The first lifecycle is: When the client
                // connects, it must greet the server. If
                // it does not do that in the first 10s,
                // then it will be disconnected with a
                // SyncTimeout message. Otherwise, it will
                // send a particular message depending on
                // it being an application or a candidate
                // to a control pad. That request can be
                // approved or rejected.
                DefineServerMessage("SynchronizationTimeout");
                DefineClientMessage<SynchronizationRequest>("Hello");
                DefineServerMessage("Synchronization:Approved");
                DefineServerMessage<RejectionResponse>("Synchronization:Rejected");
                
                // Then, already-registered applications
                // will receive a message when a control
                // is successfully synchronized:
                DefineServerMessage<PadSlot>("Synchronization:Started");
                
                // In the meantime, a control may tell to
                // de-synchronize (doing this, also tells
                // the server to close the connection).
                // Already-registered application will
                // receive a message telling that the pad
                // is being removed.
                //
                // While application also will use this
                // message to close its connection, this
                // closure will be silent for the pads and
                // other applications.
                DefineClientMessage("GoodBye");
                // This is the private answer the server
                // sends to the client (being it a pad, an
                // application, or a not-yet-synchronized
                // client connection).
                DefineServerMessage("SeeYouSoon");
                // This is the notification the server
                // sends to applications when an already
                // synchronized pad is desynchronized (by
                // sending GoodBye or by timing out in the
                // ping protocol).
                DefineServerMessage<PadSlot>("Synchronization:Ended");
                
                // Alternatively, the user may send a request
                // to the server. That request may involve only
                // two actions:
                // 1. Press a button, an analog, or orient an
                //    analog axis.
                // 2. Release a button, analog, or analog axis.
                //
                // Honestly, analog input at all will be quite
                // hard to implement in client-side, but still
                // this input is supported here.
                //
                // Servers might and often WILL clamp the values
                // of this input, and then apply sensitivity
                // metrics to the input (sensitivity, gravity,
                // dead, snap) for both the analog and digital
                // axes ("dead" will only correspond to analog).
                DefineClientMessage<VirtualJoystickInputState>("Input");

                // Additionally, the client may change the label
                // that would correspond to the virtual pad. It
                // must not be empty, but can be unique.
                DefineClientMessage<String>("Rename");
                DefineServerMessage("Label:Approved");
                DefineServerMessage("Label:Rejected");
                // This is the notification the server
                // sends to applications when an already
                // synchronized pad has its label updated (by
                // sending GoodBye or by timing out in the ping
                // protocol).
                DefineServerMessage<PadSlot>("Label:Update");
            }
        }
    }
}