using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;
using AlephVault.Unity.Support.Generic.Authoring.Types;


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
            public interface ISimple<AuthType, ElementType, IDType>
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
                
                // TODO think tomorrow about the invocation of custom methods.
            }
        }
    }
}