using AlephVault.Unity.Binary;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Models.Entities.Objects;
using GameMeanMachine.Unity.NetRose.Authoring.Models;
using System;
using System.Collections.Generic;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        /// <summary>
        ///   This is a setup for the netrose protocol, which allows
        ///   the netrose extensions developer to register custom
        ///   types, statically, for the allowed synchronization
        ///   messages. Concretely, this involves: 1. Custom spawn
        ///   message types, 2. Custom property update types, 3.
        ///   Custom "watch" (or "partial spawn" / "add-on") types,
        ///   and 4. Custom property updates for the "watch" types.
        /// </summary>
        public static class NetRoseSetup
        {
            // The registered primary models' sync type and properties.
            private static Dictionary<Type, Tuple<Type, Dictionary<string, Type>>> mapObjectPrimaryModels = new Dictionary<Type, Tuple<Type, Dictionary<string, Type>>>();

            // The registered watched models' sync type and properties.
            private static Dictionary<Type, Tuple<Type, Dictionary<string, Type>>> mapObjectWatchedModels = new Dictionary<Type, Tuple<Type, Dictionary<string, Type>>>();

            /// <summary>
            ///   Registers a <see cref="MapObjectPrimaryModel{T}"/>
            ///   class in the netrose setup for the existing model
            ///   classes (only primary ones are registered here).
            /// </summary>
            /// <typeparam name="ModelClass">The model class</typeparam>
            /// <typeparam name="ModelType">The model class' sync type</typeparam>
            /// <returns>Whether the model was just registered (returns false if it was already registered)</returns>
            public static bool RegisterMapObjectPrimaryModel<ModelClass, ModelType>()
                where ModelClass : MapObjectPrimaryModel<ModelType>
                where ModelType : ISerializable, new()
            {
                if (!mapObjectPrimaryModels.ContainsKey(typeof(ModelClass)))
                {
                    mapObjectPrimaryModels.Add(typeof(ModelClass), new Tuple<Type, Dictionary<string, Type>>(typeof(ModelType), new Dictionary<string, Type>()));
                    return true;
                }
                return false;
            }

            /// <summary>
            ///   Registers a <see cref="MapObjectPrimaryModel{T}"/>
            ///   class in the netrose setup for the existing model
            ///   classes (only watched ones are registered here).
            /// </summary>
            /// <typeparam name="ModelClass">The model class</typeparam>
            /// <typeparam name="ModelType">The model class' sync type</typeparam>
            /// <returns>Whether the model was just registered (returns false if it was already registered)</returns>
            public static bool RegisterMapObjectWatchedModel<ModelClass, ModelType>()
                where ModelClass : MapObjectWatchedModel<ModelType>
                where ModelType : ISerializable, new()
            {
                if (!mapObjectPrimaryModels.ContainsKey(typeof(ModelClass)))
                {
                    mapObjectPrimaryModels.Add(typeof(ModelClass), new Tuple<Type, Dictionary<string, Type>>(typeof(ModelType), new Dictionary<string, Type>()));
                    return true;
                }
                return false;
            }

            /// <summary>
            ///   Registers a property, positionally, for the
            ///   given <see cref="MapObjectPrimaryModel{T}"/>
            ///   type (which must be already registered) and
            ///   a chosen property type.
            /// </summary>
            /// <typeparam name="ModelClass">The model class to register a property for</typeparam>
            /// <typeparam name="PropertyType">The type of the property to register</typeparam>
            /// <param name="propertyName">The name of the property</param>
            /// <returns>Whether the property was just registered (returns false if it was already registered)</returns>
            public static bool RegisterMapObjectPrimaryModelProperty<ModelClass, PropertyType>(string propertyName)
                where PropertyType : ISerializable, new()
            {
                if (mapObjectPrimaryModels.TryGetValue(typeof(ModelClass), out var entry))
                {
                    if (!entry.Item2.ContainsKey(propertyName))
                    {
                        entry.Item2.Add(propertyName, typeof(PropertyType));
                        return true;
                    }
                    return false;
                }
                else
                {
                    throw new ArgumentException("The chosen ModelClass type is not registered - Ensure you call RegisterMapObjectPrimaryModel for it first");
                }
            }

            /// <summary>
            ///   Registers a property, positionally, for the
            ///   given <see cref="MapObjectWatchedModel{T}"/>
            ///   type (which must be already registered) and
            ///   a chosen property type.
            /// </summary>
            /// <typeparam name="ModelClass">The model class to register a property for</typeparam>
            /// <typeparam name="PropertyType">The type of the property to register</typeparam>
            /// <param name="propertyName">The name of the property</param>
            /// <returns>Whether the property was just registered (returns false if it was already registered)</returns>
            public static bool RegisterMapObjectWatchedModelProperty<ModelClass, PropertyType>(string propertyName)
                where PropertyType : ISerializable, new()
            {
                if (mapObjectWatchedModels.TryGetValue(typeof(ModelClass), out var entry))
                {
                    if (!entry.Item2.ContainsKey(propertyName))
                    {
                        entry.Item2.Add(propertyName, typeof(PropertyType));
                        return true;
                    }
                    return false;
                }
                else
                {
                    throw new ArgumentException("The chosen ModelClass type is not registered - Ensure you call RegisterMapObjectPrimaryModel for it first");
                }
            }

            /// <summary>
            ///   Given a particular type (which is a subclass
            ///   of <see cref="MapObjectPrimaryModel{T}"/>),
            ///   this method retrieves it and also enumerates
            ///   its registered properties. The class itself
            ///   is processed inside a callback, and each of
            ///   its registered properties are also processed
            ///   inside another callback (passing both the
            ///   index and the type to it). If the type is not
            ///   registered, an error is thrown.
            /// </summary>
            /// <typeparam name="ModelClass">The registered model class to act upon</typeparam>
            /// <param name="modelClassCallback">A callback to act upon the sync type</param>
            /// <param name="modelPropertyCallback">A callback to act upon each registered property</param>
            public static void WithMapObjectPrimaryModelType<ModelClass>(Action<Type> modelClassCallback, Action<string, Type> modelPropertyCallback)
            {
                if (mapObjectPrimaryModels.TryGetValue(typeof(ModelClass), out var entry))
                {
                    modelClassCallback?.Invoke(entry.Item1);
                    foreach (KeyValuePair<string, Type> pair in entry.Item2)
                    {
                        modelPropertyCallback?.Invoke(pair.Key, pair.Value);
                    }
                }
                else
                {
                    throw new ArgumentException("The chosen ModelClass type is not registered - Ensure you call RegisterMapObjectPrimaryModel for it first");
                }
            }

            /// <summary>
            ///   Given a particular type (which is a subclass
            ///   of <see cref="MapObjectWatchedModel{T}"/>),
            ///   this method retrieves it and also enumerates
            ///   its registered properties. The class itself
            ///   is processed inside a callback, and each of
            ///   its registered properties are also processed
            ///   inside another callback (passing both the
            ///   index and the type to it). If the type is not
            ///   registered, an error is thrown.
            /// </summary>
            /// <typeparam name="ModelClass">The registered model class to act upon</typeparam>
            /// <param name="modelClassCallback">A callback to act upon the sync type</param>
            /// <param name="modelPropertyCallback">A callback to act upon each registered property</param>
            public static void WithMapObjectWatchedModelType<ModelClass>(Action<Type> modelClassCallback, Action<string, Type> modelPropertyCallback)
            {
                if (mapObjectWatchedModels.TryGetValue(typeof(ModelClass), out var entry))
                {
                    modelClassCallback?.Invoke(entry.Item1);
                    foreach (KeyValuePair<string, Type> pair in entry.Item2)
                    {
                        modelPropertyCallback?.Invoke(pair.Key, pair.Value);
                    }
                }
                else
                {
                    throw new ArgumentException("The chosen ModelClass type is not registered - Ensure you call RegisterMapObjectWatchedModel for it first");
                }
            }
        }
    }
}
