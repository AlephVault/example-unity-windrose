using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IWeak<AuthType, L1, ElementType>
            {
                // To bind:

                public IWeak<AuthType, ElementType> Bind(L1 l1);
                
                // To get OTHER resources:
                
                public IWeak<AuthType, L1, E> GetWeak<E>(string name);
                
                // Particular methods:
                
                public Task<IResult> Get(L1 l1);
                public Task<IResult> Replace(L1 l1, ElementType data);
                public Task<IResult> Update(L1 l1, IDictionary<string, object> data);
                public Task<IResult> Delete(L1 l1);
            }
        }
    }
}