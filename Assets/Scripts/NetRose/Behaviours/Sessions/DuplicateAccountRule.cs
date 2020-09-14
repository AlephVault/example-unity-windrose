namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   Tells what to do when an account logs in while
            ///     it is currently logged in another connection.
            ///     The "Kick" rule stands for kicking the new
            ///     connection, while the "Ghost" rule stands
            ///     for kicking the old connection.
            /// </summary>
            public enum DuplicateAccountRule
            {
                Kick, Ghost
            }
        }
    }
}
