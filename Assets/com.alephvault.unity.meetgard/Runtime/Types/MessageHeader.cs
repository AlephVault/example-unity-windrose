using AlephVault.Unity.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.Meetgard
{
    namespace Types
    {
        /// <summary>
        ///   A message header with the 3 fields:
        ///   protocol id, message tag, message size.
        ///   It also has the ability to check the
        ///   size of the message against a maximum
        ///   provided size.
        /// </summary>
        public class MessageHeader : ISerializable
        {
            public ushort ProtocolId;
            public ushort MessageTag;
            public ushort MessageSize;

            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref ProtocolId);
                serializer.Serialize(ref MessageTag);
                serializer.Serialize(ref MessageSize);
            }

            public void CheckSize(long maxMessageSize)
            {
                if (MessageSize > maxMessageSize)
                {
                    throw new MessageOverflowException($"The message's size ({MessageSize}) is greater than the maximum allowed size ({maxMessageSize})");
                }
            }
        }
    }
}
