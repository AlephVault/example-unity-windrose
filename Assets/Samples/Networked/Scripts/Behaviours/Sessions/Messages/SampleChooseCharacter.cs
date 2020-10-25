using NetRose.Behaviours.Sessions.Messages;
using Mirror;
using System.Collections.Generic;
using System;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            namespace Messages
            {
                public class SampleChooseCharacter : ChooseCharacter<int, string>
                {
                    protected override int ReadCharacterID(NetworkReader reader)
                    {
                        return reader.ReadInt32();
                    }

                    protected override string ReadCharacterPreviewData(NetworkReader reader)
                    {
                        return reader.ReadString();
                    }

                    protected override void WriteCharacterID(NetworkWriter writer, int id)
                    {
                        writer.WriteInt32(id);
                    }

                    protected override void WriteCharacterPreviewData(NetworkWriter writer, string data)
                    {
                        writer.WriteString(data);
                    }

                    public SampleChooseCharacter(IReadOnlyList<Tuple<int, string>> characters)
                    {
                        Characters = characters;
                    }
                }
            }
        }
    }
}