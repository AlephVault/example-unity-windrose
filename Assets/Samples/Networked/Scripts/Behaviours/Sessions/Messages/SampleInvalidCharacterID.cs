using NetRose.Behaviours.Sessions.Messages;
using Mirror;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            namespace Messages
            {
                public class SampleInvalidCharacterID : InvalidCharacterID<int>
                {
                    protected override int ReadCharacterID(NetworkReader reader)
                    {
                        return reader.ReadInt32();
                    }

                    protected override void WriteCharacterID(NetworkWriter writer, int id)
                    {
                        writer.WriteInt32(id);
                    }

                    public SampleInvalidCharacterID(int id)
                    {
                        ID = id;
                    }

                    public SampleInvalidCharacterID() {}
                }
            }
        }
    }
}