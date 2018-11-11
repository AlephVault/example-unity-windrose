using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Inventory
        {
            namespace Items
            {
                using Support.Utils;

                public class RequireDisplayStrategy : AssetsLayout.Depends
                {
                    public RequireDisplayStrategy(Type dependency) : base(dependency)
                    {
                    }

                    protected override Type BaseDependency()
                    {
                        return typeof(DisplayStrategies.ItemDisplayStrategy);
                    }
                }
            }
        }
    }
}
