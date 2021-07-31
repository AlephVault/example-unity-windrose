using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
                /// <typeparam name="AccountIDType"></typeparam>
                /// <typeparam name="AccountType"></typeparam>
                public abstract class SingleProfileRealm<AccountIDType, AccountType> : Realm
                {
                }
            }
        }
    }
}
