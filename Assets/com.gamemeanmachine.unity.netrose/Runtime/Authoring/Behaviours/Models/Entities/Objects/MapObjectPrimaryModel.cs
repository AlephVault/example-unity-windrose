using AlephVault.Unity.Binary;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Models
            {
                namespace Entities.Objects
                {
                    /// <summary>
                    ///   This behaviours stands for a model holder
                    ///   involving the "public" properties of an
                    ///   object (e.g. aesthetics, if dynamic, and
                    ///   the name, if a player). Each connection
                    ///   will receive this information, even if
                    ///   for a specific connection some censoring
                    ///   may occur.
                    /// </summary>
                    /// <typeparam name="T">The underlying model type</typeparam>
                    [RequireComponent(typeof(MapObject))]
                    public abstract class MapObjectPrimaryModel<T> : MonoBehaviour where T : ISerializable, new()
                    {
                        // The internal, watched, model data.
                        private T data = new T();

                        /// <summary>
                        ///   A read-only access to the whole data.
                        /// </summary>
                        public T FullData => data;
                    }
                }
            }
        }
    }
}