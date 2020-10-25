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
                public class SampleUsingCharacter : UsingCharacter<int, SampleDatabase.Character>
                {
                    protected override int ReadCharacterID(NetworkReader reader)
                    {
                        return reader.ReadInt32();
                    }

                    protected override SampleDatabase.Character ReadCharacterFullData(NetworkReader reader)
                    {
                        SampleDatabase.Character character = new SampleDatabase.Character();
                        character.CharName = reader.ReadString();
                        character.Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(reader.ReadString()));
                        return character;
                    }

                    protected override void WriteCharacterID(NetworkWriter writer, int id)
                    {
                        writer.WriteInt32(id);
                    }

                    protected override void WriteCharacterFullData(NetworkWriter writer, SampleDatabase.Character data)
                    {
                        writer.WriteString(data.CharName);
                        writer.WriteString(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data.Prefab)));
                    }

                    public SampleUsingCharacter(int id, SampleDatabase.Character data)
                    {
                        CurrentCharacterID = id;
                        CurrentCharacterData = data;
                    }
                }
            }
        }
    }
}