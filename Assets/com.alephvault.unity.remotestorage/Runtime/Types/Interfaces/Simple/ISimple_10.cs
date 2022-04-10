using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface ISimple<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, ElementType, ElementIDType>
            {
                // To bind:
                
                public ISimple<AuthType, ElementType, ElementIDType> Bind(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);

                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, E, ID> GetSimple<E, ID>(string name);
                public IWeak<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, E> GetWeak<E>(string name);
                
                // Particular methods (`Create` makes use of ElementIDType):

                public Task<Result<ElementType, ElementIDType>> Create(ElementType payload);
                public Task<Result<ElementType, ElementIDType>> Get(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);
                public Task<Result<ElementType, ElementIDType>> Replace(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10, ElementType data);
                public Task<Result<ElementType, ElementIDType>> Update(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10, IDictionary<string, object> data);
                public Task<Result<ElementType, ElementIDType>> Delete(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, L10 l10);
            }
        }
    }
}