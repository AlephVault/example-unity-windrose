using System;
using AlephVault.Unity.Binary;

namespace AlephVault.Unity.EVMGames.Auth
{
    namespace Types
    {
        /// <summary>
        ///   Challenge utils related to timestamps (and
        ///   their checks) to avoid replay attacks.
        /// </summary>
        public static class ChallengeUtils
        {
            /// <summary>
            ///   Generates the current timestamp for the
            ///   challenge.
            /// </summary>
            /// <returns>A timestamp, with microseconds</returns>
            public static uint CurrentTimestamp()
            {
                return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }

            /// <summary>
            ///   Checks whether the given timestamp is close
            ///   to the current timestamp, given a tolerance.
            /// </summary>
            /// <param name="timestamp">The given timestamp</param>
            /// <param name="tolerance">The tolerance</param>
            /// <returns>
            ///   Whether the given timestamp is close to the current timestamp (in terms of the tolerance)
            /// </returns>
            public static bool IsCloseToNow(uint timestamp, uint tolerance)
            {
                return Math.Abs(timestamp - CurrentTimestamp()) < tolerance;
            }
        }
    }
}