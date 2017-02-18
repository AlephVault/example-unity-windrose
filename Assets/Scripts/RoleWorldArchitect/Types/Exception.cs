using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoleWorldArchitect
{
    namespace Types
    {
        public class Exception : System.Exception
        {
            public Exception() {}
            public Exception(string message) : base(message) {}
            public Exception(string message, System.Exception inner) : base(message, inner) {}
        }
    }
}