using System;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Implementation;
using AlephVault.Unity.RemoteStorage.Types.Interfaces;
using AlephVault.Unity.RemoteStorage.Types.Results;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.RemoteStorage
{
    namespace StandardHttp
    {
        namespace Types
        {
            /// <summary>
            ///   A Standard HTTP MongoDB Storage resource.
            /// </summary>
            public class Resource
            {
                /// <summary>
                ///   The resource name.
                /// </summary>
                public readonly string Name;
                
                // The authorization header.
                protected readonly Authorization Authorization;
                
                /// <summary>
                ///   Creating the resource implies the name and the
                ///   authorization header to use.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="authorization">The authorization header</param>
                public Resource(string name, Authorization authorization)
                {
                    Name = name;
                    Authorization = authorization;
                }

                /// <summary>
                ///   Wraps a function to return its result or wrap an exception
                ///   into a different result.
                /// </summary>
                /// <param name="wrapped">The callback to invoke</param>
                /// <typeparam name="E">The result's element type</typeparam>
                /// <typeparam name="ID">The result's ID type</typeparam>
                /// <returns>A task that, once awaited, returns the underlying result</returns>
                protected async Task<Result<E, string>> WrapException<E>(Func<Task<Result<E, string>>> wrapped)
                {
                    try
                    {
                        return await wrapped();
                    }
                    catch (Engine.Exception e)
                    {
                        return new Result<E, string>
                        {
                            Code = e.Code,
                            ValidationErrors = e.ValidationErrors
                        };
                    }
                }
            }
        }
    }
}