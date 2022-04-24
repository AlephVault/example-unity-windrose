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
            }
        }
    }
}