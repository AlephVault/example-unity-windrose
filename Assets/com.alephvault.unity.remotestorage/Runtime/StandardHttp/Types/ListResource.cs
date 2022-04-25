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
            ///   A Standard HTTP MongoDB Storage list resource.
            /// </summary>
            public class ListResource<AuthType, ListType, ElementType> :
                Resource, IListResource<AuthType, ListType, ElementType, string, Cursor>
            {
                /// <summary>
                ///   Creating a list resource requires both
                ///   the name and authorization header.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="authorization">The authorization header</param>
                public ListResource(string name, Authorization authorization) : base(name, authorization) {}

                public Task<Result<ListType[], string>> List(Cursor cursor)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Create(ElementType body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Read(string id)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Update(string id, Dictionary<string, object> changes)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Replace(string id, ElementType replacement)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, string>> Delete(string id)
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

                public Task<Result<JObject, string>> ItemView(string item, string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, string>> ItemOperation<E>(string item, string method, Dictionary<string, string> args, E body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, string>> ItemOperation(string item, string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}