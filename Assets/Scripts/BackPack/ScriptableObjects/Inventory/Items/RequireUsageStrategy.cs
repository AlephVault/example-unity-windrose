using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackPack
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                using AlephVault.Unity.Layout.Utils;

                /// <summary>
                ///   This is an attribute to be used on item Rendering Strategies. This attribute ensures a 
                ///     strategy requires a particular usage strategy, because their functionalities are
                ///     dependent somehow. This requirement is both in runtime and as documentation.
                /// </summary>
                public class RequireUsageStrategy : Assets.Depends
                {
                    public RequireUsageStrategy(Type dependency) : base(dependency)
                    {
                    }

                    protected override Type BaseDependency()
                    {
                        return typeof(UsageStrategies.ItemUsageStrategy);
                    }
                }
            }
        }
    }
}
