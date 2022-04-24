using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using Newtonsoft.Json.Linq;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            /// <summary>
            ///   A reference to a simple resource. Simple resources can be
            ///   created, updated, replaced, deleted and read. Also, methods
            ///   can be invoked (if available), setup either as a read-only
            ///   operation or a write-enabled operation.
            /// </summary>
            /// <typeparam name="AuthType">The type to marshal authentication details</typeparam>
            /// <typeparam name="ElementType">The type to marshal the related resource instances</typeparam>
            /// <typeparam name="IDType">The type to marshal the related resource ids</typeparam>
            public interface ISimpleResource<AuthType, ElementType, IDType>
            {
                /// <summary>
                ///   Creates the resource. It is an error if the resource is
                ///   already created. It may also incur in validation errors
                ///   (or even key conflict errors on soft-deleted instances).
                /// </summary>
                /// <param name="body">The resource body</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Create(ElementType body);
                
                /// <summary>
                ///   Reads the resource. It is an error if the resource is not
                ///   already created.
                /// </summary>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Read();
                
                /// <summary>
                ///   Updates the resource. It is an error if the resource is
                ///   not already created. It may also incur in validation errors
                ///   (or even key conflict errors on soft-deleted instances).
                /// </summary>
                /// <param name="changes">The map of changes to apply</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Update(Dictionary<string, object> changes);
                
                /// <summary>
                ///   Replaces the resource with a new one. It is an error if the
                ///   resource is not already created. It may also incur in validation
                ///   errors (or even key conflict errors on soft-deleted instances).
                /// </summary>
                /// <param name="replacement">The new resource body</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Replace(ElementType replacement);
                
                /// <summary>
                ///   Deletes the resource. It is an error if the resource is not
                ///   already created.
                /// </summary>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Delete();
                
                /// <summary>
                ///   Queries a view method from the only item in the
                ///   collection.
                /// </summary>
                /// <param name="method">The method to query</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> View(string method, Dictionary<string, string> args);

                /// <summary>
                ///   Runs an operation method from the only item in
                ///   the collection. It also provides a custom body.
                /// </summary>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <param name="body">The body to send</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> Operation<E>(string method, Dictionary<string, string> args,
                    E body);

                /// <summary>
                ///   Runs an operation method from the only item in
                ///   the collection.
                /// </summary>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> Operation(string method, Dictionary<string, string> args);
            }
        }
    }
}