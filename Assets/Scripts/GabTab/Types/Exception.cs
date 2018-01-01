namespace GabTab
{
    namespace Types
    {
        public class Exception : Support.Types.Exception
        {
            public Exception() {}
            public Exception(string message) : base(message) {}
            public Exception(string message, System.Exception inner) : base(message, inner) {}
        }
    }
}