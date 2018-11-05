using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace ScriptableObjects
    {
        namespace Tiles
        {
            [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
            public class RequireTileStrategy : Support.Utils.AssetsLayout.Depends
            {
                public RequireTileStrategy(Type dependency) : base(dependency) {}

                protected override Type BaseDependency()
                {
                    return typeof(Strategies.TileStrategy);
                }
            }
        }
    }
}
