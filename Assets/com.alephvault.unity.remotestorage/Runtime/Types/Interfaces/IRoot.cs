namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            /// <summary>
            ///   A root only allows referencing the remote resources to hit.
            ///   Those resources are either simple or list resources.
            /// </summary>
            /// <typeparam name="AuthType">The auth type to use</typeparam>
            public interface IRoot<AuthType>
            {
                /// <summary>
                ///   Retrieves an instance of a handler to a remote simple resource.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <typeparam name="E">The type to marshal the related resource instances</typeparam>
                /// <typeparam name="ID">The type to marshal the related resource ids</typeparam>
                /// <returns>A simple resource reference</returns>
                public ISimple<AuthType, E, ID> GetSimple<E, ID>(string name);
                
                /// <summary>
                ///   Retrieves an instance of a handler to a remote list resource.
                /// </summary>
                /// <param name="name">The resource name</param>
                /// <typeparam name="E">The type to marshal the related resource instances</typeparam>
                /// <typeparam name="ID">The type to marshal the related resource ids</typeparam>
                /// <typeparam name="C">The type to marshal the paging cursor</typeparam>
                /// <returns>A simple resource reference</returns>
                public IList<AuthType, E, ID, C> GetList<E, ID, C>(string name);
            }
        }
    }
}