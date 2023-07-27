namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   This agent relates to having a time pool to grab
        ///   more "forgiveness" seconds from after a timeout.
        /// </summary>
        public interface ITimePoolHoldingAgent
        {
            /// <summary>
            ///   Gets the current time pool.
            /// </summary>
            public int GetTimePool();

            /// <summary>
            ///   Sets the new time pool. Negative values are
            ///   zero-maxed.
            /// </summary>
            public int SetTimePool();
        }
    }
}