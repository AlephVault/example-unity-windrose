﻿using System;
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

                /// <summary>
                ///   This is an attribute to be used on item Rendering Strategies. This attribute ensures a 
                ///     strategy requires another particular rendering strategy, because their functionalities
                ///     are dependent somehow. This requirement is both in runtime and as documentation.
                /// </summary>
                public class RequireRenderingStrategy : AssetsLayout.Depends
                {
                    public RequireRenderingStrategy(Type dependency) : base(dependency)
                    {
                    }

                    protected override Type BaseDependency()
                    {
                        return typeof(RenderingStrategies.ItemRenderingStrategy);
                    }
                }
            }
        }
    }
}
