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
            /// <typeparam name="ElementType">The type to marshal the related resource instances</typeparam>
            /// <typeparam name="IDType">The type to marshal the related resource ids</typeparam>
            /// <typeparam name="CursorType">The type to marshal the paging cursor</typeparam>
            public interface IList<AuthType, ElementType, IDType, CursorType>
            {
            }
        }
    }
}