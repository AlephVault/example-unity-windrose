using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IWeak<AuthType, L1, L2, L3, ElementType>
            {
                // To bind:

                public IWeak<AuthType, ElementType> Bind(L1 l1, L2 l2, L3 l3);
                
                // To get OTHER resources:
                
                public IWeak<AuthType, L1, L2, L3, E> GetWeak<E>(string name);
                
                // Particular methods:
                
                public Task<Result<ElementType>> Get(L1 l1, L2 l2, L3 l3);
                public Task<Result<ElementType>> Replace(L1 l1, L2 l2, L3 l3, ElementType data);
                public Task<Result<ElementType>> Update(L1 l1, L2 l2, L3 l3, IDictionary<string, object> data);
                public Task<Result<ElementType>> Delete(L1 l1, L2 l2, L3 l3);
            }
        }
    }
}