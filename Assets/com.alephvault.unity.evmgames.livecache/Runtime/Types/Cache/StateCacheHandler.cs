using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.EVMGames.LiveCache
{
    namespace Types
    {
        namespace Cache
        {
            /// <summary>
            ///   An abstraction over an EVM State resource.
            ///   Mainly intended as abstraction to trigger
            ///   the grabber method.
            /// </summary>
            public class StateCacheHandler
            {
                // The "EVM State" resource.
                public readonly SimpleResource<State> StateResource;
                
                /// <summary>
                ///   Creates the instance from a specific resource
                ///   (a root one) and a resource.
                /// </summary>
                /// <param name="root">The root resource</param>
                /// <param name="resource">The resource key</param>
                public StateCacheHandler(Root root, string resource = "evm-state")
                {
                    StateResource = (SimpleResource<State>)root.GetSimple<State>(resource);
                }

                /// <summary>
                ///   Executes a grab operation, and converts everything
                ///   to an array of arbitrary elements.
                /// </summary>
                /// <returns>Returns the grab</returns>
                public async Task<Result<JObject[], string>> Grab()
                {
                    Result<JObject[], string> result = await StateResource.OperationTo<JObject[]>("grab", null);
                    return new Result<JObject[], string>
                    {
                        Element = result.Element,
                        Code = result.Code
                    };
                }
            }
        }
    }
}
