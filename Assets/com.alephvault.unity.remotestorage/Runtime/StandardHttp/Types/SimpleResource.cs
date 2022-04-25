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
            ///   A Standard HTTP MongoDB Storage simple resource.
            /// </summary>
            public class SimpleResource<AuthType, ElementType, IDType> :
                Resource, ISimpleResource<AuthType, ElementType, IDType>
            {
                /// <summary>
                ///   Creating a simple resource requires both
                ///   the name and authorization header.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="authorization">The authorization header</param>
                public SimpleResource(string name, Authorization authorization) : base(name, authorization) {}

                public Task<Result<ElementType, IDType>> Create(ElementType body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Read()
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Update(Dictionary<string, object> changes)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Replace(ElementType replacement)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Delete()
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, IDType>> View(string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, IDType>> Operation<E>(string method, Dictionary<string, string> args, E body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, IDType>> Operation(string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}