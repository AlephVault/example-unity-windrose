using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace Bags
            {
                /// <summary>
                ///   A singleton position. The only expression that matters here is
                ///     <see cref="Instance"/> and only in the context of the implementation
                ///     of <see cref="InventorySinglePositioningManagementStrategy"/>.
                /// </summary>
                public class Position
                {
                    private static Position instance = null;
                    private Position() { }

                    /// <summary>
                    ///   Implementing this method allows this type to be used consistently
                    ///     as a dictionary key.
                    /// </summary>
                    /// <returns>0</returns>
                    public override int GetHashCode()
                    {
                        return 0;
                    }

                    /// <summary>
                    ///   The singleton expression ít will be used.
                    /// </summary>
                    public static Position Instance
                    {
                        get
                        {
                            if (instance == null)
                            {
                                instance = new Position();
                            }
                            return instance;
                        }
                    }
                }
            }
        }
    }
}
