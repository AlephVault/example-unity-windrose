using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IElement<AuthType, L1, L2, L3, L4, ElementType>
            {
                // To bind:

                public IElement<AuthType, ElementType> Bind(L1 l1, L2 l2, L3 l3, L4 l4);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, L4, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, L3, L4, E, ID, C> GetList<E, ID, C>(string name);
                public IWeak<AuthType, L1, L2, L3, L4, E> GetWeak<E>(string name);

                // Particular methods:

                public Task<IResult> Get(L1 l1, L2 l2, L3 l3, L4 l4);
                public Task<IResult> Replace(L1 l1, L2 l2, L3 l3, L4 l4, ElementType data);
                public Task<IResult> Update(L1 l1, L2 l2, L3 l3, L4 l4, IDictionary<string, object> data);
                public Task<IResult> Delete(L1 l1, L2 l2, L3 l3, L4 l4);
            }
        }
    }
}