using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IWeak<AuthType, ElementType>
            {
                // To get OTHER resources:
                
                public IWeak<AuthType, E> GetWeak<E>(string name);
                
                // Particular methods:
                
                public Task<Result<ElementType>> Get();
                public Task<Result<ElementType>> Replace(ElementType data);
                public Task<Result<ElementType>> Update(IDictionary<string, object> data);
                public Task<Result<ElementType>> Delete();
            }
        }
    }
}