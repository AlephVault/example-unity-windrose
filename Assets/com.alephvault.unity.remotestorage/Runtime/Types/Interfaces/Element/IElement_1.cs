using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IElement<AuthType, L1, ElementType>
            {
                // To bind:

                public IElement<AuthType, ElementType> Bind(L1 l1);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, E, ID, C> GetList<E, ID, C>(string name);
                public IWeak<AuthType, L1, E> GetWeak<E>(string name);

                // Particular methods:

                public Task<Result<ElementType>> Get(L1 l1);
                public Task<Result<ElementType>> Replace(L1 l1, ElementType data);
                public Task<Result<ElementType>> Update(L1 l1, IDictionary<string, object> data);
                public Task<Result<ElementType>> Delete(L1 l1);
            }
        }
    }
}