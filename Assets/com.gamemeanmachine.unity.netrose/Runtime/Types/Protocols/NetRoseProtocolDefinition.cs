using AlephVault.Unity.Meetgard.Protocols;
using GameMeanMachine.Unity.NetRose.Authoring.Models;
using System;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Types
    {
        namespace Protocols
        {
            /// <summary>
            ///   This is an abstract class for the NetRose protocol and involved
            ///   subclasses. It defines the standard NetRose messages and a way
            ///   to install custom NetRose MapObject models, their sync types,
            ///   and their properties.
            /// </summary>
            public abstract class NetRoseProtocolDefinition : ProtocolDefinition
            {
                /// <summary>
                ///   Defines all the messages for a given registered
                ///   <see cref="MapObjectPrimaryModel{T}"/> subclass.
                ///   Subclasses of this definition type must call this
                ///   method for every model being interested in.
                /// </summary>
                /// <typeparam name="ModelClass">The registered model class to prepare their messages</typeparam>
                protected void DefineSpawnedModel<ModelClass>()
                {
                    NetRoseSetup.WithMapObjectWatchedModelType<ModelClass>((modelType) =>
                    {
                        Type genericObjectSpawned = typeof(ObjectSpawned<>);
                        Type concreteObjectSpawned = genericObjectSpawned.MakeGenericType(modelType);
                        DefineServerMessage($"Spawned:{typeof(ModelClass).FullName}", concreteObjectSpawned);
                    }, (property, propertyType) =>
                    {
                        Type genericObjectWatched = typeof(ObjectUpdated<>);
                        Type concreteObjectWatched = genericObjectWatched.MakeGenericType(propertyType);
                        DefineServerMessage($"Updated:{typeof(ModelClass).FullName}.{property}", concreteObjectWatched);
                    });
                }

                /// <summary>
                ///   Defines all the messages for a given registered
                ///   <see cref="MapObjectWatchedModel{T}"/> subclass.
                ///   Subclasses of this definition type must call this
                ///   method for every model being interested in.
                /// </summary>
                /// <typeparam name="ModelClass">The registered model class to prepare their messages</typeparam>
                protected void DefineWatchedModel<ModelClass>()
                {
                    NetRoseSetup.WithMapObjectWatchedModelType<ModelClass>((modelType) =>
                    {
                        Type genericObjectWatched = typeof(ObjectWatched<>);
                        Type concreteObjectWatched = genericObjectWatched.MakeGenericType(modelType);
                        DefineServerMessage($"Watched:{typeof(ModelClass).FullName}", concreteObjectWatched);
                    }, (property, propertyType) =>
                    {
                        Type genericObjectWatched = typeof(ObjectUpdated<>);
                        Type concreteObjectWatched = genericObjectWatched.MakeGenericType(propertyType);
                        DefineServerMessage($"Updated:{typeof(ModelClass).FullName}.{property}", concreteObjectWatched);
                    });
                }

                /// <summary>
                ///   Override this method with several calls to
                ///   <see cref="DefineSpawnedModel{ModelClass}"/>.
                /// </summary>
                protected abstract void DefineSpawnedModels();

                /// <summary>
                ///   Override this method with several calls to
                ///   <see cref="DefineWatchedModel{ModelClass}"/>.
                /// </summary>
                protected abstract void DefineWatchedModels();

                /// <summary>
                ///   Defines all the basic NetRose messages, which involve:
                ///   - All the needed Spawned messages (by the user in <see cref="DefineSpawnedModels"/>).
                ///   - All the needed Watched messages (by the user in <see cref="DefineSpawnedModels"/> and <see cref="DefineWatchedModels"/>).
                ///   - All the needed Updated messages (by the user in <see cref="DefineWatchedModels"/>).
                ///   - Scope added/removed messages.
                ///   - Object attached/detached messages.
                ///   - Object movement messages, and speed/orientation ones.
                ///   - Unwatched and Despawned messages.
                /// </summary>
                protected override void DefineMessages()
                {
                    DefineServerMessage<AddedToScope>("Scope:Added");
                    DefineSpawnedModels();
                    DefineWatchedModels();
                    DefineServerMessage<ObjectAttached>("Object:Attached");
                    DefineServerMessage<ObjectDetached>("Object:Detached");
                    DefineServerMessage<ObjectMovementStarted>("Object:Movement:Started");
                    DefineServerMessage<ObjectMovementFinished>("Object:Movement:Cancelled");
                    DefineServerMessage<ObjectMovementFinished>("Object:Movement:Finished");
                    DefineServerMessage<ObjectTeleported>("Object:Teleported");
                    DefineServerMessage<ObjectSpeedChanged>("Object:Speed:Changed");
                    DefineServerMessage<ObjectOrientationChanged>("Object:Orientation:Changed");
                    DefineServerMessage<ObjectUnwatched>("Unwatched");
                    DefineServerMessage<ObjectDespawned>("Despawned");
                    DefineServerMessage<RemovedFromScope>("Scope:Removed");
                }
            }
        }
    }
}
