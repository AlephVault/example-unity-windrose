using AlephVault.Unity.Binary;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using System;
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
                    ///   involving a particular non-public view
                    ///   over an object (e.g. its inventory, or
                    ///   its health status).
                    /// </summary>
                    /// <typeparam name="T">The underlying model type</typeparam>
                    [RequireComponent(typeof(MapObject))]
                    public abstract class MapObjectWatchedModel<T> : MonoBehaviour where T : ISerializable, new()
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