namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   Multi-profile accounts are accounts that also contain the list
            ///   of profiles they contain.
            /// </summary>
            /// <typeparam name="AccountIDType">The type of the account ID (e.g. int)</typeparam>
            public interface IMultiProfileAccount<AccountIDType, AccountPreviewType, ProfileIDType, ProfilePreviewType, ProfileType> : IAccount<AccountIDType, AccountPreviewType>
                where AccountPreviewType : IAccountPreview<AccountIDType>
                where ProfilePreviewType : IProfilePreview<ProfileIDType>
                where ProfileType : IProfile<ProfileIDType, ProfilePreviewType>
            {
                /// <summary>
                ///   Lists all of the available profiles.
                /// </summary>
                ProfilePreviewType[] GetProfiles();
            }
        }
    }
}
