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
            public class SimpleResource<ElementType> :
                Resource, ISimpleResource<Authorization, ElementType, string>
            {
                /// <summary>
                ///   Creating a simple resource requires both
                ///   the name and authorization header, as well
                ///   as the base endpoint to hit.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <param name="baseEndpoint">The base endpoint</param>
                /// <param name="authorization">The authorization header</param>
                public SimpleResource(string name, string baseEndpoint, Authorization authorization) :
                    base(name, baseEndpoint, authorization) {}

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

                public Task<Result<ElementType, string>> Read()
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

                public Task<Result<ElementType, string>> Update(JObject changes)
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

                public Task<Result<ElementType, string>> Replace(ElementType replacement)
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

                public Task<Result<ElementType, string>> Delete()
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
                    return WrapException(async () =>
                    {
                        JObject obj = await Engine.View($"{BaseEndpoint}/{Name}/~{method}", Authorization, args);
                        return new Result<JObject, string>
                        {
                            Element = obj,
                            Code = ResultCode.Ok
                        };
                    });
                }

                public Task<Result<JObject, string>> Operation<E>(string method, Dictionary<string, string> args, E body)
                {
                    return WrapException(async () =>
                    {
                        JObject obj = await Engine.Operation($"{BaseEndpoint}/{Name}/~{method}", Authorization, args, body);
                        return new Result<JObject, string>
                        {
                            Element = obj,
                            Code = ResultCode.Ok
                        };
                    });
                }

                public Task<Result<JObject, string>> Operation(string method, Dictionary<string, string> args)
                {
                    return WrapException(async () =>
                    {
                        JObject obj = await Engine.Operation($"{BaseEndpoint}/{Name}/~{method}", Authorization, args);
                        return new Result<JObject, string>
                        {
                            Element = obj,
                            Code = ResultCode.Ok
                        };
                    });
                }
            }
        }
    }
}