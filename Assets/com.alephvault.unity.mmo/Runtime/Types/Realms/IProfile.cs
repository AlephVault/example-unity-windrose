using MLAPI.Serialization;

namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   Profiles depend on more data than just
            ///   the preview/display data. So a full
            ///   profile data can access all of the
            ///   preview data and its id.
            /// </summary>
            /// <typeparam name="ProfileIDType">The type of the profile ID (e.g. int)</typeparam>
            /// <typeparam name="ProfilePreviewType">The type of the profile preview data</typeparam>
            public interface IProfile<ProfileIDType, ProfilePreviewType> where ProfilePreviewType : IProfilePreview<ProfileIDType>, INetworkSerializable
            {
                /// <summary>
                ///   Returns the preview data of this profile.
                /// </summary>
                ProfilePreviewType GetPreview();
            }
        }
    }
}
