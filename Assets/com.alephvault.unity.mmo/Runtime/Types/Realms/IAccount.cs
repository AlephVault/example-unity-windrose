using MLAPI.Serialization;

namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   Accounts depend on more data than just
            ///   the preview/display data. So a full
            ///   account data can access all of the
            ///   preview data and its id.
            /// </summary>
            /// <typeparam name="AccountIDType">The type of the account ID (e.g. int)</typeparam>
            /// <typeparam name="AccountPreviewType">The type of the account preview data</typeparam>
            public interface IAccount<AccountIDType, AccountPreviewType> where AccountPreviewType : IAccountPreview<AccountIDType>, INetworkSerializable
            {
                /// <summary>
                ///   Returns the preview data of this account.
                /// </summary>
                AccountPreviewType GetPreview();
            }
        }
    }
}
