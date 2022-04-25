using System;
using AlephVault.Unity.RemoteStorage.Types.Interfaces;


namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   A Standard HTTP MongoDB Storage root node.
            /// </summary>
            public class Root : IRoot<Authorization>
            {
                // The authorization to use.
                private Authorization Authorization;
                
                /// <summary>
                ///   On creation, it takes an authorization header.
                /// </summary>
                /// <param name="authorization">The authorization header to use</param>
                /// <exception cref="ArgumentNullException">The authorization is null</exception>
                public Root(Authorization authorization)
                {
                    Authorization = authorization ?? throw new ArgumentNullException(nameof(authorization));
                }
                
                /// <summary>
                ///   Returns a Standard HTTP MongoDB Storage simple resource.
                /// </summary>
                /// <param name="name">The name of the resource</param>
                /// <typeparam name="E">The element type of the resource</typeparam>
                /// <typeparam name="ID">The id type of the resource</typeparam>
                /// <returns>A simple resource handler</returns>
                public ISimpleResource<Authorization, E, ID> GetSimple<E, ID>(string name)
                {
                    if (typeof(ID) != typeof(string))
                    {
                        throw new ArgumentException("Only string is supported as ID type");
                    }

                    // Just create a resource.
                    return new SimpleResource<Authorization, E, ID>(name, Authorization);
                }

                /// <summary>
                ///   Returns a Standard HTTP MongoDB Storage simple resource.
                /// </summary>
                /// <param name="name">The name of the resource</param>
                /// <typeparam name="E">The element type of the resource</typeparam>
                /// <returns>A simple resource handler</returns>
                public ISimpleResource<Authorization, E, string> GetSimple<E>(string name)
                {
                    // Just create a resource.
                    return new SimpleResource<Authorization, E, string>(name, Authorization);
                }

                /// <summary>
                ///   Returns a Standard HTTP MongoDB Storage list resource.
                /// </summary>
                /// <param name="name">The name of the resource</param>
                /// <typeparam name="LE">The list-element type of the resource</typeparam>
                /// <typeparam name="E">The element type of the resource</typeparam>
                /// <typeparam name="ID">The id type of the resource</typeparam>
                /// <typeparam name="C">The cursor type</typeparam>
                /// <returns>A simple resource handler</returns>
                public IListResource<Authorization, LE, E, ID, C> GetList<LE, E, ID, C>(string name)
                {
                    if (typeof(ID) != typeof(string) || typeof(C) != typeof(Cursor))
                    {
                        throw new ArgumentException("Only string is supported as ID type, and Cursor " +
                                                    "as cursor type");
                    }
                    
                    // Just create a resource.
                    return new ListResource<Authorization, LE, E, ID, C>(name, Authorization);
                }
                
                
                /// <summary>
                ///   Returns a Standard HTTP MongoDB Storage list resource.
                /// </summary>
                /// <param name="name">The name of the resource</param>
                /// <typeparam name="LE">The list-element type of the resource</typeparam>
                /// <typeparam name="E">The element type of the resource</typeparam>
                /// <returns>A simple resource handler</returns>
                public IListResource<Authorization, LE, E, string, Cursor> GetList<LE, E>(string name)
                {
                    // Just create a resource.
                    return new ListResource<Authorization, LE, E, string, Cursor>(name, Authorization);
                }
            }
        }
    }
}