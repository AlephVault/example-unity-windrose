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
            public class SimpleResource<AuthType, ElementType> :
                Resource, ISimpleResource<AuthType, ElementType, string>
            {
                /// <summary>
                ///   Creating a simple resource requires both
                ///   the name and authorization header.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="authorization">The authorization header</param>
                public SimpleResource(string name, Authorization authorization) : base(name, authorization) {}

                public Task<Result<ElementType, string>> Create(ElementType body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Read()
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Update(Dictionary<string, object> changes)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Replace(ElementType replacement)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Delete()
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, string>> View(string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, string>> Operation<E>(string method, Dictionary<string, string> args, E body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, string>> Operation(string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}