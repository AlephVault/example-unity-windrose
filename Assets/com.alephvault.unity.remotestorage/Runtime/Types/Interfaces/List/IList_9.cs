using System.Reflection;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IList<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, ElementType, ElementIDType, in CursorType>
            {
                // To bind:
                
                public IList<AuthType, ElementType, ElementIDType, CursorType> Bind(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9);
                
                // To get OTHER resources:
                
                public ISimple<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, E, ID, C> GetList<E, ID, C>(string name);
                
                // Particular methods:

                public IElement<AuthType, L1, L2, L3, L4, L5, L6, L7, L8, L9, ElementIDType, ElementType> GetElement(ElementIDType id);
                public Task<IResult> Create(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, ElementType payload);
                public Task<IResult> List(L1 l1, L2 l2, L3 l3, L4 l4, L5 l5, L6 l6, L7 l7, L8 l8, L9 l9, CursorType cursor);
            }
        }
    }
}