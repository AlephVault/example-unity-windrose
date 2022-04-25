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
            ///   A reference to a list resource. list resources can be created, and
            ///   listed. Their elements (once created) can be updated, replaced,
            ///   deleted and read. Also, methods can be invoked (if available)
            ///   setup either as a read-only operation or a write-enabled operation,
            ///   and also telling whether they should affect or refer a single element
            ///   or affect the whole list.
            /// </summary>
            /// <typeparam name="AuthType">The type to marshal authentication details</typeparam>
            /// <typeparam name="ListElementType">
            ///     The type to marshal the related resource instances when coming in a list.
            ///     This is typically a subset of what is supported in <see cref="ElementType"/>.
            /// </typeparam>
            /// <typeparam name="ElementType">
            ///     The type to marshal the related resource instances when creating, replacing,
            ///     or getting them alone (not as part of a page / listing).
            /// </typeparam>
            /// <typeparam name="IDType">The type to marshal the related resource ids</typeparam>
            /// <typeparam name="CursorType">The type to marshal the paging cursor</typeparam>
            public interface IListResource<AuthType, ListElementType, ElementType, IDType, CursorType>
            {
                /// <summary>
                ///   List a page of resources.
                /// </summary>
                /// <param name="cursor">The cursor to use for the query</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ListElementType, IDType>> List(CursorType cursor);
                
                /// <summary>
                ///   Creates a resource. It may incur in validation errors
                ///   (or even key conflict errors on other instances).
                /// </summary>
                /// <param name="body">The resource body</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Create(ElementType body);
                
                /// <summary>
                ///   Reads a resource. It is an error if the resource does not
                ///   exist by the given key.
                /// </summary>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Read(IDType id);
                
                /// <summary>
                ///   Updates a resource. It is an error if the resource does not
                ///   exist by the given key. It may also incur in validation errors
                ///   (or even key conflict errors on other instances).
                /// </summary>
                /// <param name="changes">The map of changes to apply</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Update(IDType id, JObject changes);
                
                /// <summary>
                ///   Replaces a resource with a new one. It is an error if the
                ///   resource does not exist by the given key. It may also incur in
                ///   validation errors (or even key conflict errors on other
                ///   instances).
                /// </summary>
                /// <param name="replacement">The new resource body</param>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Replace(IDType id, ElementType replacement);
                
                /// <summary>
                ///   Deletes a resource. It is an error if the resource does not
                ///   exist by the given key. It may also incur in referential
                ///   integrity errors (by its potential absence).
                /// </summary>
                /// <returns>A result of the operation</returns>
                public Task<Result<ElementType, IDType>> Delete(IDType id);
                
                /// <summary>
                ///   Queries a view method from the whole list.
                /// </summary>
                /// <param name="method">The method to query</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> View(string method, Dictionary<string, string> args);

                /// <summary>
                ///   Runs an operation method from the whole list.
                ///   It also provides a custom body.
                /// </summary>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <param name="body">The body to send</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> Operation<E>(string method, Dictionary<string, string> args,
                    E body);

                /// <summary>
                ///   Runs an operation method from the whole list.
                /// </summary>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> Operation(string method, Dictionary<string, string> args);

                /// <summary>
                ///   Queries a view method from the whole list
                ///   for a particular item.
                /// </summary>
                /// <param name="item">The intended item in the list</param>
                /// <param name="method">The method to query</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> ItemView(IDType item, string method,
                    Dictionary<string, string> args);

                /// <summary>
                ///   Runs an operation method from the whole list
                ///   for a particular item. It also provides a custom
                ///   body.
                /// </summary>
                /// <param name="item">The intended item in the list</param>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <param name="body">The body to send</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> ItemOperation<E>(IDType item,
                    string method, Dictionary<string, string> args, E body);

                /// <summary>
                ///   Runs an operation method from the whole list
                ///   for a particular item.
                /// </summary>
                /// <param name="item">The intended item in the list</param>
                /// <param name="method">The method to run</param>
                /// <param name="args">The arguments to pass</param>
                /// <returns>A result of the operation. The id type is, actually, typically ignored</returns>
                public Task<Result<JObject, IDType>> ItemOperation(IDType item, string method,
                    Dictionary<string, string> args);
            }
        }
    }
}