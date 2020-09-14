using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   Triggered on standar session error.
            /// </summary>
            public class Exception : Types.Exception
            {
                public Exception() { }
                public Exception(string message) : base(message) { }
                public Exception(string message, System.Exception inner) : base(message, inner) { }
            }
        }
    }
}
