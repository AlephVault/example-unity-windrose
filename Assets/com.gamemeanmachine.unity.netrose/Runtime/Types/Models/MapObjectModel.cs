using AlephVault.Unity.Binary;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Models
        {
            /// <summary>
            ///   A map object model has two properties: the underlying
            ///   data and the attachment (which may be null when the
            ///   object is not attached to a map, and includes the
            ///   current movement).
            /// </summary>
            public class MapObjectModel<ModelData> : ISerializable
                where ModelData : class, ISerializable, new()
            {
                /// <summary>
                ///   The current map status. It is null if the object
                ///   is not attached to a map.
                /// </summary>
                Status Status;

                /// <summary>
                ///   The current object data.
                /// </summary>
                ModelData Data;

                public void Serialize(Serializer serializer)
                {
                    Status.Serialize(serializer);
                    if (serializer.IsReading)
                    {
                        if (serializer.Reader.ReadBool())
                        {
                            Data = new ModelData();
                            Data.Serialize(serializer);
                        }
                        else
                        {
                            Data = null;
                        }
                    }
                    else
                    {
                        serializer.Writer.WriteBool(Data != null);
                        Data?.Serialize(serializer);
                    }
                }
            }
        }
    }
}
