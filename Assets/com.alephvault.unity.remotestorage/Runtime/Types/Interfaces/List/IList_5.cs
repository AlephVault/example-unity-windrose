using System.Reflection;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IList<AuthType, L1, L2, L3, L4, L5, ElementType, ElementIDType, in CursorType>
            {
                // To bind:
                
                public IList<AuthType, ElementType, ElementIDType, CursorType> Bind(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, L4, L5, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, L3, L4, L5, E, ID, C> GetList<E, ID, C>(string name);
                
                // Particular methods:

                public IElement<AuthType, L1, L2, L3, L4, L5, ElementIDType, ElementType> GetElement(ElementIDType id);
                public Task<Result<ElementType, ElementIDType>> Create(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, ElementType payload);
                public Task<Result<ElementType, ElementIDType>> List(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, CursorType cursor);
            }
        }
    }
}