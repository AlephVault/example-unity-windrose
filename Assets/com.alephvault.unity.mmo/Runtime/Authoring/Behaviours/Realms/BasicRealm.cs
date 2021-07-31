using System.Collections;
using System.Collections.Generic;
using AlephVault.Unity.MMO.Types.Realms;


namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Realms
            {
                /// <summary>
                ///   This realm involves a single profile
                ///   per account. Such profile is embedded
                ///   in the same account (i.e. loading the
                ///   account involves loading the profile,
                ///   and any missing data is game-specific).
                /// </summary>
                /// <typeparam name="AccountIDType">The type of the account id (e.g. int)</typeparam>
                /// <typeparam name="AccountType">The type of the account preview data</typeparam>
                /// <typeparam name="AccountPreviewType">The type of the account data</typeparam>
                public abstract class BasicRealm<AccountIDType, AccountPreviewType, AccountType> : Realm
                    where AccountPreviewType : IAccountPreview<AccountIDType>
                    where AccountType : IAccount<AccountIDType, AccountPreviewType>
                {
                }
            }
        }
    }
}
