namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Interfaces
        {
            public interface IRoot<AuthType>
            {
                public ISimple<AuthType, E, ID> GetSimple<E, ID>(string name);
                public IList<AuthType, E, ID, C> GetList<E, ID, C>(string name);
            }
        }
    }
}