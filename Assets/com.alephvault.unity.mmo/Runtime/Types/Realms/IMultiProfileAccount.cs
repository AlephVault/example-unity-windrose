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
            /// <typeparam name="AccountPreviewType">The type of the account preview data</typeparam>
            /// <typeparam name="ProfileIDType">The type of the profile ID (e.g. int)</typeparam>
            /// <typeparam name="ProfilePreviewType">The type of the profile preview data</typeparam>
            /// <typeparam name="ProfileType">The type of the profile data</typeparam>
            public interface IMultiProfileAccount<AccountIDType, AccountPreviewType, ProfileIDType, ProfilePreviewType, ProfileType> : IAccount<AccountIDType, AccountPreviewType>
                where AccountPreviewType : IAccountPreview<AccountIDType>
                where ProfilePreviewType : IProfilePreview<ProfileIDType>
                where ProfileType : IProfile<ProfileIDType, ProfilePreviewType>
            {
                /// <summary>
                ///   Lists all of the available profiles.
                /// </summary>
                ProfilePreviewType[] GetProfiles();

                /// <summary>
                ///   Gets a single profile by its ID.
                /// </summary>
                /// <param name="id">The ID to get a profile by</param>
                ProfileType GetProfile(ProfileIDType id);
            }
        }
    }
}
