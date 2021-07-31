namespace AlephVault.Unity.MMO
{
    namespace Types
    {
        namespace Realms
        {
            /// <summary>
            ///   A profile is characterized by a custom
            ///   set of preview data. This preview data
            ///   includes at least an ID, and typically
            ///   some sort of preview/display data.
            /// </summary>
            /// <typeparam name="ProfileIDType">The type of the profile ID (e.g. int)</typeparam>
            public interface IProfilePreview<ProfileIDType>
            {
                ProfileIDType GetID();
            }
        }
    }
}
