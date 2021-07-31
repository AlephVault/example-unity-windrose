namespace AlephVault.States
{
    namespace Types
    {
        public class Exception : System.Exception
        {
            public Exception() : base() {}
            public Exception(string message) : base(message) {}
            public Exception(string message, System.Exception innerException) : base(message, innerException) {}
        }
    }
}