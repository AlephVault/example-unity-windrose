using System.Collections.Generic;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface ISimple<AuthType, ElementType, ElementIDType>
            {
                // To get OTHER resources:
                
                public ISimple<AuthType, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, E, ID, C> GetList<E, ID, C>(string name);
                public IWeak<AuthType, E> GetWeak<E>(string name);
                
                // Particular methods (`Create` makes use of ElementIDType):

                public Task<IResult> Create(ElementType payload);
                public Task<IResult> Get();
                public Task<IResult> Replace(ElementType data);
                public Task<IResult> Update(IDictionary<string, object> data);
                public Task<IResult> Delete();
            }
        }
    }
}