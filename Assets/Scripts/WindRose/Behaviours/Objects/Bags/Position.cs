using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Bags
            {
                public class Position
                {
                    private static Position instance = null;
                    private Position() { }
                    public override int GetHashCode()
                    {
                        return 0;
                    }
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
