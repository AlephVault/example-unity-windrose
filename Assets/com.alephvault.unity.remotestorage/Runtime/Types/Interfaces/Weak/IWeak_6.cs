using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IWeak<AuthType, L1, L2, L3, L4, L5, L6, ElementType>
            {
                // To bind:

                public IWeak<AuthType, ElementType> Bind(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6);
                
                // To get OTHER resources:
                
                public IWeak<AuthType, L1, L2, L3, L4, L5, L6, E> GetWeak<E>(string name);
                
                // Particular methods:
                
                public Task<IResult> Get(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6);
                public Task<IResult> Replace(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, ElementType data);
                public Task<IResult> Update(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, IDictionary<string, object> data);
                public Task<IResult> Delete(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6);
            }
        }
    }
}