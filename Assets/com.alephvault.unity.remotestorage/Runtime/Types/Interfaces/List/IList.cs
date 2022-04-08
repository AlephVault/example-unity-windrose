using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IList<AuthType, ElementType, ElementIDType, in CursorType>
            {
                // To get OTHER resources:
                
                public ISimple<AuthType, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, E, ID, C> GetList<E, ID, C>(string name);
                
                // Particular methods:

                public IElement<AuthType, ElementIDType, ElementType> GetElement(ElementIDType id);
                public Task<IResult> Create(ElementType payload);
                public Task<IResult> List(CursorType cursor);
            }
        }
    }
}