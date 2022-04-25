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
                ///   the name and authorization header, as well
                ///   as the base endpoint to hit.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="baseEndpoint">The base endpoint</param>
                /// <param name="authorization">The authorization header</param>
                public ListResource(string name, string baseEndpoint, Authorization authorization) : base(name, baseEndpoint, authorization) {}

                public Task<Result<ListType, string>> List(Cursor cursor)
                {
                    return WrapException(async () =>
                    {
                        ListType[] result = await Engine.List<ListType, Authorization>(
                            $"{BaseEndpoint}/{Name}", Authorization, cursor
                        );
                        return new Result<ListType, string>
                        {
                            Code = ResultCode.Ok,
                            Elements = result
                        };
                    });
                }

                public Task<Result<ElementType, string>> Create(ElementType body)
                {
                    return WrapException(async () =>
                    {
                        string id = await Engine.Create($"{BaseEndpoint}/{Name}", Authorization, body);
                        return new Result<ElementType, string>
                        {
                            Code = ResultCode.Created,
                            CreatedID = id
                        };
                    });
                }

                public Task<Result<ElementType, string>> Read(string id)
                {
                    return WrapException(async () =>
                    {
                        ElementType result = await Engine.One<ElementType, Authorization>(
                            $"{BaseEndpoint}/{Name}", Authorization
                        );
                        return new Result<ElementType, string>
                        {
                            Code = ResultCode.Ok,
                            Element = result
                        };
                    });
                }

                public Task<Result<ElementType, string>> Update(string id, JObject changes)
                {
                    return WrapException(async () =>
                    {
                        await Engine.Update(
                            $"{BaseEndpoint}/{Name}", Authorization, changes
                        );
                        return new Result<ElementType, string>
                        {
                            Code = ResultCode.Ok
                        };
                    });
                }

                public Task<Result<ElementType, string>> Replace(string id, ElementType replacement)
                {
                    return WrapException(async () =>
                    {
                        await Engine.Replace(
                            $"{BaseEndpoint}/{Name}", Authorization, replacement
                        );
                        return new Result<ElementType, string>
                        {
                            Code = ResultCode.Ok
                        };
                    });
                }

                public Task<Result<ElementType, string>> Delete(string id)
                {
                    return WrapException(async () =>
                    {
                        await Engine.Delete($"{BaseEndpoint}/{Name}", Authorization);
                        return new Result<ElementType, string>
                        {
                            Code = ResultCode.Ok
                        };
                    });
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