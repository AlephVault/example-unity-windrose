using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IList<AuthType, L1, L2, ElementType, ElementIDType, in CursorType>
            {
                // To bind:
                
                public IList<AuthType, ElementType, ElementIDType, CursorType> Bind(L1 l1, L2 l2);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, E, ID, C> GetList<E, ID, C>(string name);
                
                // Particular methods:

                public IElement<AuthType, L1, L2, ElementIDType, ElementType> GetElement(ElementIDType id);
                public Task<Result<ElementType, ElementIDType>> Create(L1 l1, L2 l2, ElementType payload);
                public Task<Result<ElementType, ElementIDType>> List(L1 l1, L2 l2, CursorType cursor);
            }
        }
    }
}