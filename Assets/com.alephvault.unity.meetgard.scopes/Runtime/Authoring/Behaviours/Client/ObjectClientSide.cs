using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Support.Authoring.Behaviours;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   The client side implementation of an object. This is
                ///   an exact mirror of what happens in the server side.
                ///   It has features to refresh itself when something is
                ///   happens in the server, and has event to notify when
                ///   it was refreshed, spawned or de-spawned.
                /// </summary>
                public abstract class ObjectClientSide : MonoBehaviour
                {
                    // This field is set by the protocol.

                    /// <summary>
                    ///   The protocol this object is associated to. Typically,
                    ///   this means that an object is created in that particular
                    ///   client and with a particular prefab therein.
                    /// </summary>
                    public ScopesProtocolClientSide Protocol { get; internal set; }

                    // This field is set by the owning scope.

                    /// <summary>
                    ///   The id this object is assigned to inside a particular
                    ///   scope. This also requires a particular scope to be set
                    ///   in the <see cref="Scope"/> field. Also, the scope being
                    ///   assigned will belong to the same protocol that was given
                    ///   as value in the <see cref="Protocol"/> field. This
                    ///   means that this value is meaningless, even when being
                    ///   zero, if <see cref="Protocol"/> is not set.
                    /// </summary>
                    public uint Id { get; private set; }

                    /// <summary>
                    ///   If the object is spawned, this field will have which
                    ///   scope this object is spawned into. If the object belongs
                    ///   to a server but it is not spawned, this value will be
                    ///   null. When this value is set, the <see cref="Id"/> field
                    ///   will have a meaninful value: the id this object was given
                    ///   when spawning this object (and also populating this field).
                    /// </summary>
                    public ScopeClientSide Scope { get; private set; }

                    /// <summary>
                    ///   Tells whether this object is spawned or not.
                    /// </summary>
                    public bool Spawned => Scope != null;

                    /// <summary>
                    ///   What to do when the object is spawned.
                    /// </summary>
                    public event Action OnSpawned;

                    /// <summary>
                    ///   What to do when the object is refreshed. Since the object can
                    ///   be expressed with different levels of censorship, the object
                    ///   will also be provided for this event so further behaviours
                    ///   will be able to act in consequence.
                    /// </summary>
                    public event Action<ISerializable> OnRefreshed;

                    /// <summary>
                    ///   What to do when the object is despawned.
                    /// </summary>
                    public event Action OnDespawned;

                    /// <summary>
                    ///   Spawns an object using a content of incoming data. The data
                    ///   must match the format / class that is used in the corresponding
                    ///   server side counterpart. See <see cref="ReadSpawnData(byte[])"/>
                    ///   for more details.
                    /// </summary>
                    /// <param name="scope">The scope to attach this object to</param>
                    /// <param name="id">The id to give to this object</param>
                    /// <param name="data">The data to use for spawning</param>
                    public void Spawn(ScopeClientSide scope, uint id, byte[] data)
                    {
                        if (scope == null)
                        {
                            throw new ArgumentNullException("scope");
                        }

                        if (data == null)
                        {
                            throw new ArgumentNullException("data");
                        }

                        if (!gameObject)
                        {
                            throw new InvalidOperationException("Cannot spawn a destroyed object");
                        }

                        if (Spawned)
                        {
                            throw new InvalidOperationException("This object is already spawned");
                        }

                        Id = id;
                        Scope = scope;
                        ReadSpawnData(data);

                        if (GetComponentInParent<ScopeClientSide>() != scope)
                        {
                            transform.SetParent(scope.transform);
                        }

                        OnSpawned?.Invoke();
                    }

                    /// <summary>
                    ///   Override this function to define how does the object inflate
                    ///   from the incoming byte array. This byte array must typically
                    ///   be de-serialized into an object of a given class that matches
                    ///   what the server sends.
                    /// </summary>
                    /// <param name="data">The data to de-serialize</param>
                    protected abstract void ReadSpawnData(byte[] data);

                    /// <summary>
                    ///   Despawns the object. Only valid when the object is already
                    ///   spawned.
                    /// </summary>
                    public void Despawn()
                    {
                        if (!gameObject)
                        {
                            throw new InvalidOperationException("Cannot despawn a destroyed object");
                        }

                        if (!Spawned)
                        {
                            throw new InvalidOperationException("This object is not spawned");
                        }

                        if (GetComponentInParent<ScopeClientSide>() == Scope)
                        {
                            transform.SetParent(null);
                        }

                        Scope = null;

                        OnDespawned?.Invoke();
                    }

                    /// <summary>
                    ///   Refreshes the object. Only valid when the object is already
                    ///   spawned.
                    /// </summary>
                    /// <param name="data">The data to perform the refresh with. It might be partially censored</param>
                    /// <returns>The de-serialized model</returns>
                    public ISerializable Refresh(byte[] data)
                    {
                        if (!gameObject)
                        {
                            throw new InvalidOperationException("Cannot refresh a destroyed object");
                        }

                        if (!Spawned)
                        {
                            throw new InvalidOperationException("This object is not spawned");
                        }

                        ISerializable model = ReadRefreshData(data);
                        OnRefreshed?.Invoke(model);
                        return model;
                    }

                    /// <summary>
                    ///   Override this function to define how does the object refresh
                    ///   from the incoming byte array. This byte array must typically
                    ///   be de-serialized into an object of a given class that matches
                    ///   what the server sends, but the data might be partially/fully
                    ///   censored, depending on the game needs. The object must update
                    ///   itself from the input data, and also the de-serialized model
                    ///   must be returned for further processing.
                    /// </summary>
                    /// <param name="data">The data to de-serialize</param>
                    /// <returns>The de-serialized model</returns>
                    protected abstract ISerializable ReadRefreshData(byte[] data);
                }
            }
        }
    }
}

