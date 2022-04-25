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
            public class ListResource<AuthType, ListType, ElementType, IDType, CursorType> :
                IListResource<AuthType, ListType, ElementType, IDType, CursorType>
            {
                /// <summary>
                ///   The resource name.
                /// </summary>
                public readonly string Name;
                
                // The authorization header.
                private readonly Authorization Authorization;
                
                /// <summary>
                ///   Creating the resource implies the name and the
                ///   authorization header to use.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="authorization">The authorization header</param>
                public ListResource(string name, Authorization authorization)
                {
                    Name = name;
                    Authorization = authorization;
                }

                public Task<Result<ListType[], IDType>> List(CursorType cursor)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Create(ElementType body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Read(IDType id)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Update(IDType id, Dictionary<string, object> changes)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Replace(IDType id, ElementType replacement)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<ElementType, IDType>> Delete(IDType id)
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

                public Task<Result<JObject, IDType>> ItemView(IDType item, string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, IDType>> ItemOperation<E>(IDType item, string method, Dictionary<string, string> args, E body)
                {
                    throw new NotImplementedException();
                }

                public Task<Result<JObject, IDType>> ItemOperation(IDType item, string method, Dictionary<string, string> args)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}