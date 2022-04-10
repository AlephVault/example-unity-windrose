using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface ISimple<AuthType, L1, L2, ElementType, ElementIDType>
            {
                // To bind:
                
                public ISimple<AuthType, ElementType, ElementIDType> Bind(L1 l1, L2 l2);

                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, E, ID, C> GetList<E, ID, C>(string name);
                public IWeak<AuthType, L1, L2, E> GetWeak<E>(string name);
                
                // Particular methods (`Create` makes use of ElementIDType):

                public Task<Result<ElementType, ElementIDType>> Create(ElementType payload);
                public Task<Result<ElementType, ElementIDType>> Get(L1 l1, L2 l2);
                public Task<Result<ElementType, ElementIDType>> Replace(L1 l1, L2 l2, ElementType data);
                public Task<Result<ElementType, ElementIDType>> Update(L1 l1, L2 l2, IDictionary<string, object> data);
                public Task<Result<ElementType, ElementIDType>> Delete(L1 l1, L2 l2);
            }
        }
    }
}