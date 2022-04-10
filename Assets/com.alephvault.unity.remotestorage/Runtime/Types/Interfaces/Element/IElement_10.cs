using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IElement<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, ElementType>
            {
                // To bind:

                public IElement<AuthType, ElementType> Bind(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, E, ID> GetSimple<E, ID>(string name);
                public IWeak<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, E> GetWeak<E>(string name);

                // Particular methods:

                public Task<Result<ElementType>> Get(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);
                public Task<Result<ElementType>> Replace(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10, ElementType data);
                public Task<Result<ElementType>> Update(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10, IDictionary<string, object> data);
                public Task<Result<ElementType>> Delete(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);
            }
        }
    }
}