using AlephVault.Unity.Binary;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   This client side implementation of an object maintains
                ///   certain data elements that will be kept synchronizable
                ///   on spawn and refresh. 
                /// </summary>
                public abstract class ModelClientSide<SpawnType, RefreshType> : ObjectClientSide
                    where SpawnType : ISerializable, new()
                    where RefreshType : ISerializable, new()
                {
                    /// <inheritdoc/>
                    protected override void ReadSpawnData(byte[] data)
                    {
                        SpawnType obj = new SpawnType();
                        BinaryUtils.Load(obj, data);
                        InflateFrom(obj);
                    }

                    /// <summary>
                    ///   Inflates the current object from the input data.
                    /// </summary>
                    /// <param name="fullData">The full data to inflate the object from</param>
                    protected abstract void InflateFrom(SpawnType fullData);

                    /// <inheritdoc/>
                    protected override ISerializable ReadRefreshData(byte[] data)
                    {
                        RefreshType obj = new RefreshType();
                        BinaryUtils.Load(obj, data);
                        UpdateFrom(obj);
                        return obj;
                    }

                    /// <summary>
                    ///   Updates the current object from the input data.
                    /// </summary>
                    /// <param name="refreshData">The refresh data to update the object from</param>
                    protected abstract void UpdateFrom(RefreshType refreshData);
                }
            }
        }
    }
}

