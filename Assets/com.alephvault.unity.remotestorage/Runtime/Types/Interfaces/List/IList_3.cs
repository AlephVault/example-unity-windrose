using System.Reflection;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IList<AuthType, L1, L2, L3, ElementType, ElementIDType, in CursorType>
            {
                // To bind:
                
                public IList<AuthType, ElementType, ElementIDType, CursorType> Bind(L1 l1, L2 l2, L3 l3);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, L3, E, ID, C> GetList<E, ID, C>(string name);
                
                // Particular methods:

                public IElement<AuthType, L1, L2, L3, ElementIDType, ElementType> GetElement(ElementIDType id);
                public Task<IResult> Create(L1 l1, L2 l2, L3 l3, ElementType payload);
                public Task<IResult> List(L1 l1, L2 l2, L3 l3, CursorType cursor);
            }
        }
    }
}