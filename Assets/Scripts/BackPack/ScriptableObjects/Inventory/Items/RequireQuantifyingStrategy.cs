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
                using GMM.Utils;

                /// <summary>
                ///   This is an attribute to be used on item Rendering/Usage Strategies. This attribute ensures a 
                ///     strategy requires a particular quantifying strategy, because their functionalities are
                ///     dependent somehow. This requirement is both in runtime and as documentation.
                /// </summary>
                public class RequireQuantifyingStrategy : AssetsLayout.Depends
                {
                    public RequireQuantifyingStrategy(Type dependency) : base(dependency)
                    {
                    }

                    protected override Type BaseDependency()
                    {
                        return typeof(QuantifyingStrategies.ItemQuantifyingStrategy);
                    }
                }
            }
        }
    }
}
