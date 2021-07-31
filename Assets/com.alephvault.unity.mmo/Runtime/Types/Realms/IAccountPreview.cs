namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   An account is characterized by a custom
            ///   set of preview data. This preview data
            ///   includes at least an ID, and typically
            ///   some sort of preview/display data.
            /// </summary>
            /// <typeparam name="AccountIDType">The type of the account ID (e.g. int)</typeparam>
            public interface IAccountPreview<AccountIDType>
            {
                AccountIDType GetID();
            }
        }
    }
}
