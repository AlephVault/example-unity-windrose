﻿using NetRose.Behaviours.Sessions.Messages;
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
                public class SampleCharacterDoesNotExist : CharacterDoesNotExist<int>
                {
                    protected override int ReadCharacterID(NetworkReader reader)
                    {
                        return reader.ReadInt32();
                    }

                    protected override void WriteCharacterID(NetworkWriter writer, int id)
                    {
                        writer.WriteInt32(id);
                    }

                    public SampleCharacterDoesNotExist(int id)
                    {
                        ID = id;
                    }

                    public SampleCharacterDoesNotExist() {}
                }
            }
        }
    }
}